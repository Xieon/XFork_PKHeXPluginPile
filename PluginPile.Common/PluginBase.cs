using PKHeX.Core;
using System.Reflection;

namespace PluginPile.Common;
public abstract class PluginBase : IPlugin {
  public abstract string Name { get; }
  public virtual int Priority => 1; // Loading order, lowest is first.

  // Initialized on plugin load
  protected object[] Args { get; private set; } = null!;
  public static Form MainWindow { get; private set; } = null!;
  public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
  public IPKMView PKMEditor { get; private set; } = null!;

  public virtual void Initialize(params object[] args) {
    Console.WriteLine($"Loading {Name}...");
    Args = args;
    SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider)!;
    PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView)!;
    MainWindow = (Form)Array.Find(args, z => z is Form)!;
    ToolStrip menu = (ToolStrip)Array.Find(args, z => z is ToolStrip)!;
    ToolStripDropDownItem tools = (ToolStripDropDownItem)menu.Items.Find("Menu_Tools", false)[0]!;
    LoadMenu(tools);
    // Since the classes for these are in PKHeX.WinForms but not PKHeX.Core have to do this to get the box context menu
    // (SAVEditor)SaveFileEditor.(BoxMenuStrip)SortMenu
    ContextMenuStrip boxMenu = (ContextMenuStrip)((dynamic)SaveFileEditor).SortMenu;
    LoadBoxMenu(boxMenu);
    // Since the classes for these are in PKHeX.WinForms but not PKHeX.Core have to do this to get the slot context menu
    // (SAVEditor)SaveFileEditor.(ContextMenuSAV)menu.(ContextMenuStrip)mnuVSD
    ContextMenuStrip contextMenu = (ContextMenuStrip)((dynamic)SaveFileEditor).menu.mnuVSD;
    LoadContextMenu(contextMenu);
  }

  protected virtual void LoadMenu(ToolStripDropDownItem tools) { }

  protected virtual void LoadBoxMenu(ContextMenuStrip boxMenu) { }

  protected virtual void LoadContextMenu(ContextMenuStrip contextMenu) { }

  public virtual void NotifySaveLoaded() {
    Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
    HandleSaveLoaded();
  }

  protected virtual void HandleSaveLoaded() { }

  public virtual void NotifyDisplayLanguageChanged(string language) { }

  public bool TryLoadFile(string filePath) {
    Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
    return false; // no action taken
  }

  protected SlotViewInfo<PictureBox> GetSenderInfo(ref object sender) {
    // Accessing private static method GetSenderInfo by using (SAVEditor)SaveFileEditor.(ContextMenuSAV)menu and reflection
    Type contextMenuSAVType = ((dynamic)SaveFileEditor).menu.GetType();
    MethodInfo getSenderInfoMethod = contextMenuSAVType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
      .Single(m => m.Name.Contains("GetSenderInfo"));
    return (SlotViewInfo<PictureBox>)getSenderInfoMethod.Invoke(null, [sender])!;
  }

  protected BoxManipulator GetBoxManipulatorWF() {
    Type BoxManipulatorWFType = ((dynamic)SaveFileEditor).SortMenu.GetType().Assembly.GetType("PKHeX.WinForms.Controls.BoxManipulatorWF");
    return (BoxManipulator)Activator.CreateInstance(BoxManipulatorWFType, SaveFileEditor)!;
  }

}

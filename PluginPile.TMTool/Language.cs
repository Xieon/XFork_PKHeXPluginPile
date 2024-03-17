using PKHeX.Core;

namespace PluginPile.TMTool;
internal static class Language {

  public static string MenuItemName {
    get {
      return GameInfo.CurrentLanguage switch {
        "zh"      => "TM 工具",
        "en" or _ => "TM Tool"
      };
    }
  }

  public static string AddProtect {
    get {
      return GameInfo.CurrentLanguage switch {
        "zh"      => "添加保护",
        "en" or _ => "Add Protect"
      };
    }
  }

  public static string LearnTMs {
    get {
      return GameInfo.CurrentLanguage switch {
        "zh"      => "学习 TM",
        "en" or _ => "Learn TMs"
      };
    }
  }

}

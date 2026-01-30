using System.Diagnostics;
using System.Globalization;
using UnityEngine.Localization;

namespace Infrastructure.Commons
{
  public static class LocalizationUtils
  {
    public static string GetSystemLanguageCode()
    {
      CultureInfo cultureInfo = CultureInfo.InstalledUICulture;
      return GetLanguageCode(cultureInfo);
    }
    
    public static string GetLanguageCode(CultureInfo cultureInfo)
    {
      switch (cultureInfo.TwoLetterISOLanguageName)
      {
        case "ko":
          return "ko";
        case "en":
          return "en";
        case "ja":
          return "ja";
        default:
          return "en";
      }
    }

    public static string GetLanguageCode(Locale locale)
    {
      if (locale == null) return "en";

      switch (locale.Identifier.Code)
      {
        case "ko":
        case "ko-KR":
          return "ko";
        case "en":
        case "en-US":
          return "en";
        case "ja":
        case "ja-JP":
          return "ja";
        default:
#if UNITY_EDITOR
          UnityEngine.Debug.LogWarning($"Unsupported locale code: {locale.Identifier.Code}, defaulting to 'en'");
#endif
          return "en";
      }
    }
  }
}

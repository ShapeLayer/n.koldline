using System.Globalization;

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
  }
}

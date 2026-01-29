using Infrastructure.Commons;

namespace Infrastructure.Definitions
{
  public partial class Defaults
  {
    public static readonly LocaleData[] SUPPORT_LOCALES = 
    {
      new LocaleData { LocaleCode = "en", DisplayName = "English" },
      new LocaleData { LocaleCode = "ja", DisplayName = "日本語" },
      new LocaleData { LocaleCode = "ko", DisplayName = "한국어" },
    };
  }
}

using System.Globalization;
using System.Threading.Tasks;
using GamePlay.Definitions;
using Infrastructure.Commons;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace GamePlay.IntroPlay
{
  public static class IntroSceneL10nCollections
  {
    static StringTable currLangTable;

    static IntroSceneL10nCollections()
    {
      // Defer initialization to async methods to avoid blocking the main thread.
    }

    public static Task SetLocaleAsync(CultureInfo cultureInfo)
    {
      return SetLocaleAsync(LocalizationUtils.GetLanguageCode(cultureInfo));
    }

    public static async Task SetLocaleAsync(string localeCode)
    {
      if (string.IsNullOrWhiteSpace(localeCode)) return;

      await EnsureInitializedAsync();

      Locale locale = FindLocale(localeCode)
                      ?? FindLocale("en");

      if (locale == null) return;

      if (LocalizationSettings.SelectedLocale == locale) return;

      LocalizationSettings.SelectedLocale = locale;

      var tableOp = LocalizationSettings.StringDatabase.GetTableAsync(Defaults.TABLE_COLLECTION_ID, locale);
      await tableOp.Task;
      currLangTable = tableOp.Result;
    }

    public static Locale CurrentLocale => LocalizationSettings.SelectedLocale;

    public static string Q(string key)
    {
      if (currLangTable == null)
      {
        if (!LocalizationSettings.InitializationOperation.IsDone)
        {
          return key;
        }

        var locale = LocalizationSettings.SelectedLocale;
        if (locale != null)
        {
          currLangTable = LocalizationSettings.StringDatabase.GetTable(Defaults.TABLE_COLLECTION_ID, locale);
        }
      }

      if (currLangTable == null) return key;

      var entry = currLangTable.GetEntry(key);
      return entry == null ? key : entry.LocalizedValue;
    }

    static async Task EnsureInitializedAsync()
    {
      var init = LocalizationSettings.InitializationOperation;
      if (!init.IsDone)
      {
        await init.Task;
      }
    }

    static Locale FindLocale(string localeCode)
    {
      if (string.IsNullOrWhiteSpace(localeCode)) return null;

      var locales = LocalizationSettings.AvailableLocales;
      if (locales == null) return null;

      var exact = locales.GetLocale(localeCode);
      if (exact != null) return exact;

      foreach (var locale in locales.Locales)
      {
        if (locale == null) continue;

        var identifier = locale.Identifier;
        if (identifier.Code == localeCode) return locale;

        var culture = identifier.CultureInfo;
        if (culture != null && culture.TwoLetterISOLanguageName == localeCode) return locale;
      }

      return null;
    }
  }
}

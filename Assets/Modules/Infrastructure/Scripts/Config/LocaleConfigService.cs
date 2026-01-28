using System;

namespace Infrastructure.Config
{
  /**
   * TODO: Deserialize and settings panel
   */
  
  public static class LocaleConfigService
  {
    public static bool SaveSelectedLocale(string localeCode)
    {
#if UNITY_EDITOR && KOLDLINE_DEBUG_SERIALIZE_CONFIG
      UnityEngine.Debug.Log($"Saving selected locale: {localeCode}");
#endif
      if (string.IsNullOrWhiteSpace(localeCode)) return false;

      try
      {
        var config = ConfigManager.Instance;
        config.CurrentLocale = localeCode;
        return ConfigManager.Save(config);
      }
      catch (Exception e)
      {
#if UNITY_EDITOR && KOLDLINE_DEBUG_SERIALIZE_CONFIG
        Debug.LogError($"Failed to save locale config: {e.Message}");
#endif
        return false;
      }
    }
  }
}

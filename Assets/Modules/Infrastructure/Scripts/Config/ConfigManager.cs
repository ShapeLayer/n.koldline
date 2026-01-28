using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Infrastructure.Definitions;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Infrastructure.Config
{
  public static class ConfigManager
  {
    // Define paths relative to the application execution directory
    private static readonly string ConfigPath = Path.Combine(Application.dataPath, Defaults.CONFIG_CONFIG_FILE_PATH);

    private static readonly Lazy<ConfigModel> _instance = new Lazy<ConfigModel>(Load);
    public static ConfigModel Instance => _instance.Value;

    /// <summary>
    /// Loads the config from disk. If the file is missing or invalid, returns a default model.
    /// </summary>
    public static ConfigModel Load()
    {
      if (!File.Exists(ConfigPath))
      {
#if UNITY_EDITOR && KOLDLINE_DEBUG_SERIALIZE_CONFIG
        UnityEngine.Debug.Log("Config file not found. Creating default.");
#endif
        ConfigModel defaultModel = new ConfigModel();
        Save(defaultModel);
        return defaultModel;
      }

      try
      {
        string json = File.ReadAllText(ConfigPath);

        // Validate against schema before deserializing
        if (!ConfigValidator.TryValidate(json, out List<string> errors))
        {
          Console.WriteLine("Config validation failed! Errors:");
          errors.ForEach(e => Console.WriteLine($"- {e}"));

          // Option: Return default or throw exception
          return new ConfigModel();
        }

        return ConfigModel.FromJson(json);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed to load config: {ex.Message}");
        return new ConfigModel();
      }
    }

    /// <summary>
    /// Saves the config model to disk after validating it.
    /// </summary>
    public static bool Save(ConfigModel config)
    {
#if UNITY_EDITOR && KOLDLINE_DEBUG_SERIALIZE_CONFIG
      UnityEngine.Debug.Log("Saving config to disk...");
#endif
      try
      {
        string json = config.ToJson();

        // Self-validate before writing to ensure we don't break the file on disk
        if (!ConfigValidator.TryValidate(json, out List<string> errors))
        {
#if UNITY_EDITOR && KOLDLINE_DEBUG_SERIALIZE_CONFIG
          UnityEngine.Debug.LogError("Cannot save: Config model violates schema.");
          UnityEngine.Debug.Log($"Model JSON: {json}");
          errors.ForEach(e => UnityEngine.Debug.LogError($"- {e}"));
#endif
          Console.WriteLine("Cannot save: Config model violates schema.");
          errors.ForEach(e => Console.WriteLine($"- {e}"));
          return false;
        }

        File.WriteAllText(ConfigPath, json);
        return true;
      }
      catch (Exception ex)
      {
#if UNITY_EDITOR && KOLDLINE_DEBUG_SERIALIZE_CONFIG
        UnityEngine.Debug.LogError($"Failed to save config: {ex.Message}");
#endif
        Console.WriteLine($"Failed to save config: {ex.Message}");
        return false;
      }
    }
  }
}

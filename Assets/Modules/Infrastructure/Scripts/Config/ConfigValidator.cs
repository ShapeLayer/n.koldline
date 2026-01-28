using System;
using System.IO;
using System.Text.Json;
using Json.Schema;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Definitions;
using UnityEngine;

namespace Infrastructure.Config
{
  public static class ConfigValidator
  {
    private const string SchemaPath = Defaults.CONFIG_SCHEMA_FILE_PATH;
    
    /// <summary>
    /// Validates a JSON string against a schema file.
    /// </summary>
    /// <param name="json">The JSON content to validate.</param>
    /// <param name="schemaPath">The physical path to the .schema.json file.</param>
    /// <param name="errors">Output list of validation errors if any.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool TryValidate(string json, out List<string> errors)
    {
      errors = new List<string>();
      TextAsset schemaAsset = Resources.Load<TextAsset>(SchemaPath);
      if (schemaAsset == null)
      {
        errors.Add($"Schema file not found in Resources at: {SchemaPath}");
        return false;
      }

      try
      {
        string schemaContent = schemaAsset.text;
        JsonSchema schema = JsonSchema.FromText(schemaContent);

        // Parse the input JSON
        using JsonDocument instance = JsonDocument.Parse(json);

        // Evaluate
        EvaluationResults results = schema.Evaluate(instance.RootElement, new EvaluationOptions
        {
          OutputFormat = OutputFormat.List
        });

        if (results.IsValid) return true;

        // Extract human-readable errors
        foreach (var detail in results.Details.Where(d => d.Errors != null && d.Errors.Count > 0))
        {
          foreach (var error in detail.Errors)
          {
            errors.Add($"[{detail.InstanceLocation}] {error.Value}");
          }
        }

        return false;
      }
      catch (JsonException ex)
      {
        errors.Add($"Invalid JSON Syntax: {ex.Message}");
        return false;
      }
      catch (Exception ex)
      {
        errors.Add($"Validation Error: {ex.Message}");
        return false;
      }
    }
  }
}

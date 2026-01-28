using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Config
{
  public class ConfigModel
  {
    [JsonPropertyName("current_locale")]
    public string CurrentLocale { get; set; } = "en";

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

    public static ConfigModel FromJson(string json)
      => JsonSerializer.Deserialize<ConfigModel>(json) ?? new ConfigModel();
  }
}

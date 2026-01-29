using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.UIElements
{
  public static class MultilingualFontFallback
  {
    public static void ApplyToLabel(Label label, ref Font currentFont, string content, Font[] fallbackFonts)
    {
      if (label == null)
        return;

      var font = ResolveFontForText(content ?? string.Empty, fallbackFonts);
      if (font == null || ReferenceEquals(currentFont, font))
        return;

      currentFont = font;
      label.style.unityFontDefinition = FontDefinition.FromFont(font);
    }

    public static Font ResolveFontForText(string text, Font[] fallbackFonts)
    {
      if (fallbackFonts == null || fallbackFonts.Length == 0)
        return null;

      Font defaultFont = null;
      foreach (var font in fallbackFonts)
      {
        if (font != null)
        {
          defaultFont = font;
          break;
        }
      }

      if (defaultFont == null)
        return null;

      if (string.IsNullOrEmpty(text))
        return defaultFont;

      foreach (var font in fallbackFonts)
      {
        if (font == null)
          continue;

        var supportsAll = true;
        foreach (var ch in text)
        {
          if (!font.HasCharacter(ch))
          {
            supportsAll = false;
            break;
          }
        }

        if (supportsAll)
          return font;
      }

      return defaultFont;
    }
  }
}

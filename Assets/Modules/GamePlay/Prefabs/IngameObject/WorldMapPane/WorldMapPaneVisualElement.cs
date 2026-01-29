using System;
using System.Collections.Generic;
using System.IO;
using Infrastructure.UIElements;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.VisualElements
{
  [UxmlElement]
  public partial class WorldMapPaneVisualElement : VisualElement
  {
    const string DefaultTimerText = "00:00:00.000";

    static readonly Font[] s_FallbackFonts = InitializeFonts();

    static readonly Dictionary<string, Sprite> s_SpriteCache = new();

    readonly VisualElement _mapHolder;
    readonly VisualElement _timerBar;
    readonly Label _timerLabel;
    Font _currentFont;

    public WorldMapPaneVisualElement()
    {
      name = "world-map-pane";
      AddToClassList("world-map-pane");
      style.flexGrow = 1f;

      _mapHolder = new VisualElement { name = "world-map-holder" };
      _mapHolder.AddToClassList("world-map-holder");
      _mapHolder.pickingMode = PickingMode.Ignore;
      Add(_mapHolder);

      _timerBar = new VisualElement { name = "world-map-timer-bar" };
      _timerBar.AddToClassList("world-map-timer-bar");
      _timerBar.pickingMode = PickingMode.Ignore;
      Add(_timerBar);

      _timerLabel = new Label(DefaultTimerText) { name = "world-map-timer" };
      _timerLabel.AddToClassList("world-map-timer");
      _timerLabel.pickingMode = PickingMode.Ignore;
      _timerLabel.style.fontSize = 16;
      _timerLabel.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f));
      // font
      _timerBar.Add(_timerLabel);

      MultilingualFontFallback.ApplyToLabel(_timerLabel, ref _currentFont, DefaultTimerText, s_FallbackFonts);
      SetTimerVisible(false);
    }

    public Label TimerLabel => _timerLabel;
    public VisualElement MapHolder => _mapHolder;
    public bool IsTimerVisible => _timerBar.style.display != DisplayStyle.None;

    public void SetTimerVisible(bool visible)
    {
      _timerBar.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void ShowTimer() => SetTimerVisible(true);
    public void HideTimer() => SetTimerVisible(false);

    public void SetTimerText(string text)
    {
      var resolved = string.IsNullOrEmpty(text) ? DefaultTimerText : text;
      MultilingualFontFallback.ApplyToLabel(_timerLabel, ref _currentFont, resolved, s_FallbackFonts);
      _timerLabel.text = resolved;
    }

    static Font[] InitializeFonts()
    {
      var fonts = new List<Font>
      {
        Resources.Load<Font>("Fonts/SarasaMono/SarasaMonoK-Regular"),
        Resources.Load<Font>("Fonts/SarasaMono/SarasaMonoJ-Regular"),
      };

      fonts.RemoveAll(font => font == null);
      return fonts.ToArray();
    }

    public void SetTimerTime(TimeSpan time)
    {
      SetTimerText(FormatTimerTime(time));
    }

    public void SetTimerTimeSeconds(double totalSeconds)
    {
      SetTimerTime(TimeSpan.FromSeconds(Math.Max(0.0, totalSeconds)));
    }

    public void SetTimerTimeMilliseconds(int totalMilliseconds)
    {
      SetTimerTimeMilliseconds((long)totalMilliseconds);
    }

    public void SetTimerTimeMilliseconds(long totalMilliseconds)
    {
      SetTimerTime(TimeSpan.FromMilliseconds(Math.Max(0L, totalMilliseconds)));
    }

    // Back-compat wrappers (old name was "Elapsed")
    public void SetElapsed(TimeSpan elapsed) => SetTimerTime(elapsed);
    public void SetElapsedMilliseconds(int totalMilliseconds) => SetTimerTimeMilliseconds(totalMilliseconds);
    public void SetElapsedMilliseconds(long totalMilliseconds) => SetTimerTimeMilliseconds(totalMilliseconds);

    public void SetMapSprite(Sprite sprite)
    {
      if (sprite == null)
      {
        _mapHolder.style.backgroundImage = StyleKeyword.None;
        return;
      }

      _mapHolder.style.backgroundImage = new StyleBackground(sprite);
    }

    public void LoadMapFromResources(string resourcesPathWithoutExtension)
    {
      if (string.IsNullOrWhiteSpace(resourcesPathWithoutExtension))
        return;

      if (s_SpriteCache.TryGetValue(resourcesPathWithoutExtension, out var cached) && cached != null)
      {
        SetMapSprite(cached);
        return;
      }

      // If the SVG is imported as a Sprite by Unity, this will succeed.
      var sprite = Resources.Load<Sprite>(resourcesPathWithoutExtension);
      if (sprite != null)
      {
        s_SpriteCache[resourcesPathWithoutExtension] = sprite;
        SetMapSprite(sprite);
        return;
      }

      // If the SVG remains as raw text, load it as a TextAsset and rasterize to a Sprite at runtime.
      var svgTextAsset = Resources.Load<TextAsset>(resourcesPathWithoutExtension);
      if (svgTextAsset == null)
      {
        Debug.LogWarning($"[WorldMapPane] Could not load map from Resources path '{resourcesPathWithoutExtension}'. Expected Sprite or TextAsset.");
        return;
      }

      try
      {
        var generated = BuildSpriteFromSvg(svgTextAsset.text);
        if (generated == null)
          return;

        s_SpriteCache[resourcesPathWithoutExtension] = generated;
        SetMapSprite(generated);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }

    static Sprite BuildSpriteFromSvg(string svgText)
    {
      if (string.IsNullOrWhiteSpace(svgText))
        return null;

      using var reader = new StringReader(svgText);
      var sceneInfo = SVGParser.ImportSVG(reader, pixelsPerUnit: 100.0f);

      var tessellationOptions = new VectorUtils.TessellationOptions
      {
        StepDistance = 1.0f,
        MaxCordDeviation = 0.5f,
        MaxTanAngleDeviation = 0.1f,
        SamplingStepSize = 0.01f,
      };

      var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessellationOptions);
      return VectorUtils.BuildSprite(
          geoms,
          100.0f,
          VectorUtils.Alignment.Center,
          Vector2.zero,
          128,
          true);
    }

    public static string FormatTimerTime(TimeSpan time)
    {
      if (time < TimeSpan.Zero)
        time = TimeSpan.Zero;

      var hours = (int)Math.Floor(time.TotalHours);
      return $"{hours:00}:{time.Minutes:00}:{time.Seconds:00}.{time.Milliseconds:000}";
    }

  }
}


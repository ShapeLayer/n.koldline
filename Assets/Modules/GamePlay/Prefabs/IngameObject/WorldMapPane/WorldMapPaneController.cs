using System;
using GamePlay.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.WorldMapPane
{
  [RequireComponent(typeof(UIDocument))]
  public sealed class WorldMapPaneController : MonoBehaviour
  {
    [Header("UI")]
    [SerializeField] UIDocument uiDocument;
    [SerializeField] string resourcesMapPath = "VectorGraphics/pacific-world-map";
    [SerializeField] bool loadMapOnEnable = true;

    WorldMapPaneVisualElement _pane;
    TimeSpan _timerTime;
    string _lastTimerText;

    public WorldMapPaneVisualElement Pane => _pane;
    public TimeSpan TimerTime => _timerTime;

    void Reset()
    {
      uiDocument = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
      if (uiDocument == null)
        uiDocument = GetComponent<UIDocument>();

      EnsurePane();

      if (loadMapOnEnable)
        LoadMapFromResources(resourcesMapPath);

      SetTimerTime(_timerTime);
    }

    public void SetTimerTime(TimeSpan time)
    {
      _timerTime = time < TimeSpan.Zero ? TimeSpan.Zero : time;
      EnsurePane();
      if (_pane == null)
        return;

      var text = WorldMapPaneVisualElement.FormatTimerTime(_timerTime);

      // Since this is driven externally (often per-frame), avoid redundant UI churn.
      if (string.Equals(_lastTimerText, text, StringComparison.Ordinal))
        return;

      _lastTimerText = text;
      _pane.SetTimerText(text);
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

    public void SetTimerText(string text)
    {
      EnsurePane();
      _pane?.SetTimerText(text);
      _lastTimerText = text;
    }

    public void ShowTimer()
    {
      EnsurePane();
      _pane?.ShowTimer();
    }

    public void HideTimer()
    {
      EnsurePane();
      _pane?.HideTimer();
    }

    public void SetTimerVisible(bool visible)
    {
      EnsurePane();
      _pane?.SetTimerVisible(visible);
    }

    public void LoadMapFromResources(string resourcesPathWithoutExtension)
    {
      EnsurePane();
      _pane?.LoadMapFromResources(resourcesPathWithoutExtension);
    }

    public void SetMapSprite(Sprite sprite)
    {
      EnsurePane();
      _pane?.SetMapSprite(sprite);
    }

    void EnsurePane()
    {
      if (_pane != null)
        return;

      if (uiDocument == null)
        return;

      var root = uiDocument.rootVisualElement;
      if (root == null)
        return;

      _pane = root.Q<WorldMapPaneVisualElement>("world-map-pane");
      if (_pane == null)
      {
        _pane = new WorldMapPaneVisualElement();
        root.Add(_pane);
      }
    }

  }
}

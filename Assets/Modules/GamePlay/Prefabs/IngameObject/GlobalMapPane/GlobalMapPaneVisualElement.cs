using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.IngameObject
{
  [UxmlElement]
  public partial class GlobalMapPaneVisualElement : VisualElement
  {
    private const float TravelerSize = 20f;
    private const float TravelerSpeed = 0.3f;

    private VisualElement _traveler;
    private Vector2 _startPos, _endPos, _controlPos;
    private float _progress;
    private bool _hasBackground;

    [UxmlAttribute]
    public string MapResourcePath { get; set; }

    public GlobalMapPaneVisualElement()
    {
      Init(null);
    }

    public GlobalMapPaneVisualElement(VectorImage mapSvg)
    {
      Init(mapSvg);
    }

    private void Init(VectorImage mapSvg)
    {
      style.flexGrow = 1;
      style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
      style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
      style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
      style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

      if (mapSvg != null)
      {
        style.backgroundImage = new StyleBackground(mapSvg);
        _hasBackground = true;
      }

      _traveler = new VisualElement
      {
        style =
                {
                    width = TravelerSize,
                    height = TravelerSize,
                    backgroundColor = Color.red,
                    position = Position.Absolute,
                    borderTopLeftRadius = TravelerSize * 0.5f,
                    borderTopRightRadius = TravelerSize * 0.5f,
                    borderBottomLeftRadius = TravelerSize * 0.5f,
                    borderBottomRightRadius = TravelerSize * 0.5f
                }
      };
      Add(_traveler);

      generateVisualContent += OnGenerateVisualContent;
      RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
      schedule.Execute(UpdatePath).Every(16);
    }

    private void UpdatePath()
    {
      float w = resolvedStyle.width;
      float h = resolvedStyle.height;
      if (float.IsNaN(w) || w <= 0 || float.IsNaN(h) || h <= 0) return;

      _startPos = new Vector2(w * 0.1f, h * 0.8f);
      _endPos = new Vector2(w * 0.9f, h * 0.2f);
      _controlPos = new Vector2(w * 0.5f, h * -0.1f);

      _progress += Time.deltaTime * TravelerSpeed;
      if (_progress > 1f) _progress = 0f;

      Vector2 pos = GetBezier(_startPos, _controlPos, _endPos, _progress);
      _traveler.style.left = pos.x - (TravelerSize * 0.5f);
      _traveler.style.top = pos.y - (TravelerSize * 0.5f);

      MarkDirtyRepaint();
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
      var painter = mgc.painter2D;
      painter.BeginPath();
      painter.lineWidth = 4f;
      painter.strokeColor = Color.yellow;
      painter.MoveTo(_startPos);
      painter.QuadraticCurveTo(_controlPos, _endPos);
      painter.Stroke();
    }

    private Vector2 GetBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
      float invT = 1 - t;
      return invT * invT * p0 + 2 * invT * t * p1 + t * t * p2;
    }

    private void OnAttachToPanel(AttachToPanelEvent evt)
    {
      if (!_hasBackground && !string.IsNullOrWhiteSpace(MapResourcePath))
      {
        var mapSvg = Resources.Load<VectorImage>(MapResourcePath);
        if (mapSvg != null)
        {
          style.backgroundImage = new StyleBackground(mapSvg);
          _hasBackground = true;
        }
      }
    }
  }
}

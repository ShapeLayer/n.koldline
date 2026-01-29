using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.VisualElements
{
  [UxmlElement]
  public partial class TitleOverlayElement : VisualElement
  {
    readonly Label _titleLabel;

    public TitleOverlayElement()
    {
      name = "title-overlay";
      AddToClassList("title-overlay");
      style.flexGrow = 1f;
      style.flexDirection = FlexDirection.Column;
      style.justifyContent = Justify.Center;
      style.alignItems = Align.Center;

      _titleLabel = new Label();
      _titleLabel.name = "title-overlay-text";
      _titleLabel.AddToClassList("title-overlay-text");
      _titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
      _titleLabel.style.color = Color.white;
      _titleLabel.style.fontSize = 36;
      _titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
      _titleLabel.style.unityTextOutlineWidth = 1.2f;
      _titleLabel.style.unityTextOutlineColor = new Color(0f, 0f, 0f, 0.8f);
      _titleLabel.style.whiteSpace = WhiteSpace.Normal;
      _titleLabel.style.maxWidth = Length.Percent(90);
      _titleLabel.style.alignSelf = Align.Center;
      Add(_titleLabel);
    }

    public Label TitleLabel => _titleLabel;

    public void SetTitle(string content)
    {
      _titleLabel.text = content ?? string.Empty;
    }
  }
}

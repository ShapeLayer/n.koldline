using System.Collections.Generic;
using Infrastructure.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.VisualElements
{
  [UxmlElement]
  public partial class TitleOverlayElement : VisualElement
  {
    static readonly Font[] s_FallbackFonts = InitializeFonts();
    readonly Label _titleLabel;
    Font _currentFont;

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
      _titleLabel.style.fontSize = 20;
      _titleLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
      _titleLabel.style.backgroundColor = new Color(0f, 0f, 0f);
      _titleLabel.style.paddingLeft = 10;
      _titleLabel.style.paddingRight = 10;
      _titleLabel.style.paddingTop = 5;
      _titleLabel.style.paddingBottom = 5;
      _titleLabel.style.whiteSpace = WhiteSpace.Normal;
      _titleLabel.style.maxWidth = Length.Percent(90);
      _titleLabel.style.alignSelf = Align.Center;
      Add(_titleLabel);

      MultilingualFontFallback.ApplyToLabel(_titleLabel, ref _currentFont, string.Empty, s_FallbackFonts);
    }

    public Label TitleLabel => _titleLabel;

    public void SetTitle(string content)
    {
      MultilingualFontFallback.ApplyToLabel(_titleLabel, ref _currentFont, content, s_FallbackFonts);
      _titleLabel.text = content ?? string.Empty;
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
  }
}

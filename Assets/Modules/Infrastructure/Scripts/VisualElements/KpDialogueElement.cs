using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;
using Infrastructure.Definitions;

namespace Infrastructure.VisualElements
{
  [UxmlElement]
  public partial class KpDialogueElement : VisualElement
  {
    static readonly Font KpPuskumFont = Resources.Load<Font>("fonts/kppuskum/kppuskum");

    readonly Label _primaryLabel;
    readonly Label _secondaryLabel;
    readonly Label _clickHint;

    public KpDialogueElement()
    {
      name = "kp-dialogue";
      AddToClassList("kp-dialogue");
      style.flexGrow = 1f;
      style.flexDirection = FlexDirection.Column;
      style.justifyContent = Justify.Center;
      style.alignItems = Align.Center;
      style.backgroundColor = new Color(Defaults.BASE_BACKGROUND_COLOR[0] / 255f, Defaults.BASE_BACKGROUND_COLOR[1] / 255f, Defaults.BASE_BACKGROUND_COLOR[2] / 255f);

      _primaryLabel = CreateLabel("kp-dialogue-primary");
      _secondaryLabel = CreateLabel("kp-dialogue-secondary");
      ApplyFont(_primaryLabel);
      ApplyFont(_secondaryLabel);
      _clickHint = CreateHint();

      Add(_primaryLabel);
      Add(_secondaryLabel);
      Add(_clickHint);
    }

    static Label CreateLabel(string className)
    {
      var label = new Label();
      label.name = className;
      label.AddToClassList(className);
      label.style.unityTextAlign = TextAnchor.MiddleCenter;
      label.style.color = Color.white;
      label.style.fontSize = 24;
      label.style.whiteSpace = WhiteSpace.Normal;
      label.style.maxWidth = Length.Percent(80);
      label.style.unityFontStyleAndWeight = FontStyle.Bold;
      label.style.alignSelf = Align.Center;
      return label;
    }

    static void ApplyFont(Label label)
    {
      if (label == null) return;
      if (KpPuskumFont != null)
        label.style.unityFontDefinition = FontDefinition.FromFont(KpPuskumFont);
    }

    public Label PrimaryLabel => _primaryLabel;
    public Label SecondaryLabel => _secondaryLabel;
    public void SetHintVisible(bool visible)
    {
      _clickHint.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void ResetText()
    {
      _primaryLabel.text = string.Empty;
      _secondaryLabel.text = string.Empty;
      SetHintVisible(false);
    }

    static Label CreateHint()
    {
      var hint = new Label("Click to continue");
      hint.name = "dialogue-click-hint";
      hint.AddToClassList("dialogue-click-hint");
      hint.style.unityTextAlign = TextAnchor.MiddleCenter;
      hint.style.color = new Color(1f, 1f, 1f, 0.7f);
      hint.style.fontSize = 14;
      hint.style.marginTop = 18;
      hint.style.alignSelf = Align.Center;
      hint.style.display = DisplayStyle.None;
      return hint;
    }
  }
}

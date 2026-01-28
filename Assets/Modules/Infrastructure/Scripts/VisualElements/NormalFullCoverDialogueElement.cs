using Infrastructure.Definitions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.VisualElements
{
  [UxmlElement]
  public partial class NormalFullCoverDialogueElement : VisualElement
  {
    readonly Label _dialogueLabel;
    readonly Label _clickHint;

    public NormalFullCoverDialogueElement()
    {
      name = "normal-full-cover-dialogue";
      AddToClassList("normal-full-cover-dialogue");
      style.flexGrow = 1f;
      style.flexDirection = FlexDirection.Column;
      style.justifyContent = Justify.Center;
      style.alignItems = Align.Center;
      style.backgroundColor = new Color(Defaults.BASE_BACKGROUND_COLOR[0] / 255f, Defaults.BASE_BACKGROUND_COLOR[1] / 255f, Defaults.BASE_BACKGROUND_COLOR[2] / 255f);

      _dialogueLabel = new Label();
      _dialogueLabel.name = "dialogue-text";
      _dialogueLabel.AddToClassList("dialogue-text");
      _dialogueLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
      _dialogueLabel.style.color = Color.white;
      _dialogueLabel.style.fontSize = 26;
      _dialogueLabel.style.whiteSpace = WhiteSpace.Normal;
      _dialogueLabel.style.maxWidth = Length.Percent(80);
      Add(_dialogueLabel);

      _clickHint = new Label("Click to continue");
      _clickHint.name = "dialogue-click-hint";
      _clickHint.AddToClassList("dialogue-click-hint");
      _clickHint.style.unityTextAlign = TextAnchor.MiddleCenter;
      _clickHint.style.color = new Color(1f, 1f, 1f, 0.7f);
      _clickHint.style.fontSize = 14;
      _clickHint.style.marginTop = 20;
      _clickHint.style.display = DisplayStyle.None;
      Add(_clickHint);
    }

    public Label DialogueLabel => _dialogueLabel;
    public void SetHintVisible(bool visible)
    {
      _clickHint.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void ResetText()
    {
      _dialogueLabel.text = string.Empty;
      SetHintVisible(false);
    }
  }
}

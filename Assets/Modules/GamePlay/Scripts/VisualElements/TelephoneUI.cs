using UnityEngine.UIElements;

namespace GamePlay.VisualElements
{
  [UxmlElement]
  public partial class TelephoneUI : VisualElement
  {
    readonly Image _renderArea;

    public TelephoneUI()
    {
      name = "telephone-ui";
      AddToClassList("telephone-ui");
      style.flexDirection = FlexDirection.Column;

      _renderArea = new Image { name = "telephone-render" };
      _renderArea.AddToClassList("telephone-render");
      _renderArea.pickingMode = PickingMode.Position;
      Add(_renderArea);
    }

    public Image RenderArea => _renderArea;
  }
}

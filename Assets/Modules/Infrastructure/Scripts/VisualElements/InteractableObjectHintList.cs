using System.Collections.Generic;
using Infrastructure.Interactables;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.VisualElements
{
  [UxmlElement]
  public partial class InteractableObjectHintList : ScrollView
  {
    public InteractableObjectHintList()
        : base(ScrollViewMode.Vertical)
    {
      name = "interactable-scroll";
      AddToClassList("interactable-scroll");
      horizontalScrollerVisibility = ScrollerVisibility.Hidden;
      verticalScrollerVisibility = ScrollerVisibility.Auto;
      style.flexGrow = 1f;
      style.alignSelf = Align.Center;
      style.justifyContent = Justify.Center;
      style.alignItems = Align.Center;
      
      contentContainer.style.alignItems = Align.FlexStart;
      contentContainer.style.justifyContent = Justify.FlexStart;
    }

    public void Rebuild(
        IReadOnlyList<IInteractable> interactables,
        int selectedIndex,
        InteractableHintUIMode mode,
        string keyLabel,
        Sprite dialogueIcon)
    {
      contentContainer.Clear();

      if (interactables == null || interactables.Count == 0)
        return;

      for (int i = 0; i < interactables.Count; i++)
      {
        var element = new InteractableObjectHintListElement();
        element.Bind(interactables[i], keyLabel, mode, dialogueIcon, i == selectedIndex);
        contentContainer.Add(element);
      }

      ScrollToSelected(selectedIndex);
      ApplyStyles();
    }

    public void ScrollToSelected(int selectedIndex)
    {
      if (selectedIndex < 0 || selectedIndex >= contentContainer.childCount)
        return;

      var selectedElement = contentContainer[selectedIndex];
      if (selectedElement != null)
        ScrollTo(selectedElement);
    }

    const string UssSelectorContentViewport = "unity-content-viewport";
    VisualElement _visualElementContentViewport;
    VisualElement VisualElementContentViewport
    {
      get
      {
        if (_visualElementContentViewport == null)
        {
          _visualElementContentViewport = this.Q(name: UssSelectorContentViewport);
        }
        return _visualElementContentViewport;
      }
    }

    public void ApplyStyles()
    {
      VisualElementContentViewport.style.flexDirection = FlexDirection.Row;
      VisualElementContentViewport.style.alignItems = Align.Center;
      VisualElementContentViewport.style.paddingLeft = 300;
    }
  }
}

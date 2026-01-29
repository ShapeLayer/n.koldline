using System.Collections.Generic;
using Infrastructure.Interactables;
using Infrastructure.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.UIDocuments
{
  [RequireComponent(typeof(UIDocument))]
  public class InteractableUIController : MonoBehaviour
  {
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualElement _screenRoot;
    [SerializeField] private InteractableObjectHintList _hintList;

    [Header("Visuals")]
    [SerializeField] private Sprite _dialogueSelectionIcon;
    [SerializeField] private string _interactKeyLabel = "F";

    [Header("State")]
    [SerializeField] private InteractableHintUIMode _currentMode = InteractableHintUIMode.Default;
    [SerializeField] private int _selectedIndex = -1;
    [SerializeField] private List<IInteractable> _interactables = new List<IInteractable>();

    void Awake()
    {
      if (_uiDocument == null)
        _uiDocument = GetComponent<UIDocument>();

      CacheVisualReferences();
    }

    void OnEnable()
    {
      CacheVisualReferences();
      RefreshUI();
    }

    private void CacheVisualReferences()
    {
      if (_uiDocument == null)
        _uiDocument = GetComponent<UIDocument>();

      if (_uiDocument == null) return;

      var root = _uiDocument.rootVisualElement;
      if (root == null) return;

      _screenRoot = root.Q<VisualElement>("screen-root");
      _hintList = root.Q<InteractableObjectHintList>("interactable-scroll")
                  ?? root.Q<InteractableObjectHintList>();
    }

    public void UpdateInteractables(IReadOnlyList<IInteractable> interactables, int selectedIndex = -1)
    {
      if (_interactables == null) _interactables = new List<IInteractable>();

      _interactables.Clear();
      if (interactables != null)
      {
        for (int i = 0; i < interactables.Count; i++)
          _interactables.Add(interactables[i]);
      }

      if (_interactables.Count == 0)
      {
        _selectedIndex = -1;
      }
      else
      {
        _selectedIndex = selectedIndex < 0 ? 0 : Mathf.Clamp(selectedIndex, 0, _interactables.Count - 1);
      }

      RefreshUI();
    }

    private void RefreshUI()
    {
      if (_hintList == null) return;

      _hintList.Rebuild(_interactables, _selectedIndex, _currentMode, _interactKeyLabel, _dialogueSelectionIcon);

      if (_screenRoot != null)
      {
        _screenRoot.style.display = _interactables.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
      }
    }
  }
}

using UnityEngine;
using Infrastructure.Interactables;
using Infrastructure.Localization;
using GamePlay.UIDocuments;
using Infrastructure.UIStack;

namespace GamePlay.IngameObject
{
  public class TelephoneController : InteractableObjectABC
  {
    public const string L10N_KEY_DISPLAYNAME = "ITEM_TELEPHONE";
    public override string DisplayText => L10nCollections.Q(L10N_KEY_DISPLAYNAME);
    public override Sprite DisplayIcon => null;
    public override Color DisplayColor => Color.white;
    private bool _isInteractable = true;
    public override bool IsInteractable => _isInteractable;

    [Header("References")]
    [SerializeField] private TelephoneUIController _telephoneUIController;

    void Awake()
    {
      _telephoneUIController = FindFirstObjectByType<TelephoneUIController>();
    }

    public override void Interact(Transform interactor)
    {
      Debug.Log("TelephoneController Interact called");
      UIOverlayStackManager.Instance?.Push(_telephoneUIController);
    }
  }
}

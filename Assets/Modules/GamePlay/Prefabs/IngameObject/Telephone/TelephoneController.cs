using UnityEngine;
using Infrastructure.Interactables;
using Infrastructure.Localization;

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
    public override void Interact(Transform interactor)
    {
      Debug.Log("TelephoneController Interact called");
    }
  }
}

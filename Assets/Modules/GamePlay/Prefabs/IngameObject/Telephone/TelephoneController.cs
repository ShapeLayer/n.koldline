using UnityEngine;
using Infrastructure.Interactables;
using GamePlay.IntroPlay;

namespace GamePlay.IngameObject
{
  public class TelephoneController : InteractableObjectABC
  {
    public const string L10N_KEY_DISPLAYNAME = "ITEM_TELEPHONE";
    public override string DisplayText => L10nCollections.Q(L10N_KEY_DISPLAYNAME);
    public override Sprite DisplayIcon => null;
    public override Color DisplayColor => Color.white;
    public override void Interact(Transform interactor)
    {
      Debug.Log("TelephoneController Interact called");
    }
  }
}

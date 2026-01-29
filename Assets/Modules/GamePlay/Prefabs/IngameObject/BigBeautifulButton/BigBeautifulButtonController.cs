using UnityEngine;
using Infrastructure.Interactables;
using GamePlay.IntroPlay;

namespace GamePlay.IngameObject
{
  public class BigBeautifulButtonController : InteractableObjectABC
  {
    public const string L10N_KEY_DISPLAYNAME = "ITEM_BIG_BEAUTYFUL_BUTTON";
    public override string DisplayText => L10nCollections.Q(L10N_KEY_DISPLAYNAME);
    public override Sprite DisplayIcon => null;
    public override Color DisplayColor => Color.white;
    public override void Interact(Transform interactor)
    {
      Debug.Log("BigBeautifulButtonController Interact called");
    }
  }
}

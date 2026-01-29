using UnityEngine;
using Infrastructure.Interactables;
using Infrastructure.Localization;
using GamePlay.GameCompute;

namespace GamePlay.IngameObject
{
  public class BigBeautifulButtonController : InteractableObjectABC
  {
    public const string L10N_KEY_DISPLAYNAME = "ITEM_BIG_BEAUTYFUL_BUTTON";
    public override string DisplayText => L10nCollections.Q(L10N_KEY_DISPLAYNAME);
    public override Sprite DisplayIcon => null;
    public override Color DisplayColor => Color.white;
    private bool _isInteractable = true;
    public override bool IsInteractable => _isInteractable;

    [Header("References")]
    [SerializeField] private GameComputeManager _gameComputeManager;
    
    void Start()
    {
      _gameComputeManager = GameComputeManager.Instance;
    }

    public override void Interact(Transform interactor)
    {
      Debug.Log("BigBeautifulButtonController Interact called");

      // TODO: GameComputeManager 의존성 줄이기
      _gameComputeManager.InvokeScenarioStart();

      _isInteractable = false;
    }
  }
}

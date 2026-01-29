using UnityEngine;
using Infrastructure.Interactables;
using Infrastructure.Localization;
using GamePlay.GameCompute;

namespace GamePlay.IngameObject
{
  public class BigBeautifulButtonController : InteractableObjectABC
  {
    public const string L10N_KEY_DISPLAYNAME = "ITEM_BIG_BEAUTIFUL_BUTTON";
    public const string SPRITE_NAME = "BigBeautifulButtonIcon";
    public override string DisplayText => L10nCollections.Q(L10N_KEY_DISPLAYNAME);
    [SerializeField] private Sprite _displayIcon;
    public override Sprite DisplayIcon => _displayIcon;
    public override Color DisplayColor => Color.white;
    private bool _isInteractable = true;
    public override bool IsInteractable => _isInteractable;

    [Header("References")]
    [SerializeField] private GameComputeManager _gameComputeManager;
    
    void Awake()
    {
      _displayIcon = Resources.Load<Sprite>($"Textures/{SPRITE_NAME}");
    }

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

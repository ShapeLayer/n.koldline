using GamePlay.Definitions;
using GamePlay.IngameObject;
using GamePlay.UIDocuments;
using GamePlay.VisualElements;
using GamePlay.WorldMapPane;
using Infrastructure.Camera;
using Infrastructure.Player;
using UnityEngine;

namespace GamePlay.GameCompute
{
  public partial class GameComputeManager
  {
    [Header("Resources")]
    [SerializeField] AudioClip _audioNuclearButtonPressed;
    [SerializeField] AudioClip _audioIntroStab;
    [SerializeField] AudioClip _audioTimerSet;
    [SerializeField] AudioClip _audioTimerTickTock;
    [SerializeField] AudioClip _audioButtonPressed;
    
    [Header("References")]
    [SerializeField] MainCameraController _mainCameraController;
    [SerializeField] WorldMapPaneController _worldMapPaneController;
    [SerializeField] TelephoneUIController _telephoneUIController;

    void Awake_Loader()
    {
      LoadResources();
    }

    void Start_Loader()
    {
      LoadSingletons();
      RegisterUIEvents();
    }

    void LoadResources()
    {
      _worldMapPaneController = FindFirstObjectByType<WorldMapPaneController>();
      _telephoneUIController = FindFirstObjectByType<TelephoneUIController>();
      _audioNuclearButtonPressed = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_NUCLEAR_BUTTON_PRESSED}");
      _audioIntroStab = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_INTRO_STAB}");
      _audioTimerSet = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_TIMER_SET}");
      _audioTimerTickTock = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_TIMER_TICK_TOCK}");
      _audioButtonPressed = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_BUTTON_PRESSED}");
    }

    void LoadSingletons()
    {
      _mainCameraController = MainCameraController.Instance;
    }

    void RegisterUIEvents()
    {
      if (_telephoneUIController == null)
        return;

      _telephoneUIController.ButtonClicked += OnTelephoneButtonClicked;
    }

    bool ValidateRequiredOnScenarioStart()
    {
      if (_audioNuclearButtonPressed == null) return false;
      if (_audioIntroStab == null) return false;
      if (_audioTimerSet == null) return false;
      if (_audioTimerTickTock == null) return false;
      if (_audioButtonPressed == null) return false;
      if (_mainCameraController == null) return false;
      if (_worldMapPaneController == null) return false;
      if (_telephoneUIController == null) return false;
      return true;
    }


    void OnTelephoneButtonClicked(TelephoneButtonType buttonType)
    {
      _mainCameraController.PlayAudioOneShot(_audioButtonPressed);
#if UNITY_EDITOR
      Debug.Log($"[GameComputeManager] Telephone button clicked: {buttonType}");
#endif
    }
  }
}

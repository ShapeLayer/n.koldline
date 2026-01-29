using GamePlay.Definitions;
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
    
    [Header("References")]
    [SerializeField] MainCameraController _mainCameraController;

    void Awake_Loader()
    {
      LoadResources();
    }

    void Start_Loader()
    {
      LoadSingletons();
    }

    void LoadResources()
    {
      _audioNuclearButtonPressed = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_NUCLEAR_BUTTON_PRESSED}");
      _audioIntroStab = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_INTRO_STAB}");
      _audioTimerSet = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_TIMER_SET}");
    }

    void LoadSingletons()
    {
      _mainCameraController = MainCameraController.Instance;
    }

    bool ValidateRequiredOnScenarioStart()
    {
      if (_audioNuclearButtonPressed == null) return false;
      if (_audioIntroStab == null) return false;
      if (_mainCameraController == null) return false;
      return true;
    }
  }
}

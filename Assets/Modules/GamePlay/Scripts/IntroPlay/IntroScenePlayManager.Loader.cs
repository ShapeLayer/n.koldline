using UnityEngine;
using GamePlay.Definitions;
using Infrastructure.Exceptions;
using Infrastructure.UIDocuments;

namespace GamePlay.IntroPlay
{
  public partial class IntroScenePlayManager : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] NormalFullCoverDialogueController _normalFullCoverDialogueController;
    [SerializeField] KpDialogueController _kpDialogueController;
    [SerializeField] LocaleSelectorUIController _localeSelector;
    [SerializeField] IntroSceneCameraController _introSceneCameraController;
    [SerializeField] AudioClip _audioClipGeneralUsesTeleport;

    void Awake_Loader()
    {
      _normalFullCoverDialogueController = FindFirstObjectByType<NormalFullCoverDialogueController>();
      if (_normalFullCoverDialogueController == null) throw new InitializingNotSucceed("NormalFullCoverDialogueController not found");

      _localeSelector = FindFirstObjectByType<LocaleSelectorUIController>();
      if (_localeSelector == null) throw new InitializingNotSucceed("LocaleSelectorUIController not found");

      _kpDialogueController = FindFirstObjectByType<KpDialogueController>();
      if (_kpDialogueController == null) throw new InitializingNotSucceed("KpDialogueController not found");

      _introSceneCameraController = FindFirstObjectByType<IntroSceneCameraController>();
      if (_introSceneCameraController == null) throw new InitializingNotSucceed("IntroSceneCameraController not found");

      _audioClipGeneralUsesTeleport = Resources.Load<AudioClip>($"Musics/{Defaults.AUDIO_CLIP_GENERAL_USES_TELEPORT}");
      if (_audioClipGeneralUsesTeleport == null) throw new InitializingNotSucceed("AudioClip GeneralUsesTeleport not found");
    }
  }
}

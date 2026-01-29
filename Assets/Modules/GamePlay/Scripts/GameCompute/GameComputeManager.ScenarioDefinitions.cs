using System;
using System.Threading.Tasks;
using Infrastructure.Localization;
using Infrastructure.UIDocuments;
using UnityEngine;

namespace GamePlay.GameCompute
{
  public partial class GameComputeManager : MonoBehaviour
  {
    TitleOverlayController _title => TitleOverlayController.Instance;

    public async Task InvokeScenarioStart()
    {
      // AudioSource 등은 Camera에게 있어야 하므로 이를 검증
      if (!ValidateRequiredOnScenarioStart())
      {
#if UNITY_EDITOR
        Debug.LogError("GameComputeManager: Required references are not set before starting scenario.");
#endif
        return;
      }

      await _mainCameraController.PlayAudioOneShotAsync(_audioNuclearButtonPressed);
      await Task.Delay(500);

      DateTime localDateTime = DateTime.Now;
      int year = localDateTime.Year;
      int kpYear = year - 1911;
      await Task.WhenAll(
        _mainCameraController.PlayAudioOneShotAsync(_audioIntroStab),
        _title.InstantShowAsync(string.Format(
          L10nCollections.Q(L10N_KEY_GAME_STARTING_KP_1), kpYear, year),
        3f)
      );

      await Task.Delay(50);
      await Task.WhenAll(
        _mainCameraController.PlayAudioOneShotAsync(_audioIntroStab),
        _title.InstantShowAsync(L10nCollections.Q(L10N_KEY_GAME_STARTING_KP_2), 3f)
      );
      await Task.WhenAll(
        _mainCameraController.PlayAudioOneShotAsync(_audioIntroStab),
        _title.InstantShowAsync(L10nCollections.Q(L10N_KEY_GAME_STARTING_KP_3), 3f)
      );
      PlaySoundDelayAfter(_audioTimerSet, 1f);
      // Show Timer
      await Task.WhenAll(
        _mainCameraController.PlayAudioOneShotAsync(_audioIntroStab),
        _title.InstantShowAsync(L10nCollections.Q(L10N_KEY_GAME_STARTING_KP_4), 3f)
      );
      await Task.WhenAll(
        _mainCameraController.PlayAudioOneShotAsync(_audioIntroStab),
        _title.InstantShowAsync(L10nCollections.Q(L10N_KEY_GAME_STARTING_KP_5), 5f)
      );
      await Task.WhenAll(
        _mainCameraController.PlayAudioOneShotAsync(_audioIntroStab),
        _title.InstantShowAsync(L10nCollections.Q(L10N_KEY_GAME_STARTING_KP_6), 3f)
      );
      // Start Timer
    }
    async Task PlaySoundDelayAfter(AudioClip audioClip, float delaySeconds)
    {
      if (delaySeconds > 0f)
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

      _mainCameraController.PlayAudioOneShot(audioClip);
    }

  }
}

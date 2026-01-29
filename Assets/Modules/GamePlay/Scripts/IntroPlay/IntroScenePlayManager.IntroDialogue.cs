using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Infrastructure.Localization
{
  public partial class IntroScenePlayManager : MonoBehaviour
  {
    /**
     * 플레이 순서
     * 주의사항 > KP 인트로 
     */
    
    const string KEY_INTRO_NOTIFY_KP_WARNING = "GAME_INTRO_NOTIFY_KP_WARNING";
    const string KEY_GAME_INTRO_KP_1 = "GAME_INTRO_KP_1";
    const string KEY_GAME_INTRO_KP_2 = "GAME_INTRO_KP_2";
    const string KEY_GAME_INTRO_KP_2_1 = "GAME_INTRO_KP_2_1";
    const string KEY_GAME_INTRO_KP_2_2 = "GAME_INTRO_KP_2_2";
    const string KEY_GAME_INTRO_KP_3 = "GAME_INTRO_KP_3";
    const string KEY_GAME_INTRO_KP_4 = "GAME_INTRO_KP_4";
    
    const int INGAME_SCENE_BUILD_INDEX = 1;

    bool _isPlayingIntro;
    enum ActiveDialogue
    {
      None,
      Normal,
      Kp
    }

    ActiveDialogue _activeDialogue = ActiveDialogue.None;

    async Task PlayIntro()
    {
      if (_isPlayingIntro) return;
      _isPlayingIntro = true;

      try
      {
        _activeDialogue = ActiveDialogue.Normal;
        await _normalFullCoverDialogueController.Show();
        await _normalFullCoverDialogueController.PlayDialogue(
          L10nCollections.Q(KEY_INTRO_NOTIFY_KP_WARNING)
        );
        _introSceneCameraController.PlayAudio(_audioClipGeneralUsesTeleport);
        await _normalFullCoverDialogueController.Hide();
        _activeDialogue = ActiveDialogue.None;

        _activeDialogue = ActiveDialogue.Kp;
        await _kpDialogueController.Show();
        // DateTime localDateTime = DateTime.Now;
        // int year = localDateTime.Year;
        // int kpYear = year - 1911;
        // await _normalFullCoverDialogueController.PlayDialogue(string.Format(StringAssetBaseKP.GAME_INTRO_KP_1, year, kpYear));
        bool isLocaleKR = LocalizationSettings.SelectedLocale.Identifier.Code.Contains("ko");
        await _kpDialogueController.PlayDialogue(StringAssetBaseKP.GAME_INTRO_KP_1, !isLocaleKR ? L10nCollections.Q(KEY_GAME_INTRO_KP_1) : "");
        await _kpDialogueController.PlayDialogue(StringAssetBaseKP.GAME_INTRO_KP_2_1, !isLocaleKR ? L10nCollections.Q(KEY_GAME_INTRO_KP_2_1) : "");
        await _kpDialogueController.PlayDialogue(StringAssetBaseKP.GAME_INTRO_KP_2_2, !isLocaleKR ? L10nCollections.Q(KEY_GAME_INTRO_KP_2_2) : "", presetPrimary: StringAssetBaseKP.GAME_INTRO_KP_2_1, presetSecondary: L10nCollections.Q(KEY_GAME_INTRO_KP_2_1));
        await _kpDialogueController.PlayDialogue(StringAssetBaseKP.GAME_INTRO_KP_3, !isLocaleKR ? L10nCollections.Q(KEY_GAME_INTRO_KP_3) : "");
        await _kpDialogueController.PlayDialogue(StringAssetBaseKP.GAME_INTRO_KP_4, !isLocaleKR ? L10nCollections.Q(KEY_GAME_INTRO_KP_4) : "");
        _introSceneCameraController.StopAudio();
        await _kpDialogueController.Hide();
        _activeDialogue = ActiveDialogue.None;
        // await _normalFullCoverDialogueController.PlayDialogue(StringAssetBaseKP.GAME_INTRO_KP_5);
      }
      finally
      {
        _activeDialogue = ActiveDialogue.None;
        _isPlayingIntro = false;
      }

      SceneManager.LoadScene(INGAME_SCENE_BUILD_INDEX, LoadSceneMode.Single);
    }
  }
}

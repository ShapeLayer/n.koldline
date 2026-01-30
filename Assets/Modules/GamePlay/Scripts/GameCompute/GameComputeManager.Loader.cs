using System.Collections.Generic;
using GamePlay.CallingInteraction;
using GamePlay.Definitions;
using GamePlay.IngameObject;
using GamePlay.UIDocuments;
using GamePlay.VisualElements;
using GamePlay.WorldMapPane;
using Infrastructure.Camera;
using Infrastructure.Commons;
using Infrastructure.Player;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

using CID = GamePlay.CallingInteraction.CallingInteractionDefinitions;

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
    Dictionary<string, AudioClip[]> _audioARSClips = new Dictionary<string, AudioClip[]>();
    public const string AUDIO_CLIPS_NAVIGATION_HINT_KEY = "NAVIGATION_HINT";
    
    [Header("References")]
    [SerializeField] MainCameraController _mainCameraController;
    [SerializeField] WorldMapPaneController _worldMapPaneController;
    [SerializeField] TelephoneController _telephoneController;
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
      _telephoneController = FindFirstObjectByType<TelephoneController>();
      _telephoneUIController = FindFirstObjectByType<TelephoneUIController>();
      _audioNuclearButtonPressed = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_NUCLEAR_BUTTON_PRESSED}");
      _audioIntroStab = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_INTRO_STAB}");
      _audioTimerSet = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_TIMER_SET}");
      _audioTimerTickTock = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_TIMER_TICK_TOCK}");
      _audioButtonPressed = Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_SOUND}/{Defaults.AUDIO_CLIP_BUTTON_PRESSED}");
      LoadARSResources();
    }

    void LoadARSResources()
    {
      _audioARSClips.Clear();
      string localeCode = LocalizationUtils.GetLanguageCode(LocalizationSettings.SelectedLocale);
      _audioARSClips.Add(AUDIO_CLIPS_NAVIGATION_HINT_KEY, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{AUDIO_CLIPS_NAVIGATION_HINT_KEY}") });
      _audioARSClips.Add(CID.CALLING_START.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.CALLING_START.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATIONALITY_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATIONALITY_REQ.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_ENTRY.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_ENTRY.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P1.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P1.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P2.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P2.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P3.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P3.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P5.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P5.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P6.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P6.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P7.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P7.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P8.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P8.AudioL10NKey}") });
      _audioARSClips.Add(CID.NATN_CODE_P9.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_CODE_P9.AudioL10NKey}") });
      
      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.NATN_SELECTED.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_SELECTED.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_SELECTED.AudioL10NKey}_S2")
          });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.NATN_SELECTED.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_SELECTED.AudioL10NKey}_S1") });
          break;
        default:
          _audioARSClips.Add(CID.NATN_SELECTED.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_SELECTED.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NATN_SELECTED.AudioL10NKey}_S2")
          });
          break;
      }

      _audioARSClips.Add(CID.IDENTITY_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_REQ.AudioL10NKey}") });
      
      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.IDENTITY_SELECTED.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S2"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S3"),
          });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.IDENTITY_SELECTED.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.IDENTITY_SELECTED.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S2"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.IDENTITY_SELECTED.AudioL10NKey}_S3"),
          });
          break;
      }

      _audioARSClips.Add(CID.TARGET_SETTING.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.TARGET_SETTING.AudioL10NKey}") });
      _audioARSClips.Add(CID.HOTLINE_AUTH_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_AUTH_REQ.AudioL10NKey}") });
      _audioARSClips.Add(CID.HOTLINE_AUTH_FAIL.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_AUTH_FAIL.AudioL10NKey}") });
      _audioARSClips.Add(CID.HOTLINE_AUTH_SUCC.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_AUTH_SUCC.AudioL10NKey}") });
      _audioARSClips.Add(CID.HOTLINE_ABSENCE.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_ABSENCE.AudioL10NKey}") });
      
      switch (localeCode) {
        case "en":
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.HOTLINE_RESERVE.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_RESERVE.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_RESERVE.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.HOTLINE_RESERVE.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_RESERVE.AudioL10NKey}") });
        break;
      }

      _audioARSClips.Add(CID.HOTLINE_EMERGENCY.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.HOTLINE_EMERGENCY.AudioL10NKey}") });
      _audioARSClips.Add(CID.MILITARY_RISK.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MILITARY_RISK.AudioL10NKey}") });
      _audioARSClips.Add(CID.NUCLEAR_EMERGENCY.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NUCLEAR_EMERGENCY.AudioL10NKey}") });
      _audioARSClips.Add(CID.INTERCEPT_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_REQ.AudioL10NKey}") });
      
      _audioARSClips.Add(CID.MIL_CODE_ENTRY.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_CODE_ENTRY.AudioL10NKey}") });
      switch (localeCode) {
        case "en":
        case "ja":
        case "ko":
        default:
          _audioARSClips.Add(CID.MIL_CODE_CONFIRM_1.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_CODE_CONFIRM_1.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_CODE_CONFIRM_1.AudioL10NKey}_S2"),
          });  
          break;
      }

      _audioARSClips.Add(CID.MIL_CODE_CONFIRM_2.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_CODE_CONFIRM_2.AudioL10NKey}") });
      _audioARSClips.Add(CID.MIL_CODE_CANCEL.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_CODE_CANCEL.AudioL10NKey}") });
      _audioARSClips.Add(CID.MIL_CODE_FAIL.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_CODE_FAIL.AudioL10NKey}") });
      _audioARSClips.Add(CID.MIL_ACTION_START.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MIL_ACTION_START.AudioL10NKey}") });
      switch (localeCode) {
        case "en":
        case "ja":
        case "ko":
        default:
          _audioARSClips.Add(CID.OPERATOR_CONNECT.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.OPERATOR_CONNECT.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.OPERATOR_CONNECT.AudioL10NKey}_S2"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.OPERATOR_CONNECT.AudioL10NKey}_S3"),
          });
          break;
      }
      _audioARSClips.Add(CID.TRAFFIC_OVERLOAD.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.TRAFFIC_OVERLOAD.AudioL10NKey}") });
      _audioARSClips.Add(CID.MISC_INQUIRY.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MISC_INQUIRY.AudioL10NKey}") });
      _audioARSClips.Add(CID.NUCLEAR_PROG.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NUCLEAR_PROG.AudioL10NKey}") });
      _audioARSClips.Add(CID.NUCLEAR_LEARN.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.NUCLEAR_LEARN.AudioL10NKey}") });
      _audioARSClips.Add(CID.MISSILE_LIST_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MISSILE_LIST_REQ.AudioL10NKey}") });
      _audioARSClips.Add(CID.MISSILE_LIST_NK.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.MISSILE_LIST_NK.AudioL10NKey}") });
      _audioARSClips.Add(CID.US_WEAPONS.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.US_WEAPONS.AudioL10NKey}") });
      _audioARSClips.Add(CID.DEFENSE_WEAPONS.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.DEFENSE_WEAPONS.AudioL10NKey}") });
      _audioARSClips.Add(CID.POWER_CODE_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_REQ.AudioL10NKey}") });
      
      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.POWER_CODE_LOW.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_LOW.AudioL10NKey}_S1"),
          });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.POWER_CODE_LOW.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_LOW.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_LOW.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.POWER_CODE_LOW.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_LOW.AudioL10NKey}_S1"),
          });
          break;
      }
      
      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.POWER_CODE_HIGH.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_HIGH.AudioL10NKey}_S1") });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.POWER_CODE_HIGH.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_HIGH.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_HIGH.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.POWER_CODE_HIGH.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.POWER_CODE_HIGH.AudioL10NKey}_S1") });
          break;
      }
      
      _audioARSClips.Add(CID.INTERCEPT_METHOD.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_METHOD.AudioL10NKey}") });
      
      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.INTERCEPT_CODE_X0.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_X0.AudioL10NKey}_S1") });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.INTERCEPT_CODE_X0.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_X0.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_X0.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.INTERCEPT_CODE_X0.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_X0.AudioL10NKey}_S1") });
          break;
      }

      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.INTERCEPT_CODE_Y0.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_Y0.AudioL10NKey}_S1") });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.INTERCEPT_CODE_Y0.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_Y0.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_Y0.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.INTERCEPT_CODE_Y0.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.INTERCEPT_CODE_Y0.AudioL10NKey}_S1") });
          break;
      }

      _audioARSClips.Add(CID.CODE_LOST_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.CODE_LOST_REQ.AudioL10NKey}") });
      
      switch (localeCode) {
        case "en":
        case "ja":
        case "ko":
        default:
          _audioARSClips.Add(CID.BOT_CHECK_REQ.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.BOT_CHECK_REQ.AudioL10NKey}_S1") });
        break;
      }

      _audioARSClips.Add(CID.BOT_CHECK_FAIL.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.BOT_CHECK_FAIL.AudioL10NKey}") });
      switch (localeCode) {
        case "en":
          _audioARSClips.Add(CID.CODE_ISSUE_SUCC.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.CODE_ISSUE_SUCC.AudioL10NKey}_S1") });
          break;
        case "ja":
        case "ko":
          _audioARSClips.Add(CID.CODE_ISSUE_SUCC.AudioL10NKey, new AudioClip[] {
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.CODE_ISSUE_SUCC.AudioL10NKey}_S1"),
            Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.CODE_ISSUE_SUCC.AudioL10NKey}_S2"),
          });
          break;
        default:
          _audioARSClips.Add(CID.CODE_ISSUE_SUCC.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.CODE_ISSUE_SUCC.AudioL10NKey}_S1") });
          break;
      }
      _audioARSClips.Add(CID.ERROR_404.AudioL10NKey, new AudioClip[] { Resources.Load<AudioClip>($"{Defaults.PREFIX_AUDIO_CLIP_VOICE}/{localeCode}/{CID.ERROR_404.AudioL10NKey}") });
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
      if (_telephoneController == null) return false;
      return true;
    }


    void OnTelephoneButtonClicked(TelephoneButtonType buttonType)
    {
      _mainCameraController.PlayAudioOneShot(_audioButtonPressed);
#if UNITY_EDITOR
      Debug.Log($"[GameComputeManager] Telephone button clicked: {buttonType}");
#endif
      OnTelephoneButtonClickedTransfer(buttonType);
    }
  }
}

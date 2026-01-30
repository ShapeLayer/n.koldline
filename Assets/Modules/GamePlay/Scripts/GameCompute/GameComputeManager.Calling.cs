using System;
using System.Linq;
using GamePlay.CallingInteraction;
using Infrastructure.Commons;
using UnityEngine;
using UnityEngine.Localization.Settings;
using CID = GamePlay.CallingInteraction.CallingInteractionDefinitions;

namespace GamePlay.GameCompute
{
  public partial class GameComputeManager
  {
    [SerializeField] ResponseNode _currentNode;
    [SerializeField] AudioClip[] _currentQueuedARSClips;

    [SerializeField] string _localeCode;           // 현재 로케일 코드
    [SerializeField] string _selectedNationId;     // 국가 ID
    [SerializeField] string _selectedPersonalRank; // 신분 등급 ID
    [SerializeField] string _selectedActionDescId; // 군사 행위 설명 ID
    [SerializeField] DateTime _reservedDateTime;   // 예약된 일시
    [SerializeField] int _waitNum;                 // 대기 번호
    [SerializeField] int _waitMin;                 // 대기 시간
    [SerializeField] string _powerCodeLowPart;     // 저속 탄도 미사일 앞 2자리
    [SerializeField] string _powerCodeHighPart;    // 고속 탄도 미사일 앞 2자리
    [SerializeField] string _interceptCodeX0Part;  // X0 방식 뒤 2자리
    [SerializeField] string _interceptCodeY0Part;  // Y0 방식 뒤 2자리      // 권한 코드 부분 (앞/뒤 두자리)
    [SerializeField] string _botCode;              // 로봇 확인용 코드
    [SerializeField] string _newIssuedCode;

    void PrepareInstanceVariable()
    {
      _waitNum = UnityEngine.Random.Range(1, 30);
      _waitMin = UnityEngine.Random.Range(10, 30);
    }

    void ReadyCurrentARS()
    {
      _localeCode = LocalizationUtils.GetLanguageCode(LocalizationSettings.SelectedLocale);

      if (_currentNode.IsContentFormattingRequired)
        ReadyCurrentARSFormatting();
      else
        _currentQueuedARSClips = _audioARSClips[_currentNode.AudioL10NKey];
      
      if (_currentNode.SuppressNavigationHint == false)
        _currentQueuedARSClips = _currentQueuedARSClips.Append(_audioARSClips[AUDIO_CLIPS_NAVIGATION_HINT_KEY][0]).ToArray();
    }

    void ProcessCurrentARSResponse
    (
      NodeTransferConditionType conditionType,
      TelephoneButtonType[] pressedButtons = null
    )
    {
    }

    void ReadyCurrentARSFormatting()
    {
      // 1. 국가 선택
      if (_currentNode.Id == CID.NATN_SELECTED.Id)
      {
        switch (_localeCode)
        {
          case "ja":
          case "ko":
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[_selectedNationId][0],
                            _audioARSClips[CID.NATN_SELECTED.AudioL10NKey + "_S1"][0],
                        };
            break;
          default: // en 포함
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[CID.NATN_SELECTED.AudioL10NKey + "_S1"][0],
                            _audioARSClips[_selectedNationId][0],
                            _audioARSClips[CID.NATN_SELECTED.AudioL10NKey + "_S2"][0],
                        };
            break;
        }
      }
      // 2. 신분 선택
      else if (_currentNode.Id == CID.IDENTITY_SELECTED.Id)
      {
        switch (_localeCode)
        {
          case "ja":
          case "ko":
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[_selectedNationId][0],
                            _audioARSClips[CID.IDENTITY_SELECTED.AudioL10NKey + "_S1"][0],
                            _audioARSClips[_selectedPersonalRank][0],
                            _audioARSClips[CID.IDENTITY_SELECTED.AudioL10NKey + "_S2"][0],
                        };
            break;
          default: // en 포함
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[CID.IDENTITY_SELECTED.AudioL10NKey + "_S1"][0],
                            _audioARSClips[_selectedNationId][0],
                            _audioARSClips[CID.IDENTITY_SELECTED.AudioL10NKey + "_S2"][0],
                            _audioARSClips[_selectedPersonalRank][0],
                            _audioARSClips[CID.IDENTITY_SELECTED.AudioL10NKey + "_S3"][0],
                        };
            break;
        }
      }
      // 3. 예약 확인
      else if (_currentNode.Id == CID.HOTLINE_RESERVE.Id)
      {
        var now = DateTime.Now;
        if (_reservedDateTime == null || _reservedDateTime <= now)
          _reservedDateTime = now + TimeSpan.FromMinutes(10) + TimeSpan.FromMinutes(UnityEngine.Random.Range(0, 20));

        switch (_localeCode)
        {
          case "ja":
          case "ko":
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[CID.HOTLINE_RESERVE.AudioL10NKey + "_S1"][0],
                            _audioARSClips[$"NUM_{now.Hour}"][0], _audioARSClips["O_CLOCK"][0],
                            _audioARSClips[$"NUM_{now.Minute}"][0], _audioARSClips["MINUTE"][0],
                            _audioARSClips[$"NUM_{_reservedDateTime.Hour}"][0], _audioARSClips["O_CLOCK"][0],
                            _audioARSClips[$"NUM_{_reservedDateTime.Minute}"][0], _audioARSClips["MINUTE"][0],
                            _audioARSClips[CID.HOTLINE_RESERVE.AudioL10NKey + "_S2"][0],
                        };
            break;
          default:
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[CID.HOTLINE_RESERVE.AudioL10NKey + "_S1"][0],
                            _audioARSClips[$"NUM_{now.Hour}"][0], _audioARSClips[$"NUM_{now.Minute}"][0], _audioARSClips["MINUTE"][0],
                            _audioARSClips[CID.HOTLINE_RESERVE.AudioL10NKey + "_S2"][0],
                            _audioARSClips[$"NUM_{_reservedDateTime.Hour}"][0], _audioARSClips[$"NUM_{_reservedDateTime.Minute}"][0], _audioARSClips["MINUTE"][0],
                        };
            break;
        }
      }
            // 4. 저속 탄도 미사일 코드 안내
            else if (_currentNode.Id == CID.POWER_CODE_LOW.Id)
            {
                AssembleCodeClips(CID.POWER_CODE_LOW.AudioL10NKey, _powerCodeLowPart);
            }
            // 5. 고속 탄도 미사일 코드 안내
            else if (_currentNode.Id == CID.POWER_CODE_HIGH.Id)
            {
                AssembleCodeClips(CID.POWER_CODE_HIGH.AudioL10NKey, _powerCodeHighPart);
            }
            // 6. X0 요격 방식 코드 안내
            else if (_currentNode.Id == CID.INTERCEPT_CODE_X0.Id)
            {
                AssembleCodeClips(CID.INTERCEPT_CODE_X0.AudioL10NKey, _interceptCodeX0Part);
            }
            // 7. Y0 요격 방식 코드 안내
            else if (_currentNode.Id == CID.INTERCEPT_CODE_Y0.Id)
            {
                AssembleCodeClips(CID.INTERCEPT_CODE_Y0.AudioL10NKey, _interceptCodeY0Part);
            }
            // 8. 군사 코드 확인
            else if (_currentNode.Id == CID.MIL_CODE_CONFIRM_1.Id)
            {
                _currentQueuedARSClips = new AudioClip[] {
                    _audioARSClips[CID.MIL_CODE_CONFIRM_1.AudioL10NKey + "_S1"][0],
                    _audioARSClips[_selectedActionDescId][0],
                    _audioARSClips[CID.MIL_CODE_CONFIRM_1.AudioL10NKey + "_S2"][0],
                };
            }
            // 9. 상담원 연결 정보
            else if (_currentNode.Id == CID.OPERATOR_CONNECT.Id)
            {
                _currentQueuedARSClips = new AudioClip[] {
                    _audioARSClips[CID.OPERATOR_CONNECT.AudioL10NKey + "_S1"][0],
                    _audioARSClips[$"NUM_{_waitNum}"][0],
                    _audioARSClips[CID.OPERATOR_CONNECT.AudioL10NKey + "_S2"][0],
                    _audioARSClips[$"NUM_{_waitMin}"][0],
                    _audioARSClips[CID.OPERATOR_CONNECT.AudioL10NKey + "_S3"][0],
                };
            }
            // 10. 봇 확인
            else if (_currentNode.Id == CID.BOT_CHECK_REQ.Id)
            {
                _currentQueuedARSClips = new AudioClip[] {
                    _audioARSClips[CID.BOT_CHECK_REQ.AudioL10NKey + "_S1"][0],
                    _audioARSClips[$"NUM_{_botCode}"][0],
                };
            }
            // 11. 코드 발급 완료
            else if (_currentNode.Id == CID.CODE_ISSUE_SUCC.Id)
            {
              int code = UnityEngine.Random.Range(0, 9999);
              _newIssuedCode = code.ToString("D4");
              _currentQueuedARSClips = new AudioClip[] {
                _audioARSClips[CID.CODE_ISSUE_SUCC.AudioL10NKey + "_S1"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[0]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[1]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[2]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[3]}"][0],
              };
            }
        }

        void AssembleCodeClips(string baseKey, string codeValue)
        {
            if (_localeCode == "en")
            {
                _currentQueuedARSClips = new AudioClip[] {
                    _audioARSClips[baseKey + "_S1"][0],
                    _audioARSClips[$"NUM_{codeValue}"][0],
                };
            }
            else
            {
                _currentQueuedARSClips = new AudioClip[] {
                    _audioARSClips[baseKey + "_S1"][0],
                    _audioARSClips[$"NUM_{codeValue}"][0],
                    _audioARSClips[baseKey + "_S2"][0],
                };
            }
        }
  }
}

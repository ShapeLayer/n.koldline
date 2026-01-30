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
    void PrepareInstanceVariable_Specific()
    {
      _waitNum = UnityEngine.Random.Range(1, 30);
      _waitMin = UnityEngine.Random.Range(10, 30);
      _powerCodeLowPart = UnityEngine.Random.Range(10, 100).ToString("00");
      do
      {
        _powerCodeHighPart = UnityEngine.Random.Range(10, 100).ToString("00");
      } while (_powerCodeHighPart == _powerCodeLowPart);

      _interceptCodeX0Part = UnityEngine.Random.Range(10, 100).ToString("00");
      do
      {
        _interceptCodeY0Part = UnityEngine.Random.Range(10, 100).ToString("00");
      } while (_interceptCodeY0Part == _interceptCodeX0Part);

      _botCode = UnityEngine.Random.Range(0, 10000).ToString("0000");
    }

    bool SequenceEquals(TelephoneButtonType[] a, TelephoneButtonType[] b)
    {
      if (a == null && b == null)
        return true;
      if (a == null || b == null)
        return false;
      if (a.Length != b.Length)
        return false;
      for (int i = 0; i < a.Length; i++)
      {
        if (a[i] != b[i])
          return false;
      }
      return true;
    }


    /// <summary>
    /// 현재 ARS 노드에 대한 응답 처리
    /// </summary>
    bool ProcessCurrentSpecialARSResponse
    (
      NodeTransferConditionType conditionType,
      TelephoneButtonType[] pressedButtons = null
    )
    {
      if (pressedButtons == null)
        pressedButtons = Array.Empty<TelephoneButtonType>();

      if (_currentNode.Id == CID.NATIONALITY_REQ.Id)
      {
        if (conditionType == NodeTransferConditionType.SinglePressed && pressedButtons.Length > 0)
        {
          if (pressedButtons[0] == TelephoneButtonType.Star)
          {
            _nextNode = CID.NATN_CODE_ENTRY;
            return true;
          }
        }
        else if (conditionType == NodeTransferConditionType.NumberSequence)
        {
          var nationKey = GetNationKeyFromSequence(pressedButtons);
          if (string.IsNullOrEmpty(nationKey) == false)
          {
            _selectedNationId = nationKey;
            _nextNode = CID.NATN_SELECTED;
            return true;
          }
        }
      }
      else if (IsNationalityInfoNode(_currentNode.Id))
      {
        if (conditionType == NodeTransferConditionType.SinglePressed && pressedButtons.Length > 0)
        {
          if (pressedButtons[0] == TelephoneButtonType.Number9)
          {
            _nextNode = _currentNode;
            return true;
          }
        }
      }
      else if (_currentNode.Id == CID.IDENTITY_REQ.Id)
      {
        if (conditionType == NodeTransferConditionType.SinglePressed && pressedButtons.Length > 0)
        {
          if (TrySetIdentityRank(pressedButtons[0]))
          {
            _nextNode = CID.IDENTITY_SELECTED;
            return true;
          }
        }
      }
      else if (_currentNode.Id == CID.HOTLINE_AUTH_REQ.Id)
      {
        if (conditionType == NodeTransferConditionType.NumberSequence)
        {
          if (_newIssuedCode != null && SequenceEquals(pressedButtons, _newIssuedCode))
          {
            _nextNode = CID.HOTLINE_AUTH_SUCC;
          }
          else
          {
            _nextNode = CID.HOTLINE_AUTH_FAIL;
          }
          return true;
        }
      }
      else if (_currentNode.Id == CID.MIL_CODE_ENTRY.Id)
      {
        if (conditionType == NodeTransferConditionType.NumberSequence)
        {
          if (TryResolveMilitaryCode(pressedButtons, out var actionDescKey))
          {
            _selectedActionDescId = actionDescKey;
            _nextNode = CID.MIL_CODE_CONFIRM_1;
          }
          else
          {
            _nextNode = CID.MIL_CODE_FAIL;
          }
          return true;
        }
      }
      else if (_currentNode.Id == CID.BOT_CHECK_REQ.Id)
      {
        if (conditionType == NodeTransferConditionType.NumberSequence)
        {
          var inputDigits = ToDigitString(pressedButtons);
          if (string.IsNullOrEmpty(inputDigits) == false && inputDigits == _botCode)
          {
            _nextNode = CID.CODE_ISSUE_SUCC;
          }
          else
          {
            _nextNode = CID.BOT_CHECK_FAIL;
          }
          return true;
        }
      }
      return false;
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
              _waitNum = Math.Min(_waitNum + UnityEngine.Random.Range(-2, 3), 59);
              _waitMin = Math.Max(10, Math.Min(_waitMin + UnityEngine.Random.Range(-2, 3), 59));
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
              _botCode = UnityEngine.Random.Range(0, 10000).ToString("0000");
                _currentQueuedARSClips = new AudioClip[] {
                    _audioARSClips[CID.BOT_CHECK_REQ.AudioL10NKey + "_S1"][0],
                    _audioARSClips[$"NUM_{_botCode}"][0],
                };
            }
            // 11. 코드 발급 완료
            else if (_currentNode.Id == CID.CODE_ISSUE_SUCC.Id)
            {
              int code = UnityEngine.Random.Range(0, 999999);
              _newIssuedCode = new TelephoneButtonType[6];
              for (int i = 5; i >= 0; i--)
              {
                int digit = code % 10;
                code /= 10;
                switch (digit)
                {
                  case 0: _newIssuedCode[i] = TelephoneButtonType.Number0; break;
                  case 1: _newIssuedCode[i] = TelephoneButtonType.Number1; break;
                  case 2: _newIssuedCode[i] = TelephoneButtonType.Number2; break;
                  case 3: _newIssuedCode[i] = TelephoneButtonType.Number3; break;
                  case 4: _newIssuedCode[i] = TelephoneButtonType.Number4; break;
                  case 5: _newIssuedCode[i] = TelephoneButtonType.Number5; break;
                  case 6: _newIssuedCode[i] = TelephoneButtonType.Number6; break;
                  case 7: _newIssuedCode[i] = TelephoneButtonType.Number7; break;
                  case 8: _newIssuedCode[i] = TelephoneButtonType.Number8; break;
                  case 9: _newIssuedCode[i] = TelephoneButtonType.Number9; break;
                }
              }
              _currentQueuedARSClips = new AudioClip[] {
                _audioARSClips[CID.CODE_ISSUE_SUCC.AudioL10NKey + "_S1"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[0]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[1]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[2]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[3]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[4]}"][0],
                _audioARSClips[$"NUM_{_newIssuedCode[5]}"][0],
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

            static bool TryGetDigit(TelephoneButtonType button, out int digit)
            {
              switch (button)
              {
                case TelephoneButtonType.Number0: digit = 0; return true;
                case TelephoneButtonType.Number1: digit = 1; return true;
                case TelephoneButtonType.Number2: digit = 2; return true;
                case TelephoneButtonType.Number3: digit = 3; return true;
                case TelephoneButtonType.Number4: digit = 4; return true;
                case TelephoneButtonType.Number5: digit = 5; return true;
                case TelephoneButtonType.Number6: digit = 6; return true;
                case TelephoneButtonType.Number7: digit = 7; return true;
                case TelephoneButtonType.Number8: digit = 8; return true;
                case TelephoneButtonType.Number9: digit = 9; return true;
                default:
                  digit = 0;
                  return false;
              }
            }

            static string ToDigitString(TelephoneButtonType[] buttons)
            {
              if (buttons == null || buttons.Length == 0)
                return string.Empty;

              var chars = new char[buttons.Length];
              for (int i = 0; i < buttons.Length; i++)
              {
                if (TryGetDigit(buttons[i], out var digit) == false)
                  return string.Empty;
                chars[i] = (char)('0' + digit);
              }
              return new string(chars);
            }

            static string GetNationKeyFromSequence(TelephoneButtonType[] buttons)
            {
              var digits = ToDigitString(buttons);
              if (string.IsNullOrEmpty(digits))
                return null;
              return $"NUM_{digits[0]}";
            }

            bool IsNationalityInfoNode(string nodeId)
            {
              return nodeId == CID.NATN_CODE_P1.Id
                || nodeId == CID.NATN_CODE_P2.Id
                || nodeId == CID.NATN_CODE_P3.Id
                || nodeId == CID.NATN_CODE_P5.Id
                || nodeId == CID.NATN_CODE_P6.Id
                || nodeId == CID.NATN_CODE_P7.Id
                || nodeId == CID.NATN_CODE_P8.Id
                || nodeId == CID.NATN_CODE_P9.Id;
            }

            bool TrySetIdentityRank(TelephoneButtonType button)
            {
              switch (button)
              {
                case TelephoneButtonType.Number1:
                  _selectedPersonalRank = "UNOFFICIAL_WORKING_STAFF";
                  return true;
                case TelephoneButtonType.Number2:
                  _selectedPersonalRank = "OFFICIAL_WORKING_STAFF";
                  return true;
                case TelephoneButtonType.Number3:
                  _selectedPersonalRank = "REGIONAL_OFFICIAL_WORKING_STAFF";
                  return true;
                case TelephoneButtonType.Number4:
                  _selectedPersonalRank = "CENTRAL_OFFICIAL_WORKING_STAFF";
                  return true;
                case TelephoneButtonType.Number5:
                  _selectedPersonalRank = "CENTRAL_OFFICIAL_AUTH_HEAD_STAFF";
                  return true;
                case TelephoneButtonType.Number6:
                  _selectedPersonalRank = "CENTRAL_OFFICIAL_TOP_STAFF";
                  return true;
                default:
                  return false;
              }
            }

            bool TryResolveMilitaryCode(TelephoneButtonType[] buttons, out string actionDescKey)
            {
              actionDescKey = null;
              var digits = ToDigitString(buttons);
              if (string.IsNullOrEmpty(digits))
                return false;

              if (digits == _powerCodeLowPart + _interceptCodeX0Part)
              {
                actionDescKey = CID.POWER_CODE_LOW.AudioL10NKey;
                return true;
              }
              if (digits == _powerCodeLowPart + _interceptCodeY0Part)
              {
                actionDescKey = CID.POWER_CODE_LOW.AudioL10NKey;
                return true;
              }
              if (digits == _powerCodeHighPart + _interceptCodeX0Part)
              {
                actionDescKey = CID.POWER_CODE_HIGH.AudioL10NKey;
                return true;
              }
              if (digits == _powerCodeHighPart + _interceptCodeY0Part)
              {
                actionDescKey = CID.POWER_CODE_HIGH.AudioL10NKey;
                return true;
              }

              return false;
            }
  }
}

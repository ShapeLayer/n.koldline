using System;
using System.Collections.Generic;
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
        var natnSelectedClips = _audioARSClips[CID.NATN_SELECTED.AudioL10NKey];
        switch (_localeCode)
        {
          case "ja":
          case "ko":
            _currentQueuedARSClips = new AudioClip[] {
                            _audioARSClips[_selectedNationId][0],
                            natnSelectedClips[0],
                        };
            break;
          default: // en 포함
            _currentQueuedARSClips = new AudioClip[] {
                            natnSelectedClips[0],
                            _audioARSClips[_selectedNationId][0],
                            natnSelectedClips.Length > 1 ? natnSelectedClips[1] : natnSelectedClips[0],
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

            static readonly Dictionary<string, string> NationCodeToId = new()
            {
              // P1 (NANP)
              { "1340", CID.US_VIRGIN_ISLANDS },
              { "1670", CID.NORTHERN_MARIANA_ISLANDS },
              { "1671", CID.GUAM },
              { "1684", CID.AMERICAN_SAMOA },
              { "1787", CID.PUERTO_RICO },
              { "1939", CID.PUERTO_RICO },
              { "1441", CID.BERMUDA },
              { "1264", CID.ANGUILLA },
              { "1284", CID.BRITISH_VIRGIN_ISLANDS },
              { "1345", CID.CAYMAN_ISLANDS },
              { "1649", CID.TURKS_AND_CAICOS_ISLANDS },
              { "1664", CID.MONTSERRAT },
              { "1242", CID.BAHAMAS },
              { "1246", CID.BARBADOS },
              { "1268", CID.ANTIGUA_AND_BARBUDA },
              { "1473", CID.GRENADA },
              { "1721", CID.SINT_MAARTEN },
              { "1758", CID.SAINT_LUCIA },
              { "1767", CID.DOMINICA },
              { "1784", CID.SAINT_VINCENT_AND_THE_GRENADINES },
              { "1868", CID.TRINIDAD_AND_TOBAGO },
              { "1869", CID.SAINT_KITTS_AND_NEVIS },
              { "1876", CID.JAMAICA },
              { "1809", CID.DOMINICAN_REPUBLIC },
              { "1829", CID.DOMINICAN_REPUBLIC },
              { "1849", CID.DOMINICAN_REPUBLIC },

              // P2
              { "20", CID.EGYPT },
              { "211", CID.SOUTH_SUDAN },
              { "212", CID.MOROCCO },
              { "213", CID.ALGERIA },
              { "216", CID.TUNISIA },
              { "218", CID.LIBYA },
              { "220", CID.GAMBIA },
              { "221", CID.SENEGAL },
              { "222", CID.MAURITANIA },
              { "223", CID.MALI },
              { "224", CID.GUINEA },
              { "225", CID.IVORY_COAST },
              { "226", CID.BURKINA_FASO },
              { "227", CID.NIGER },
              { "228", CID.TOGO },
              { "229", CID.BENIN },
              { "230", CID.MAURITIUS },
              { "231", CID.LIBERIA },
              { "232", CID.SIERRA_LEONE },
              { "233", CID.GHANA },
              { "234", CID.NIGERIA },
              { "235", CID.CHAD },
              { "236", CID.CENTRAL_AFRICAN_REPUBLIC },
              { "237", CID.CAMEROON },
              { "238", CID.CAPE_VERDE },
              { "239", CID.SAO_TOME_AND_PRINCIPE },
              { "240", CID.EQUATORIAL_GUINEA },
              { "241", CID.GABON },
              { "242", CID.REPUBLIC_OF_THE_CONGO },
              { "243", CID.DR_CONGO },
              { "244", CID.ANGOLA },
              { "245", CID.GUINEA_BISSAU },
              { "246", CID.BRITISH_INDIAN_OCEAN_TERRITORY },
              { "247", CID.ASCENSION_ISLAND },
              { "248", CID.SEYCHELLES },
              { "249", CID.SUDAN },
              { "250", CID.RWANDA },
              { "251", CID.ETHIOPIA },
              { "252", CID.SOMALIA },
              { "253", CID.DJIBOUTI },
              { "254", CID.KENYA },
              { "255", CID.TANZANIA },
              { "256", CID.UGANDA },
              { "257", CID.BURUNDI },
              { "258", CID.MOZAMBIQUE },
              { "260", CID.ZAMBIA },
              { "261", CID.MADAGASCAR },
              { "262", CID.REUNION },
              { "263", CID.ZIMBABWE },
              { "264", CID.NAMIBIA },
              { "265", CID.MALAWI },
              { "266", CID.LESOTHO },
              { "267", CID.BOTSWANA },
              { "268", CID.ESWATINI },
              { "269", CID.COMOROS },
              { "27", CID.SOUTH_AFRICA },
              { "290", CID.SAINT_HELENA },
              { "291", CID.ERITREA },
              { "297", CID.ARUBA },
              { "298", CID.FAROE_ISLANDS },
              { "299", CID.GREENLAND },

              // P3
              { "30", CID.GREECE },
              { "31", CID.NETHERLANDS },
              { "32", CID.BELGIUM },
              { "33", CID.FRANCE },
              { "34", CID.SPAIN },
              { "350", CID.GIBRALTAR },
              { "351", CID.PORTUGAL },
              { "352", CID.LUXEMBOURG },
              { "353", CID.IRELAND },
              { "354", CID.ICELAND },
              { "355", CID.ALBANIA },
              { "356", CID.MALTA },
              { "357", CID.CYPRUS },
              { "358", CID.FINLAND },
              { "35818", CID.ALAND_ISLANDS },
              { "359", CID.BULGARIA },
              { "36", CID.HUNGARY },
              { "370", CID.LITHUANIA },
              { "371", CID.LATVIA },
              { "372", CID.ESTONIA },
              { "373", CID.MOLDOVA },
              { "374", CID.ARMENIA },
              { "375", CID.BELARUS },
              { "376", CID.ANDORRA },
              { "377", CID.MONACO },
              { "378", CID.SAN_MARINO },
              { "380", CID.UKRAINE },
              { "381", CID.SERBIA },
              { "382", CID.MONTENEGRO },
              { "383", CID.KOSOVO },
              { "385", CID.CROATIA },
              { "386", CID.SLOVENIA },
              { "387", CID.BOSNIA_AND_HERZEGOVINA },
              { "389", CID.NORTH_MACEDONIA },
              { "39", CID.ITALY },
              { "3906698", CID.VATICAN_CITY },
              { "40", CID.ROMANIA },
              { "41", CID.SWITZERLAND },
              { "420", CID.CZECH_REPUBLIC },
              { "421", CID.SLOVAKIA },
              { "423", CID.LIECHTENSTEIN },
              { "43", CID.AUSTRIA },
              { "44", CID.UNITED_KINGDOM },
              { "441481", CID.GUERNSEY },
              { "441534", CID.JERSEY },
              { "441624", CID.ISLE_OF_MAN },
              { "45", CID.DENMARK },
              { "46", CID.SWEDEN },
              { "47", CID.NORWAY },
              { "4779", CID.SVALBARD },
              { "48", CID.POLAND },
              { "49", CID.GERMANY },

              // P5
              { "500", CID.FALKLAND_ISLANDS },
              { "501", CID.BELIZE },
              { "502", CID.GUATEMALA },
              { "503", CID.EL_SALVADOR },
              { "504", CID.HONDURAS },
              { "505", CID.NICARAGUA },
              { "506", CID.COSTA_RICA },
              { "507", CID.PANAMA },
              { "508", CID.SAINT_PIERRE_AND_MIQUELON },
              { "509", CID.HAITI },
              { "51", CID.PERU },
              { "52", CID.MEXICO },
              { "53", CID.CUBA },
              { "54", CID.ARGENTINA },
              { "55", CID.BRAZIL },
              { "56", CID.CHILE },
              { "57", CID.COLOMBIA },
              { "58", CID.VENEZUELA },
              { "590", CID.GUADELOUPE },
              { "591", CID.BOLIVIA },
              { "592", CID.GUYANA },
              { "593", CID.ECUADOR },
              { "594", CID.FRENCH_GUIANA },
              { "595", CID.PARAGUAY },
              { "596", CID.MARTINIQUE },
              { "597", CID.SURINAME },
              { "598", CID.URUGUAY },
              { "5993", CID.SINT_EUSTATIUS },
              { "5994", CID.SABA },
              { "5997", CID.BONAIRE },
              { "5999", CID.CURACAO },

              // P6
              { "60", CID.MALAYSIA },
              { "61", CID.AUSTRALIA },
              { "6189162", CID.COCOS_ISLANDS },
              { "6189164", CID.CHRISTMAS_ISLAND },
              { "62", CID.INDONESIA },
              { "63", CID.PHILIPPINES },
              { "64", CID.NEW_ZEALAND },
              { "65", CID.SINGAPORE },
              { "66", CID.THAILAND },
              { "670", CID.EAST_TIMOR },
              { "6721", CID.AUSTRALIAN_ANTARCTIC_TERRITORY },
              { "6723", CID.NORFOLK_ISLAND },
              { "673", CID.BRUNEI },
              { "674", CID.NAURU },
              { "675", CID.PAPUA_NEW_GUINEA },
              { "676", CID.TONGA },
              { "677", CID.SOLOMON_ISLANDS },
              { "678", CID.VANUATU },
              { "679", CID.FIJI },
              { "680", CID.PALAU },
              { "681", CID.WALLIS_AND_FUTUNA },
              { "682", CID.COOK_ISLANDS },
              { "683", CID.NIUE },
              { "685", CID.SAMOA },
              { "686", CID.KIRIBATI },
              { "687", CID.NEW_CALEDONIA },
              { "688", CID.TUVALU },
              { "689", CID.FRENCH_POLYNESIA },
              { "690", CID.TOKELAU },
              { "691", CID.MICRONESIA },
              { "692", CID.MARSHALL_ISLANDS },

              // P7
              { "7840", CID.ABKHAZIA },
              { "7940", CID.ABKHAZIA },
              { "7850", CID.SOUTH_OSSETIA },
              { "7929", CID.SOUTH_OSSETIA },

              // P8
              { "81", CID.JAPAN },
              { "82", CID.SOUTH_KOREA },
              { "84", CID.VIETNAM },
              { "850", CID.NORTH_KOREA },
              { "852", CID.HONG_KONG },
              { "853", CID.MACAU },
              { "855", CID.CAMBODIA },
              { "856", CID.LAOS },
              { "86", CID.CHINA },
              { "880", CID.BANGLADESH },
              { "886", CID.TAIWAN },

              // P9
              { "90", CID.TURKEY },
              { "90533", CID.NORTHERN_CYPRUS },
              { "90548", CID.NORTHERN_CYPRUS },
              { "91", CID.INDIA },
              { "92", CID.PAKISTAN },
              { "93", CID.AFGHANISTAN },
              { "94", CID.SRI_LANKA },
              { "95", CID.MYANMAR },
              { "960", CID.MALDIVES },
              { "961", CID.LEBANON },
              { "962", CID.JORDAN },
              { "963", CID.SYRIA },
              { "964", CID.IRAQ },
              { "965", CID.KUWAIT },
              { "966", CID.SAUDI_ARABIA },
              { "967", CID.YEMEN },
              { "968", CID.OMAN },
              { "970", CID.PALESTINE },
              { "971", CID.UAE },
              { "972", CID.ISRAEL },
              { "973", CID.BAHRAIN },
              { "974", CID.QATAR },
              { "975", CID.BHUTAN },
              { "976", CID.MONGOLIA },
              { "977", CID.NEPAL },
              { "98", CID.IRAN },
              { "992", CID.TAJIKISTAN },
              { "993", CID.TURKMENISTAN },
              { "994", CID.AZERBAIJAN },
              { "995", CID.GEORGIA },
              { "99534", CID.SOUTH_OSSETIA },
              { "996", CID.KYRGYZSTAN },
              { "998", CID.UZBEKISTAN },
            };

            static string GetNationKeyFromSequence(TelephoneButtonType[] buttons)
            {
              var digits = ToDigitString(buttons);
              if (string.IsNullOrEmpty(digits))
                return null;

              if (NationCodeToId.TryGetValue(digits, out var nationId))
                return nationId;

              if (digits == "1")
                return CID.USA;

              if (digits.StartsWith("1") && digits.Length >= 4)
              {
                if (int.TryParse(digits.Substring(1, 3), out var areaCode))
                {
                  if (areaCode == 204 || (areaCode >= 226 && areaCode <= 942))
                    return CID.CANADA;

                  if (areaCode >= 201 && areaCode <= 989)
                    return CID.USA;
                }
              }

              if (digits.StartsWith("76") || digits.StartsWith("77"))
                return CID.KAZAKHSTAN;

              if (digits.StartsWith("7"))
                return CID.RUSSIA;

              return null;
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

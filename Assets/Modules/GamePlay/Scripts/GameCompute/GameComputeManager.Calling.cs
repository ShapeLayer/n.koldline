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
    [SerializeField] ResponseNode _currentNode;
    [SerializeField] ResponseNode _nextNode;
    [SerializeField] AudioClip[] _currentQueuedARSClips;

    [Header("Calling Playback")]
    [SerializeField] float _navigationHintIntervalSeconds = 6f;
    [SerializeField] bool _isCallingActive;
    [SerializeField] bool _isARSPlaying;
    Coroutine _arsPlaybackRoutine;
    Coroutine _navigationHintRoutine;

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
    [SerializeField] string _interceptCodeY0Part;  // Y0 방식 뒤 2자리
    [SerializeField] string _botCode;              // 로봇 확인용 코드
    [SerializeField] TelephoneButtonType[] _newIssuedCode;        // 직통 회선 코드

    readonly List<TelephoneButtonType> _numberSequenceBuffer = new();
    TelephoneButtonType _pendingButton;
    bool _hasPendingButton;

    void PrepareInstanceVariable()
    {
      PrepareInstanceVariable_Specific();
    }

    void ReadyCurrentARS()
    {
      _localeCode = LocalizationUtils.GetLanguageCode(LocalizationSettings.SelectedLocale);

      if (_currentNode.IsContentFormattingRequired)
        ReadyCurrentARSFormatting();
      else
        _currentQueuedARSClips = _audioARSClips[_currentNode.AudioL10NKey];
    }

    public void StartCalling()
    {
      _isCallingActive = true;
      EnsureCurrentNodeInitialized();
      ReadyCurrentARS();
      PlayCurrentARSAndHandleTransitions();
    }

    public void EndCalling()
    {
      _isCallingActive = false;
      StopARSPlayback();
      StopNavigationHintLoop();
      _currentNode = CID.CALLING_START;
      _nextNode = default;
      _numberSequenceBuffer.Clear();
    }

    void OnTelephoneButtonClickedTransfer(TelephoneButtonType buttonType)
    {
      if (_isARSPlaying)
        return;

      _pendingButton = buttonType;
      _hasPendingButton = true;
      StopNavigationHintLoop();
      OnButtonPressed();
    }

    public void OnButtonPressed()
    {
      if (_hasPendingButton == false)
        return;

      EnsureCurrentNodeInitialized();
      ApplyAlwaysTransitionsIfNeeded();

      var pressedButton = _pendingButton;
      _hasPendingButton = false;

      if (pressedButton == TelephoneButtonType.Sharp)
      {
        if (_numberSequenceBuffer.Count > 0)
        {
          ProcessCurrentARSResponse(NodeTransferConditionType.NumberSequence, _numberSequenceBuffer.ToArray());
          _numberSequenceBuffer.Clear();
        }
        else
        {
          ProcessCurrentARSResponse(NodeTransferConditionType.SinglePressed, new[] { pressedButton });
        }

        ApplyNodeTransitionIfReady();
        return;
      }

      if (ShouldBufferNumberSequence() && IsNumberButton(pressedButton))
      {
        _numberSequenceBuffer.Add(pressedButton);
        return;
      }

      ProcessCurrentARSResponse(NodeTransferConditionType.SinglePressed, new[] { pressedButton });
      ApplyNodeTransitionIfReady();
    }
    public void ProcessCurrentARSResponse(
      NodeTransferConditionType conditionType,
      TelephoneButtonType[] pressedButtons = null
    )
    {
      if (ProcessCurrentSpecialARSResponse(conditionType, pressedButtons))
        return;
      
      ProcessNormalARSResponse(conditionType, pressedButtons);
    }

    void ProcessNormalARSResponse(
      NodeTransferConditionType conditionType,
      TelephoneButtonType[] pressedButtons = null
    )
    {
      if (_currentNode.Transfers == null || _currentNode.Transfers.Length == 0)
        return;

      foreach (var transfer in _currentNode.Transfers)
      {
        if (transfer.Condition.Condition != conditionType)
          continue;

        switch (conditionType)
        {
          case NodeTransferConditionType.Always:
          case NodeTransferConditionType.NumberSequence:
            _nextNode = transfer.ToNode;
            return;
          case NodeTransferConditionType.SinglePressed:
            if (pressedButtons == null || pressedButtons.Length == 0)
              continue;
            if (transfer.Condition.Value == null || transfer.Condition.Value.Length == 0)
              continue;
            if (transfer.Condition.Value.Contains(pressedButtons[0]))
            {
              _nextNode = transfer.ToNode;
              return;
            }
            break;
          case NodeTransferConditionType.None:
          default:
            break;
        }
      }
    }

    void EnsureCurrentNodeInitialized()
    {
      if (string.IsNullOrEmpty(_currentNode.Id))
      {
        _currentNode = CID.CALLING_START;
        ReadyCurrentARS();
      }
    }

    void ApplyNodeTransitionIfReady(bool startPlayback = true)
    {
      if (string.IsNullOrEmpty(_nextNode.Id))
      {
        RestartNavigationHintLoopIfIdle();
        return;
      }

      _currentNode = _nextNode;
      _nextNode = default;
      _numberSequenceBuffer.Clear();
      ReadyCurrentARS();
      if (startPlayback)
        PlayCurrentARSAndHandleTransitions();
    }

    void ApplyAlwaysTransitionsIfNeeded()
    {
      int guard = 0;
      while (HasTransferCondition(NodeTransferConditionType.Always) && guard < 5)
      {
        ProcessCurrentARSResponse(NodeTransferConditionType.Always);
        if (string.IsNullOrEmpty(_nextNode.Id))
          break;
        ApplyNodeTransitionIfReady(startPlayback: false);
        guard++;
      }
    }

    void PlayCurrentARSAndHandleTransitions()
    {
      if (_isCallingActive == false)
        return;

      StopARSPlayback();
      StopNavigationHintLoop();
      _arsPlaybackRoutine = StartCoroutine(PlayARSSequenceCoroutine());
    }

    System.Collections.IEnumerator PlayARSSequenceCoroutine()
    {
      _isARSPlaying = true;

      var source = _telephoneController != null ? _telephoneController.AudioSrc : null;
      if (source == null)
      {
        _isARSPlaying = false;
        yield break;
      }

      if (_currentQueuedARSClips != null)
      {
        for (int i = 0; i < _currentQueuedARSClips.Length; i++)
        {
          var clip = _currentQueuedARSClips[i];
          if (clip == null)
            continue;

          source.PlayOneShot(clip);
          yield return new WaitForSeconds(clip.length);
        }
      }

      _isARSPlaying = false;

      if (HasTransferCondition(NodeTransferConditionType.Always))
      {
        ProcessCurrentARSResponse(NodeTransferConditionType.Always);
        ApplyNodeTransitionIfReady();
      }
      else
      {
        RestartNavigationHintLoopIfIdle();
      }
    }

    void StopARSPlayback()
    {
      if (_arsPlaybackRoutine != null)
      {
        StopCoroutine(_arsPlaybackRoutine);
        _arsPlaybackRoutine = null;
      }

      _isARSPlaying = false;

      var source = _telephoneController != null ? _telephoneController.AudioSrc : null;
      if (source != null)
        source.Stop();
    }

    void RestartNavigationHintLoopIfIdle()
    {
      if (_isCallingActive == false)
        return;
      if (_isARSPlaying)
        return;
      if (_currentNode.SuppressNavigationHint)
        return;

      StartNavigationHintLoop();
    }

    void StartNavigationHintLoop()
    {
      if (_navigationHintRoutine != null)
        return;
      _navigationHintRoutine = StartCoroutine(NavigationHintLoopCoroutine());
    }

    void StopNavigationHintLoop()
    {
      if (_navigationHintRoutine == null)
        return;
      StopCoroutine(_navigationHintRoutine);
      _navigationHintRoutine = null;
    }

    System.Collections.IEnumerator NavigationHintLoopCoroutine()
    {
      var source = _telephoneController != null ? _telephoneController.AudioSrc : null;
      if (source == null)
      {
        _navigationHintRoutine = null;
        yield break;
      }

      while (_isCallingActive && _currentNode.SuppressNavigationHint == false)
      {
        yield return new WaitForSeconds(_navigationHintIntervalSeconds);

        if (_isARSPlaying)
          continue;

        if (_audioARSClips.TryGetValue(AUDIO_CLIP_NAVIGATION_HINT_KEY, out var clips) && clips.Length > 0 && clips[0] != null)
          source.PlayOneShot(clips[0]);
      }

      _navigationHintRoutine = null;
    }

    bool HasTransferCondition(NodeTransferConditionType conditionType)
    {
      if (_currentNode.Transfers == null)
        return false;

      for (int i = 0; i < _currentNode.Transfers.Length; i++)
      {
        if (_currentNode.Transfers[i].Condition.Condition == conditionType)
          return true;
      }
      return false;
    }

    bool ShouldBufferNumberSequence()
    {
      if (HasTransferCondition(NodeTransferConditionType.NumberSequence))
        return true;

      return _currentNode.Id == CID.NATIONALITY_REQ.Id;
    }

    bool IsNumberButton(TelephoneButtonType buttonType)
    {
      switch (buttonType)
      {
        case TelephoneButtonType.Number0:
        case TelephoneButtonType.Number1:
        case TelephoneButtonType.Number2:
        case TelephoneButtonType.Number3:
        case TelephoneButtonType.Number4:
        case TelephoneButtonType.Number5:
        case TelephoneButtonType.Number6:
        case TelephoneButtonType.Number7:
        case TelephoneButtonType.Number8:
        case TelephoneButtonType.Number9:
          return true;
        default:
          return false;
      }
    }
  }
}

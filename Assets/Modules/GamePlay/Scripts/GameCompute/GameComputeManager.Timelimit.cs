using GamePlay.Definitions;
using Infrastructure.Commons;
using UnityEngine;

namespace GamePlay.GameCompute
{
  public partial class GameComputeManager
  {
    [Header("Time Limit")]
    public static readonly int timeLimitMs = Defaults.GAME_COMPUTE_TIME_LIMIT;
    [SerializeField] int _timeRemainingMs;
    [SerializeField] bool _timeLimitElapsingActive = false;

    [SerializeField] bool _timeLimitTickTockPending;
    [SerializeField] int _timeLimitLastWholeSecond;

    void Update_TimeLimit()
    {
      ProcessTimeLimitElapsed();

      if (_timeLimitTickTockPending)
      {
        _timeLimitTickTockPending = false;
        if (_mainCameraController != null && _audioTimerTickTock != null)
          _ = _mainCameraController.PlayAudioOneShotAsync(_audioTimerTickTock);
      }

      if (_worldMapPaneController != null)
        _worldMapPaneController.SetTimerTimeMilliseconds(_timeRemainingMs);
    }

    void ProcessTimeLimitElapsed()
    {
      if (!_timeLimitElapsingActive)
      {
        return;
      }

      var prevWholeSecond = Mathf.Max(0, _timeRemainingMs) / 1000;

      var deltaMs = Mathf.CeilToInt(Time.unscaledDeltaTime * 1000f);
      if (deltaMs <= 0)
        return;

      _timeRemainingMs = Mathf.Max(0, _timeRemainingMs - deltaMs);

      var newWholeSecond = _timeRemainingMs / 1000;
      if (newWholeSecond != prevWholeSecond)
      {
        _timeLimitTickTockPending = true;
        _timeLimitLastWholeSecond = newWholeSecond;
      }

      if (_timeRemainingMs <= 0)
        _timeLimitElapsingActive = false;
    }

    public void ResetTimeLimit()
    {
      _timeRemainingMs = timeLimitMs;
      _timeLimitElapsingActive = false;

      _timeLimitTickTockPending = false;
      _timeLimitLastWholeSecond = _timeRemainingMs / 1000;
    }

    public void StartTimeLimitCountdown()
    {
      ResetTimeLimit();

      _timeLimitElapsingActive = true;
      Update_TimeLimit();
    }

    static int ToMilliseconds(TimeStruct value)
    {
      return (((value.Hours * 60 + value.Minutes) * 60 + value.Seconds) * 1000) + value.Milliseconds;
    }
  }
}

using GamePlay.Definitions;
using Infrastructure.Commons;
using UnityEngine;

namespace GamePlay.GameCompute
{
  public partial class GameComputeManager
  {
    public static readonly int timeLimitMs = Defaults.GAME_COMPUTE_TIME_LIMIT;
    [SerializeField] int _timeRemainingMs;
    [SerializeField] float _timeLimitEndsAt;
    [SerializeField] bool _timeLimitElapsingActive = false;

    void Update_TimeLimit()
    {
      ProcessTimeLimitElapsed();
    }

    void ProcessTimeLimitElapsed()
    {
      if (!_timeLimitElapsingActive)
      {
        return;
      }

      var remainingSeconds = Mathf.Max(0f, _timeLimitEndsAt - Time.realtimeSinceStartup);
      var remainingMilliseconds = (int)(remainingSeconds * 1000f);

      _timeRemainingMs = remainingMilliseconds;

      if (remainingMilliseconds <= 0)
      {
        _timeLimitElapsingActive = false;
      }
    }

    public void ResetTimeLimit()
    {
      _timeRemainingMs = timeLimitMs;
      _timeLimitElapsingActive = false;
      _timeLimitEndsAt = 0f;
    }

    public void StartTimeLimitCountdown()
    {
      ResetTimeLimit();

      _timeLimitElapsingActive = true;
      _timeLimitEndsAt = Time.realtimeSinceStartup + (timeLimitMs / 1000f);

      Update_TimeLimit();
    }

    static int ToMilliseconds(TimeStruct value)
    {
      return (((value.Hours * 60 + value.Minutes) * 60 + value.Seconds) * 1000) + value.Milliseconds;
    }
  }
}

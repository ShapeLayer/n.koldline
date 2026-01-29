using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Camera
{
  public partial class MainCameraController : MonoBehaviour
  {
    readonly List<OneShotPlayback> _oneShotPlaybacks = new();

    private void InitAudioSource()
    {
      if (_audioSource == null) return;
      _audioSource.volume = 0.5f;
    }

    public void PlayAudioOneShot(AudioClip clip, float volumeScale = 1f)
    {
      PlayAudioOneShotInternal(clip, volumeScale);
    }

    public Task PlayAudioOneShotAsync(AudioClip clip, float volumeScale = 1f)
    {
      var playback = PlayAudioOneShotInternal(clip, volumeScale);
      return playback?.Completion.Task ?? Task.CompletedTask;
    }

    OneShotPlayback PlayAudioOneShotInternal(AudioClip clip, float volumeScale)
    {
      if (clip == null || _audioSource == null)
        return null;

      var source = CreateOneShotAudioSource();
      source.PlayOneShot(clip, volumeScale);

      var durationSeconds = clip.length / Mathf.Max(source.pitch, 0.01f);
      var playback = new OneShotPlayback(source);
      _oneShotPlaybacks.Add(playback);
      StartCoroutine(ReleaseAfterSeconds(durationSeconds, playback));
      return playback;
    }

    AudioSource CreateOneShotAudioSource()
    {
      var source = gameObject.AddComponent<AudioSource>();
      source.playOnAwake = false;
      source.volume = _audioSource.volume;
      source.pitch = _audioSource.pitch;
      source.spatialBlend = _audioSource.spatialBlend;
      source.outputAudioMixerGroup = _audioSource.outputAudioMixerGroup;
      source.dopplerLevel = _audioSource.dopplerLevel;
      source.rolloffMode = _audioSource.rolloffMode;
      source.minDistance = _audioSource.minDistance;
      source.maxDistance = _audioSource.maxDistance;
      source.mute = _audioSource.mute;
      source.priority = _audioSource.priority;
      source.loop = false;
      return source;
    }

    IEnumerator ReleaseAfterSeconds(float durationSeconds, OneShotPlayback playback)
    {
      if (durationSeconds > 0f)
        yield return new WaitForSecondsRealtime(durationSeconds);

      ReleasePlayback(playback);
    }

    void ReleasePlayback(OneShotPlayback playback)
    {
      if (playback == null) return;

      if (playback.Source != null)
        Destroy(playback.Source);

      _oneShotPlaybacks.Remove(playback);
      playback.Completion.TrySetResult(true);
    }

    class OneShotPlayback
    {
      public AudioSource Source { get; }
      public TaskCompletionSource<bool> Completion { get; }

      public OneShotPlayback(AudioSource source)
      {
        Source = source;
        Completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
      }
    }
  }
}

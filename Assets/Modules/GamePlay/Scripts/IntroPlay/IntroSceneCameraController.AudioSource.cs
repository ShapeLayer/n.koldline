using UnityEngine;

namespace Infrastructure.Localization
{
  [RequireComponent(typeof(AudioSource))]
  public partial class IntroSceneCameraController : MonoBehaviour
  {
    AudioSource _audioSource;

    void Awake_AudioSource()
    {
      _audioSource = GetComponent<AudioSource>();
      InitAudioSource();
    }

    private void InitAudioSource()
    {
      _audioSource.loop = true;
      _audioSource.playOnAwake = false;
      _audioSource.volume = 0.5f;
    }

    public void PlayAudio(AudioClip clip)
    {
      _audioSource.clip = clip;
      _audioSource.Play();
    }

    public void StopAudio()
    {
      _audioSource.Stop();
    }
  }
}

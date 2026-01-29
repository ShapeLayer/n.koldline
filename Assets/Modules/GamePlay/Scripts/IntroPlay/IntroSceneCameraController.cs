using UnityEngine;

namespace Infrastructure.Localization
{
  /**
   * AudioSource를 여기에 붙인 이유:
   * 인트로 씬이라 AudioSource를 어디에 붙일데가 없음.
   */
  public partial class IntroSceneCameraController : MonoBehaviour
  {
    void Awake()
    {
      Awake_AudioSource();
    }
  }
}

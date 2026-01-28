using UnityEngine;
using Infrastructure.UIDocuments;

namespace GamePlay.IntroPlay
{
  public partial class IntroScenePlayManager : MonoBehaviour
  {
    static IntroScenePlayManager _instance;

    void Awake()
    {
      if (_instance == null)
      {
        _instance = this;
      }
      else
      {
        Destroy(gameObject);
        return;
      }

      Awake_Loader();
    }

    void Start()
    {
      Start_Locale();
    }

    void Update()
    {
      Update_Input();
    }
  }
}

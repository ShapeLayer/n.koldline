using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace GamePlay.GameCompute
{
  public partial class GameComputeManager : MonoBehaviour
  {
    private static GameComputeManager _instance;
    public static GameComputeManager Instance => _instance;

    void Awake()
    {
      if (_instance != null && _instance != this)
      {
        Destroy(gameObject);
        return;
      }
      _instance = this;
      DontDestroyOnLoad(gameObject);

      LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2];
      Awake_Loader();
    }
    
    void Start()
    {
      Start_Loader();
    }

    void Update()
    {
      Update_TimeLimit();
    }
  }
}

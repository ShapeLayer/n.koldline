using UnityEngine;

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
    }
    
    void Update()
    {
      Update_TimeLimit();
    }
  }
}

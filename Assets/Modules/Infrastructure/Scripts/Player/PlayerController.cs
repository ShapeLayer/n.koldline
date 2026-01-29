using UnityEngine;

namespace Infrastructure.Player
{
  public partial class PlayerController : MonoBehaviour
  {
    void OnEnable()
    {
      
    }

    void Awake()
    {
      Awake_Loader();
      Awake_Camera();
      Awake_Movement();
      Awake_Interactables();
    }

    void Start()
    {
      // Start_Input();
    }

    void Update()
    {
      Update_Movement();
      Update_Interactables();
      Update_Input();
    }

    void LateUpdate()
    {
      LateUpdate_Camera();
    }
  }
}

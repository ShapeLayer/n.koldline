using UnityEngine;

namespace Infrastructure.Player
{
  public partial class PlayerController : MonoBehaviour
  {
    void Awake()
    {
      Awake_Movement();
    }

    void Start()
    {
      // Start_Input();
    }

    void Update()
    {
      Update_Movement();
      // Update_Input();
    }
  }
}

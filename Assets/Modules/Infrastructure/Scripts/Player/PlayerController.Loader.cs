using Infrastructure.Camera;
using Infrastructure.Exceptions;
using Infrastructure.UIDocuments;
using UnityEngine;

namespace Infrastructure.Player
{
  [RequireComponent(typeof(CharacterController))]
  public partial class PlayerController : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] MainCameraController _camController;
    [SerializeField] CharacterController _characterController;
    [SerializeField] InteractableUIController _interactableUIController;

    void Awake_Loader()
    {
      _characterController = GetComponent<CharacterController>();
      if (_characterController == null) throw new InitializingNotSucceed("CharacterController not found");

      _interactableUIController = FindFirstObjectByType<InteractableUIController>();
      if (_interactableUIController == null) throw new InitializingNotSucceed("InteractableUIController not found");
    }

    void Start_Loader()
    {
      if (_camController == null)
        _camController = MainCameraController.Instance;
      if (_camController == null) throw new InitializingNotSucceed("MainCameraController not found");
    }
  }
}

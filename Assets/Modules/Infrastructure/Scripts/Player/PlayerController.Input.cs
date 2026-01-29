using UnityEngine;

namespace Infrastructure.Player
{
  public partial class PlayerController
  {
    [Header("Key Configuration")]
    [SerializeField] private KeyCode _keyMovingRunning = KeyCode.LeftControl;
    [SerializeField] private KeyCode _keyInteractInteractableObject = KeyCode.F;
    [SerializeField] private KeyCode _keySelectPreviousInteractable = KeyCode.Minus;
    [SerializeField] private KeyCode _keySelectNextInteractable = KeyCode.Equals;

    void Update_Input()
    {
      // Movement input handles at Playercontroller.Movement
      HandleInteractInteractableObject();
    }

    private void HandleInteractInteractableObject()
    {
      if (Input.GetKeyDown(_keyInteractInteractableObject)) TryInteractWithSelection();
      HandleInteractablesSelectionInput();
    }
  }
}

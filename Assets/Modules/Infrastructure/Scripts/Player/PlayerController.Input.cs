using UnityEngine;

namespace Infrastructure.Player
{
  public partial class PlayerController
  {
    [Header("Key Configuration")]
    [SerializeField] private KeyCode _keyMovingRunning = KeyCode.LeftControl;

    /*
    private void Start_Input()
    {
      RegisterOverlayLock();
    }

    private void Update_Input()
    {
      var escapeConsumed = HandleEscape();

      if (_KeyHandlingLocked) return;

      if (!escapeConsumed)
        HandleEscapeMenuInput();

      HandleInteractInteractableObject();
    }

    private void RegisterOverlayLock(IUIOverlay overlay, Action<bool> setLocked)
    {
      if (overlay == null || setLocked == null) return;

      overlay.OverlayPushed += () => setLocked(true);
      overlay.OverlayPopped += () => setLocked(false);
    }

    private void HandleInteractInteractableObject()
    {
      if (Input.GetKeyDown(_keyInteractInteractableObject)) TryInteractWithSelection();
      HandleInteractablesSelectionInput();
    }
    */
  }
}

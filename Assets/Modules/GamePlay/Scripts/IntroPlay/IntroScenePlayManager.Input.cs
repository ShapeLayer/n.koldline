using UnityEngine;

namespace GamePlay.IntroPlay
{
  public partial class IntroScenePlayManager : MonoBehaviour
  {
    void Update_Input()
    {
      if (_normalFullCoverDialogueController == null && _kpDialogueController == null) return;

      if (
        Input.GetMouseButtonDown(0) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetKeyDown(KeyCode.KeypadEnter) ||
        Input.GetKeyDown(KeyCode.Space)
      )
      {
        switch (_activeDialogue)
        {
          case ActiveDialogue.Normal:
            _normalFullCoverDialogueController?.HandleInteraction();
            break;
          case ActiveDialogue.Kp:
            _kpDialogueController?.HandleInteraction();
            break;
          default:
            _normalFullCoverDialogueController?.HandleInteraction();
            _kpDialogueController?.HandleInteraction();
            break;
        }
      }
    }
  }
}

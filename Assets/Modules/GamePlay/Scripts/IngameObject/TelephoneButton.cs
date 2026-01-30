using UnityEngine;
using GamePlay.CallingInteraction;

namespace GamePlay.IngameObject
{
  public sealed class TelephoneButton : MonoBehaviour
  {
    [SerializeField] private TelephoneButtonType key;
    public TelephoneButtonType Key => key;
    public void OnButtonClicked()
    {
      
    }
  }
}

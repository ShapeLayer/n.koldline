using UnityEngine;

namespace Infrastructure.Interactables
{
  public abstract class InteractableObjectABC : MonoBehaviour, IInteractable
  {
    public abstract string DisplayText { get; }
    public abstract Sprite DisplayIcon { get; }
    public abstract Color DisplayColor { get; }
    public abstract bool IsInteractable { get; }
    public abstract void Interact(Transform interactor);
  }
}

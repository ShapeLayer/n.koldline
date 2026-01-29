using System.Collections.Generic;
using Infrastructure.Interactables;
using UnityEngine;

namespace Infrastructure.Player
{
  public partial class PlayerController : MonoBehaviour
  {
    [Header("Interactables")]
    [SerializeField, Min(0.5f)] private float _interactDetectionRadius = 1.3f;
    [SerializeField] private LayerMask _interactionLayerMask = ~0;
    [SerializeField, Min(0.02f)] private float _interactQueryInterval = 0.05f;

    [SerializeField] private int _selectedInteractableIndex = -1;

    private readonly Collider[] _overlapColliderBuffer = new Collider[32];
    private readonly List<IInteractable> _nearbyInteractables = new List<IInteractable>();
    private readonly List<IInteractable> _scratchInteractables = new List<IInteractable>();
    private float _nextInteractQueryTime;

    void Awake_Interactables()
    {
      _nearbyInteractables.Clear();
      _scratchInteractables.Clear();
      _selectedInteractableIndex = -1;
      _nextInteractQueryTime = 0f;

      if (_interactableUIController != null)
        _interactableUIController.UpdateInteractables(_nearbyInteractables, _selectedInteractableIndex);
    }

    void Update_Interactables()
    {
      QueryNearbyInteractables();
    }

    private void QueryNearbyInteractables()
    {
      if (Time.time < _nextInteractQueryTime) return;
      _nextInteractQueryTime = Time.time + _interactQueryInterval;

      int count = Physics.OverlapSphereNonAlloc(
        transform.position,
        _interactDetectionRadius,
        _overlapColliderBuffer,
        _interactionLayerMask,
        QueryTriggerInteraction.Collide
      );

      _scratchInteractables.Clear();
      for (int i = 0; i < count; i++)
      {
        var collider = _overlapColliderBuffer[i];
        if (collider == null) continue;
        if (collider.TryGetComponent(out IInteractable interactable) && !_scratchInteractables.Contains(interactable))
        {
          if (!interactable.IsInteractable) continue;
          _scratchInteractables.Add(interactable);
        }
      }

      if (_scratchInteractables.Count > 1)
        _scratchInteractables.Sort(CompareByDistance);

      bool changed = HasListChanged(_nearbyInteractables, _scratchInteractables);

      _nearbyInteractables.Clear();
      _nearbyInteractables.AddRange(_scratchInteractables);

      if (changed)
      {
        _selectedInteractableIndex = _nearbyInteractables.Count > 0 ? 0 : -1;
        _interactableUIController?.UpdateInteractables(_nearbyInteractables, _selectedInteractableIndex);
      }
    }

    private void TryInteractWithSelection()
    {
      var selected = GetSelectedInteractable();
      if (selected == null) return;

      selected.Interact(transform);
    }

    private void HandleInteractablesSelectionInput()
    {
      if (_nearbyInteractables.Count == 0) return;

      float scroll = Input.GetAxis("Mouse ScrollWheel");
      if (scroll > 0.01f)
        MoveSelected(-1);
      else if (scroll < -0.01f)
        MoveSelected(1);

      if (Input.GetKeyDown(_keySelectPreviousInteractable) || Input.GetKeyDown(KeyCode.KeypadMinus))
        MoveSelected(-1);

      if (Input.GetKeyDown(_keySelectNextInteractable) || Input.GetKeyDown(KeyCode.KeypadPlus))
        MoveSelected(1);
    }

    private IInteractable GetSelectedInteractable()
    {
      if (_selectedInteractableIndex < 0 || _selectedInteractableIndex >= _nearbyInteractables.Count)
        return null;

      return _nearbyInteractables[_selectedInteractableIndex];
    }

    private void MoveSelected(int delta)
    {
      if (_nearbyInteractables.Count == 0) return;

      int next = _selectedInteractableIndex;
      if (next < 0) next = 0;
      next = (next + delta) % _nearbyInteractables.Count;
      if (next < 0) next += _nearbyInteractables.Count;

      if (next == _selectedInteractableIndex) return;
      _selectedInteractableIndex = next;
      _interactableUIController?.UpdateInteractables(_nearbyInteractables, _selectedInteractableIndex);
    }

    private int CompareByDistance(IInteractable a, IInteractable b)
    {
      float da = GetDistanceSqr(a);
      float db = GetDistanceSqr(b);
      return da.CompareTo(db);
    }

    private float GetDistanceSqr(IInteractable interactable)
    {
      if (interactable is MonoBehaviour mono)
      {
        return (mono.transform.position - transform.position).sqrMagnitude;
      }

      return 0f;
    }

    private static bool HasListChanged(List<IInteractable> previous, List<IInteractable> next)
    {
      if (previous.Count != next.Count) return true;
      for (int i = 0; i < next.Count; i++)
      {
        if (!previous.Contains(next[i])) return true;
      }

      return false;
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.cyan;
      Gizmos.DrawWireSphere(transform.position, _interactDetectionRadius);
    }
#endif
  }
}

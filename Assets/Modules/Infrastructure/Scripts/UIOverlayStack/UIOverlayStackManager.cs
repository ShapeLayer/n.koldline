using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.UIStack
{
  public class UIOverlayStackManager : MonoBehaviour
  {
    private static UIOverlayStackManager _instance;
    public static UIOverlayStackManager Instance => _instance;

    private Stack<IOverlayUI> _overlayStack = new Stack<IOverlayUI>();

    void Awake()
    {
      if (_instance != null && _instance != this)
      {
        Destroy(gameObject);
      }
      else
      {
        _instance = this;
        DontDestroyOnLoad(gameObject);
      }
    }

    public int Count => _overlayStack.Count;

    public void Push(IOverlayUI overlay)
    {
      if (overlay == null)
        return;

      if (_overlayStack.Contains(overlay))
        Remove(overlay);

      _overlayStack.Push(overlay);
      overlay.Show();
      UpdateCursorState();
#if UNITY_EDITOR
      Debug.Log($"[UIOverlayStackManager] Pushed overlay. New stack count: {_overlayStack.Count}");
#endif
    }

    public void Pop()
    {
      if (_overlayStack.Count == 0)
      {
        UpdateCursorState();
        return;
      }

      var overlay = _overlayStack.Pop();
      overlay?.Hide();
      UpdateCursorState();
#if UNITY_EDITOR
      Debug.Log($"[UIOverlayStackManager] Popped overlay. New stack count: {_overlayStack.Count}");
#endif
    }

    public void Remove(IOverlayUI overlay)
    {
      if (overlay == null || _overlayStack.Count == 0)
        return;

      if (_overlayStack.Peek() == overlay)
      {
        Pop();
        return;
      }

      var temp = new Stack<IOverlayUI>();
      while (_overlayStack.Count > 0)
      {
        var current = _overlayStack.Pop();
        if (current != overlay)
          temp.Push(current);
      }

      while (temp.Count > 0)
        _overlayStack.Push(temp.Pop());

      overlay.Hide();
      UpdateCursorState();
    }

    void UpdateCursorState()
    {
      var hasOverlay = _overlayStack.Count > 0;
      Cursor.lockState = hasOverlay ? CursorLockMode.None : CursorLockMode.Locked;
      Cursor.visible = hasOverlay;
    }
  }
}

using System;
using Commons.Definitions;
using GamePlay.CallingInteraction;
using GamePlay.Camera;
using GamePlay.IngameObject;
using GamePlay.VisualElements;
using Infrastructure.UIStack;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.UIDocuments
{
  [RequireComponent(typeof(UIDocument))]
  public sealed class TelephoneUIController : MonoBehaviour, IOverlayUI
  {
    [Header("UI")]
    [SerializeField] UIDocument _uiDocument;

    [Header("3D Render")]
    [SerializeField] UnityEngine.Camera renderCamera;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] LayerMask raycastMask = ~0;
    [SerializeField] float raycastDistance = 10f;
    [SerializeField] bool debugLogging = false;

    TelephoneUI _telephoneUI;
    Image _renderArea;
    VisualElement _root;
    bool _isVisible;
    TelephoneButtonType _pressedKey;
    int _pressedPointerId = -1;

    public TelephoneUI TelephoneUI => _telephoneUI;
    public bool IsVisible => _isVisible;
    public event Action<TelephoneButtonType> ButtonPressed;
    public event Action<TelephoneButtonType> ButtonReleased;
    public event Action<TelephoneButtonType> ButtonClicked;

    void Reset()
    {
      _uiDocument = GetComponent<UIDocument>();
      // ResetState();
    }

    void OnEnable()
    {
      if (_uiDocument == null)
        _uiDocument = GetComponent<UIDocument>();

      EnsureUI();
      SetupRenderArea();
      Hide();

      if (debugLogging)
        ValidateTelephoneButtons();
    }

    void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();
      renderCamera = FindFirstObjectByType<TelephoneUIRenderCamera>()?.GetComponent<UnityEngine.Camera>();
      
      _uiDocument.sortingOrder = Defaults.SORT_ORDER_TELEPHONE_UI;
    }

    void OnDisable()
    {
      UnregisterRenderCallbacks();
    }

    void EnsureUI()
    {
      if (_uiDocument == null)
        return;

      _root = _uiDocument.rootVisualElement;
      if (_root == null)
        return;

      _telephoneUI = _root.Q<TelephoneUI>("telephone-ui");
      if (_telephoneUI == null)
      {
        _telephoneUI = new TelephoneUI();
        _root.Add(_telephoneUI);
      }

      _renderArea = _telephoneUI.RenderArea;
    }

    public void Show()
    {
      EnsureUI();
      if (_root == null)
        return;

      _root.style.display = DisplayStyle.Flex;
      _root.pickingMode = PickingMode.Position;
      _isVisible = true;
    }

    public void Hide()
    {
      EnsureUI();
      if (_root == null)
        return;

      _root.style.display = DisplayStyle.None;
      _root.pickingMode = PickingMode.Ignore;
      _isVisible = false;
    }

    public void ResetState()
    {
      ButtonPressed = null;
      ButtonReleased = null;
      ButtonClicked = null;
    }

    public void Toggle()
    {
      if (IsVisible)
        UIOverlayStackManager.Instance?.Remove(this);
      else
        UIOverlayStackManager.Instance?.Push(this);
    }

    void SetupRenderArea()
    {
      if (_renderArea == null)
        return;

      RegisterRenderCallbacks();

      if (renderCamera != null && renderTexture != null)
      {
        renderCamera.targetTexture = renderTexture;
        _renderArea.image = renderTexture;
        _renderArea.scaleMode = ScaleMode.ScaleToFit;
      }
    }

    void RegisterRenderCallbacks()
    {
      if (_renderArea == null)
        return;

      _renderArea.RegisterCallback<PointerDownEvent>(OnRenderPointerDown);
      _renderArea.RegisterCallback<PointerMoveEvent>(OnRenderPointerMove);
      _renderArea.RegisterCallback<PointerUpEvent>(OnRenderPointerUp);
      _renderArea.RegisterCallback<PointerLeaveEvent>(OnRenderPointerLeave);
    }

    void UnregisterRenderCallbacks()
    {
      if (_renderArea == null)
        return;

      _renderArea.UnregisterCallback<PointerDownEvent>(OnRenderPointerDown);
      _renderArea.UnregisterCallback<PointerMoveEvent>(OnRenderPointerMove);
      _renderArea.UnregisterCallback<PointerUpEvent>(OnRenderPointerUp);
      _renderArea.UnregisterCallback<PointerLeaveEvent>(OnRenderPointerLeave);
    }


    void OnRenderPointerDown(PointerDownEvent evt)
    {
      if (!IsVisible)
        return;

      if (_pressedPointerId != -1)
        return;

      _pressedPointerId = evt.pointerId;

      if (debugLogging)
        Debug.Log($"[TelephoneUIController] PointerDown local={evt.localPosition}");

      if (TryHitButton(evt.localPosition, out var button))
      {
        if (debugLogging)
          Debug.Log($"[TelephoneUIController] Hit button: {button.name} ({button.Key})");
        PressButton(button.Key);
      }
      else if (debugLogging)
      {
        Debug.Log("[TelephoneUIController] Raycast miss");
      }

      evt.StopPropagation();
    }

    void OnRenderPointerMove(PointerMoveEvent evt)
    {
      if (!IsVisible)
        return;

      if (_pressedPointerId != evt.pointerId)
        return;

      if (TryHitButton(evt.localPosition, out var button))
      {
        if (_pressedKey != button.Key)
        {
          ReleaseButton(_pressedKey);
          PressButton(button.Key);
        }
      }
      else
      {
        ReleaseButton(_pressedKey);
      }
    }

    void OnRenderPointerUp(PointerUpEvent evt)
    {
      if (!IsVisible)
        return;

      if (_pressedPointerId != evt.pointerId)
        return;

      var releasedKey = _pressedKey;
      ReleaseButton(_pressedKey);

      if (TryHitButton(evt.localPosition, out var button))
      {
        if (releasedKey == button.Key)
          ButtonClicked?.Invoke(releasedKey);
      }

      _pressedPointerId = -1;
      evt.StopPropagation();
    }

    void OnRenderPointerLeave(PointerLeaveEvent evt)
    {
      if (!IsVisible)
        return;

      if (_pressedPointerId == -1)
        return;

      ReleaseButton(_pressedKey);
    }

    void PressButton(TelephoneButtonType key)
    {
      _pressedKey = key;
      ButtonPressed?.Invoke(key);
    #if UNITY_EDITOR
      if (debugLogging)
        Debug.Log($"[TelephoneUIController] Pressed button: {key}");
    #endif
    }

    void ReleaseButton(TelephoneButtonType key)
    {
      ButtonReleased?.Invoke(key);
      if (_pressedKey == key)
        _pressedKey = default;
    }

    bool TryHitButton(Vector2 localPosition, out TelephoneButton button)
    {
      button = null;
      if (renderCamera == null || _renderArea == null)
      {
        if (debugLogging)
          Debug.Log("[TelephoneUIController] Missing renderCamera or renderArea");
        return false;
      }

      var rect = _renderArea.contentRect;
      if (rect.width <= 0f || rect.height <= 0f)
      {
        if (debugLogging)
          Debug.Log($"[TelephoneUIController] Invalid render rect: {rect}");
        return false;
      }

      var viewport = new Vector2(
        Mathf.Clamp01(localPosition.x / rect.width),
        Mathf.Clamp01(1f - (localPosition.y / rect.height))
      );

      var ray = renderCamera.ViewportPointToRay(viewport);
      if (!Physics.Raycast(ray, out var hit, raycastDistance, raycastMask, QueryTriggerInteraction.Ignore))
      {
        if (debugLogging)
          Debug.Log("[TelephoneUIController] Raycast returned no hit");
        return false;
      }

      button = hit.collider.GetComponentInParent<TelephoneButton>();
      if (debugLogging)
        Debug.Log($"[TelephoneUIController] Raycast hit: {hit.collider.name}, hasButton={(button != null)}");
      return button != null;
    }

    void ValidateTelephoneButtons()
    {
      var buttons = FindObjectsByType<TelephoneButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
      if (buttons == null || buttons.Length == 0)
      {
        Debug.LogWarning("[TelephoneUIController] No TelephoneButton found in scene.");
        return;
      }

      foreach (var button in buttons)
      {
        if (button == null)
          continue;

        var colliders = button.GetComponentsInChildren<Collider>(true);
        if (colliders == null || colliders.Length == 0)
        {
          Debug.LogWarning($"[TelephoneUIController] {button.name} has no Collider on self/children.");
          continue;
        }

        foreach (var col in colliders)
        {
          var layer = col.gameObject.layer;
          var inMask = (raycastMask.value & (1 << layer)) != 0;
          if (!inMask)
            Debug.LogWarning($"[TelephoneUIController] Collider {col.name} layer '{LayerMask.LayerToName(layer)}' is NOT in raycastMask.");
          if (col.isTrigger)
            Debug.LogWarning($"[TelephoneUIController] Collider {col.name} is Trigger (ignored by raycast).");
        }
      }
    }
  }
}

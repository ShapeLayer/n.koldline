using System;
using System.Collections;
using System.Threading.Tasks;
using Infrastructure.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.UIDocuments
{
  [RequireComponent(typeof(UIDocument))]
  public class TitleOverlayController : MonoBehaviour
  {
    public static TitleOverlayController Instance { get; private set; }

    [SerializeField] UIDocument _uiDocument;
    [SerializeField] VisualElement _screenRoot;
    [SerializeField] TitleOverlayElement _titleOverlay;

    Coroutine _instantRoutine;
    int _instantRequestId;

    void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
      }
      else if (Instance != this)
      {
        Destroy(gameObject);
        return;
      }

      _uiDocument = GetComponent<UIDocument>();
      CacheElements();
      HideImmediate();
    }

    void OnEnable()
    {
      if (_uiDocument == null)
        _uiDocument = GetComponent<UIDocument>();
      CacheElements();
    }

    void OnDisable()
    {
      StopInstantRoutine();
    }

    void CacheElements()
    {
      if (_uiDocument == null) return;
      var root = _uiDocument.rootVisualElement;
      _screenRoot = root?.Q<VisualElement>(name: "screen-root");
      _titleOverlay = root?.Q<TitleOverlayElement>(name: "title-overlay");
    }

    public void Show()
    {
      if (_screenRoot == null || _titleOverlay == null)
      {
        CacheElements();
        if (_screenRoot == null || _titleOverlay == null) return;
      }

      StopInstantRoutine();
      _screenRoot.style.display = DisplayStyle.Flex;
      _screenRoot.style.opacity = 1f;
    }

    public void Hide()
    {
      StopInstantRoutine();
      if (_screenRoot == null) return;
      _screenRoot.style.opacity = 0f;
      _screenRoot.style.display = DisplayStyle.None;
    }

    public void SetTitle(string content)
    {
      if (_titleOverlay == null)
      {
        CacheElements();
        if (_titleOverlay == null) return;
      }

      _titleOverlay.SetTitle(content);
    }

    public void InstantShow(string content, float durationSeconds)
    {
      if (_screenRoot == null || _titleOverlay == null)
      {
        CacheElements();
        if (_screenRoot == null || _titleOverlay == null) return;
      }

      StopInstantRoutine();
      var requestId = ++_instantRequestId;
      Show();
      SetTitle(content);
      _instantRoutine = StartCoroutine(HideAfterSeconds(Mathf.Max(0f, durationSeconds), requestId));
    }

    public async Task InstantShowAsync(string content, float durationSeconds)
    {
      if (_screenRoot == null || _titleOverlay == null)
      {
        CacheElements();
        if (_screenRoot == null || _titleOverlay == null) return;
      }

      StopInstantRoutine();
      var requestId = ++_instantRequestId;
      Show();
      SetTitle(content);

      if (durationSeconds > 0f)
        await Task.Delay(TimeSpan.FromSeconds(durationSeconds));

      if (requestId != _instantRequestId) return;
      Hide();
    }

    IEnumerator HideAfterSeconds(float durationSeconds, int requestId)
    {
      if (durationSeconds > 0f)
        yield return new WaitForSecondsRealtime(durationSeconds);
      if (requestId != _instantRequestId) yield break;
      Hide();
    }

    void StopInstantRoutine()
    {
      if (_instantRoutine == null) return;
      StopCoroutine(_instantRoutine);
      _instantRoutine = null;
    }

    void HideImmediate()
    {
      if (_screenRoot == null) return;
      _screenRoot.style.display = DisplayStyle.None;
      _screenRoot.style.opacity = 0f;
    }
  }
}

using System;
using System.Collections.Generic;
using Commons.Definitions;
using Infrastructure.Commons;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.UIDocuments
{
  /**
   * WARNING: This class receives and processes user input.
   * Make sure to handle input conflicts with other UI components.
   */
  [RequireComponent(typeof(UIDocument))]
  public class LocaleSelectorUIController : MonoBehaviour
  {
    [SerializeField] private static readonly List<LocaleData> _supports = new List<LocaleData>(Defaults.SUPPORT_LOCALES);

    private VisualElement _root;
    private VisualElement _screenRoot;
    private VisualElement _buttonListContainer;
    private List<Button> _spawnedButtons = new List<Button>();

    private int _focusedIndex = 0;
    private bool _hasSelected = false;
    private int _lastNavigationFrame = -1;

    public event Action<string> LocaleSelected;

    void OnEnable()
    {
      var uiDocument = GetComponent<UIDocument>();
      _root = uiDocument.rootVisualElement;
      _screenRoot = _root.Q<VisualElement>("screen-root");

      _buttonListContainer = _root.Q<VisualElement>("button-list");

      SpawnLocaleButtons();
      _hasSelected = false;
      FocusButtonAtIndex(_focusedIndex);
    }

    void SpawnLocaleButtons()
    {
      _buttonListContainer.Clear();
      _spawnedButtons.Clear();

      foreach (var each in _supports)
      {
        var btn = new Button
        {
          text = each.DisplayName
        };

        btn.AddToClassList("locale-button");
        btn.clicked += () => OnLocaleSelected(each.LocaleCode);
        btn.RegisterCallback<MouseEnterEvent>(_ =>
        {
          _focusedIndex = _supports.IndexOf(each);
          FocusButtonAtIndex(_focusedIndex);
        });
        btn.RegisterCallback<KeyDownEvent>(evt =>
        {
          if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
          {
            OnLocaleSelected(each.LocaleCode);
            evt.StopImmediatePropagation();
          }
        });

        _buttonListContainer.Add(btn);
        _spawnedButtons.Add(btn);
      }

      if (_spawnedButtons.Count > 0)
      {
        FocusButtonAtIndex(0);
      }
    }

    void FocusButtonAtIndex(int index)
    {
      if (!(0 <= index && index < _spawnedButtons.Count)) return;
      _spawnedButtons[index].Focus();
    }

    void OnLocaleSelected(string localeCode)
    {
      if (_hasSelected) return;
      _hasSelected = true;

#if UNITY_EDITOR && KOLDLINE_DEBUG_LOCALE_SELECTOR
      Debug.Log($"Locale selected: {localeCode}");
#endif
      
      LocaleSelected?.Invoke(localeCode);

      // Prevent further input
      Disable();
    }

    public void Enable()
    {
      enabled = true;
      _hasSelected = false;
      _screenRoot.style.display = DisplayStyle.Flex;
      _root.style.display = DisplayStyle.Flex;
      _root.style.opacity = 1;
      _screenRoot.style.opacity = 1;
    }

    public void Disable()
    {
      _screenRoot.style.opacity = 0;
      _root.style.opacity = 0;
      _screenRoot.style.display = DisplayStyle.None;
      _root.style.display = DisplayStyle.None;
      enabled = false;
    }

    void Update()
    {
      if (_spawnedButtons.Count == 0) return;
      if (Time.frameCount == _lastNavigationFrame) return;

      if
      (
        Input.GetKeyDown(KeyCode.DownArrow) ||
        Input.GetKeyDown(KeyCode.RightArrow)
      )
      {
        _focusedIndex = (_focusedIndex + 1) % _spawnedButtons.Count;
        FocusButtonAtIndex(_focusedIndex);
        _lastNavigationFrame = Time.frameCount;
      }
      else if (
        Input.GetKeyDown(KeyCode.UpArrow) ||
        Input.GetKeyDown(KeyCode.LeftArrow)
      )
      {
        _focusedIndex = (_focusedIndex - 1 + _spawnedButtons.Count) % _spawnedButtons.Count;
        FocusButtonAtIndex(_focusedIndex);
        _lastNavigationFrame = Time.frameCount;
      }
      else if (
        false
      ) { }
    }
  }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Infrastructure.UIDocuments
{
  [RequireComponent(typeof(UIDocument))]
  public class KpDialogueController : MonoBehaviour
  {
    [SerializeField] float _characterIntervalSeconds = 0.03f;

    [SerializeField] UIDocument _uiDocument;
    [SerializeField] VisualElement _screenRoot;
    [SerializeField] KpDialogueElement _dialogueElement;

    CancellationTokenSource _typingCts;
    TaskCompletionSource<bool> _completionTcs;

    [SerializeField] bool _isTyping;
    [SerializeField] bool _typingCompleted;
    [SerializeField] bool _isActive;
    [SerializeField] string _primaryText = string.Empty;
    [SerializeField] string _secondaryText = string.Empty;

    void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();
      CacheElements();
      HideImmediate();
    }

    void OnEnable()
    {
      if (_uiDocument == null)
        _uiDocument = GetComponent<UIDocument>();
      CacheElements();
      HideImmediate();
    }

    void CacheElements()
    {
      if (_uiDocument == null) return;
      var root = _uiDocument.rootVisualElement;
      _screenRoot = root?.Q<VisualElement>(name: "screen-root");
      _dialogueElement = root?.Q<KpDialogueElement>(name: "kp-dialogue-holder");
    }

    public Task Show()
    {
      if (_screenRoot == null || _dialogueElement == null)
        throw new InvalidOperationException("KpDialogueController is not initialized");

      _isActive = true;
      _screenRoot.style.display = DisplayStyle.Flex;
      _screenRoot.style.opacity = 0f;
      _dialogueElement.ResetText();
      _dialogueElement.SetHintVisible(false);
      _typingCompleted = false;
      return Task.CompletedTask;
    }

    public Task Hide()
    {
      CancelTyping();
      _completionTcs?.TrySetResult(true);
      _isTyping = false;
      _isActive = false;
      _typingCompleted = false;
      if (_screenRoot != null)
      {
        _screenRoot.style.opacity = 0f;
        _screenRoot.style.display = DisplayStyle.None;
      }
      _dialogueElement?.ResetText();
      _completionTcs = null;
      return Task.CompletedTask;
    }

    public bool IsActive => _isActive;
    public bool IsTyping => _isTyping;
    public bool IsWaitingForAdvance => _typingCompleted && _completionTcs != null;

    public async Task PlayDialogue(string primary, string secondary, string presetPrimary = null, string presetSecondary = null)
    {
      if (_screenRoot == null || _dialogueElement == null)
        throw new InvalidOperationException("KpDialogueController is not initialized");

      _screenRoot.style.display = DisplayStyle.Flex;
      _screenRoot.style.opacity = 1f;
      _dialogueElement.ResetText();
      _dialogueElement.SetHintVisible(false);

      var presetPrimaryText = presetPrimary ?? string.Empty;
      var presetSecondaryText = presetSecondary ?? string.Empty;
      _dialogueElement.PrimaryLabel.text = presetPrimaryText;
      _dialogueElement.SecondaryLabel.text = presetSecondaryText;

      CancelTyping();
      _completionTcs = new TaskCompletionSource<bool>();
      _typingCts = new CancellationTokenSource();
      _isTyping = true;
      _typingCompleted = false;
      _primaryText = primary ?? string.Empty;
      _secondaryText = secondary ?? string.Empty;

      var primaryTask = TypeTextAsync(_primaryText, presetPrimaryText, _dialogueElement.PrimaryLabel, _typingCts.Token);
      var secondaryTask = TypeTextAsync(_secondaryText, presetSecondaryText, _dialogueElement.SecondaryLabel, _typingCts.Token);

      await Task.WhenAll(primaryTask, secondaryTask);

      _isTyping = false;
      _typingCompleted = true;
      _dialogueElement.SetHintVisible(true);
      await _completionTcs.Task;
    }

    public void HandleInteraction()
    {
      if (!_isActive) return;

      if (_isTyping)
      {
        SkipTyping();
        return;
      }

      if (_typingCompleted)
      {
        _typingCompleted = false;
        _dialogueElement.SetHintVisible(false);
        _completionTcs?.TrySetResult(true);
      }
    }

    async Task TypeTextAsync(string fullText, string presetText, Label label, CancellationToken token)
    {
      if (label == null) return;

      label.text = presetText ?? string.Empty;
      if (string.IsNullOrEmpty(fullText)) return;

      int startIndex = label.text.Length;
      if (startIndex > fullText.Length)
      {
        label.text = fullText;
        startIndex = fullText.Length;
      }

      for (int i = startIndex; i < fullText.Length; i++)
      {
        if (token.IsCancellationRequested) break;
        label.text += fullText[i];
        await Task.Delay(TimeSpan.FromSeconds(_characterIntervalSeconds));
      }

      if (token.IsCancellationRequested)
      {
        label.text = fullText;
      }
    }

    void SkipTyping()
    {
      if (_dialogueElement == null) return;

      CancelTyping();
      _dialogueElement.PrimaryLabel.text = _primaryText;
      _dialogueElement.SecondaryLabel.text = _secondaryText;
      _isTyping = false;
      _typingCompleted = true;
      _dialogueElement.SetHintVisible(true);
    }

    void CancelTyping()
    {
      if (_typingCts != null && !_typingCts.IsCancellationRequested)
      {
        _typingCts.Cancel();
        _typingCts.Dispose();
      }
      _typingCts = null;
      _isTyping = false;
    }

    void HideImmediate()
    {
      if (_screenRoot == null) return;
      _screenRoot.style.display = DisplayStyle.None;
      _screenRoot.style.opacity = 0f;
    }
  }
}

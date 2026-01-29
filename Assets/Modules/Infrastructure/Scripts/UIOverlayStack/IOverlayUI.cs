namespace Infrastructure.UIStack
{
  public interface IOverlayUI
  {
    bool IsVisible { get; }
    void Show();
    void Hide();
  }
}

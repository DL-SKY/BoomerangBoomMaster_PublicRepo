public interface IUiController
{
    bool IsShowed { get; set; }
    bool IsInit { get; set; }

    void OnShow();
    void OnHide();

    void OnDisableObject();
}

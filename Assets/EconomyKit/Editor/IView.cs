using UnityEngine;

public interface IView
{
    void Show();
    void Hide();
    void Draw(Rect position);
}

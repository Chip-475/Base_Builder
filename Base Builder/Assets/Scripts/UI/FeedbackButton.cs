using UnityEngine;
using UnityEngine.UI;

public class FeedbackButton : MonoBehaviour
{
    [SerializeField] Button button;

    private void Awake()
    {
        button.onClick.AddListener(() => Application.OpenURL("https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwjCiq6vq_mXAxVxhv0HHZRcA-kQ78AJegQIJBAB&url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DdQw4w9WgXcQ&usg=AOvVaw0aHtehaphMhOCAkCydRLZU&opi=89978449"));
    }
}

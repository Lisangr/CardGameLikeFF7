using UnityEngine;
using UnityEngine.UI;
using YG;

public class MyGames : MonoBehaviour
{
    // Ссылка на кнопку в редакторе
    public Button button;

    // URL для открытия
    private string urlRU = "https://yandex.ru/games/developer/87388";
    private string urlEN = "https://yandex.com/games/developer/87388";

    void Start()
    {
        // Привязка метода к событию нажатия кнопки
        if (button != null)
        {
            button.onClick.AddListener(OpenURL);
        }
    }

    // Метод для открытия URL
    void OpenURL()
    {
        if (YandexGame.EnvironmentData.domain == "ru")
        {
            Application.OpenURL(urlRU);
        }
        else
        {
            Application.OpenURL(urlEN);
        }
    }
}

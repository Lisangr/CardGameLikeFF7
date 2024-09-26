using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ClearCardsButton : MonoBehaviour
{
    private const string CardsFileName = "savedCards.json"; // Имя файла

    void Start()
    {
        // Находим кнопку и назначаем ей действие
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ClearCardData);
        }
    }

    // Метод, который вызывается при нажатии кнопки
    public void ClearCardData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, CardsFileName);

        // Проверяем, существует ли файл
        if (File.Exists(filePath))
        {
            // Записываем пустой список в файл
            File.WriteAllText(filePath, JsonUtility.ToJson(new Serialization<List<CardData>>(new List<CardData>()), true));
            Debug.Log("JSON-файл очищен.");
        }
        else
        {
            Debug.LogWarning("Файл не найден, нечего очищать.");
        }
    }
}

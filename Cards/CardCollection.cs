using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class CardCollection : MonoBehaviour
{
    public GameObject cardUIPrefab;
    public Transform gridParent;

    private const string CardsFileName = "savedCards.json";
    private List<CardsForTakeCardsAndCollection> collectedCards = new List<CardsForTakeCardsAndCollection>();

    private void OnEnable()
    {
        LoadCards();
    }

    private void LoadCards()
    {
        if (!File.Exists(GetFilePath()))
        {
            Debug.LogWarning("Нет сохранённых карт для отображения в коллекции.");
            return;
        }

        string json = File.ReadAllText(GetFilePath());
        var cardDataList = JsonUtility.FromJson<Serialization<List<CardData>>>(json).target;

        ClearCollection();

        Sprite[] loadedCardImages = Resources.LoadAll<Sprite>("CardImages");

        // Естественная сортировка по имени (учитывая числа в именах файлов)
        loadedCardImages = loadedCardImages.OrderBy(s => s.name, new NaturalStringComparer()).ToArray();

        foreach (var cardData in cardDataList)
        {
            GameObject newCardUI = Instantiate(cardUIPrefab, gridParent);
            CardsForTakeCardsAndCollection cardComponent = newCardUI.GetComponent<CardsForTakeCardsAndCollection>();

            if (cardComponent == null)
            {
                Debug.LogError("Компонент 'Card' не найден на созданной карте.");
                continue;
            }

            Sprite sprite = System.Array.Find(loadedCardImages, s => s.name == cardData.spriteName);
            if (sprite != null)
            {
                cardComponent.cardImage.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Изображение с именем {cardData.spriteName} не найдено.");
            }

            cardComponent.InitializeCard(
                cardData.leftValue,
                cardData.rightValue,
                cardData.topValue,
                cardData.bottomValue,
                cardData.isPlayer1Card,
                cardData.spriteName
            );
        }

        Debug.Log($"Загружено {cardDataList.Count} карт в коллекцию.");
    }

    private void ClearCollection()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        collectedCards.Clear();
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CardsFileName);
    }
}

// Реализация естественной сортировки строк
public class NaturalStringComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        int ix = 0, iy = 0;
        while (ix < x.Length && iy < y.Length)
        {
            if (char.IsDigit(x[ix]) && char.IsDigit(y[iy]))
            {
                var numX = GetNumber(x, ref ix);
                var numY = GetNumber(y, ref iy);
                int result = numX.CompareTo(numY);
                if (result != 0)
                {
                    return result;
                }
            }
            else
            {
                int result = x[ix].CompareTo(y[iy]);
                if (result != 0)
                {
                    return result;
                }
                ix++;
                iy++;
            }
        }

        return x.Length - y.Length;
    }

    private int GetNumber(string str, ref int index)
    {
        int number = 0;
        while (index < str.Length && char.IsDigit(str[index]))
        {
            number = number * 10 + (str[index] - '0');
            index++;
        }
        return number;
    }
}



/*
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CardCollection : MonoBehaviour
{
    public GameObject cardUIPrefab; // Префаб карты UI
    public Transform gridParent; // Родительский объект для карт

    private const string CardsFileName = "savedCards.json"; // Файл для сохранённых карт
    private List<CardsForTakeCardsAndCollection> collectedCards = new List<CardsForTakeCardsAndCollection>();

    private void OnEnable()
    {
        LoadCards();
    }
    private void LoadCards()
    {
        if (!File.Exists(GetFilePath()))
        {
            Debug.LogWarning("Нет сохранённых карт для отображения в коллекции.");
            return;
        }

        string json = File.ReadAllText(GetFilePath());
        var cardDataList = JsonUtility.FromJson<Serialization<List<CardData>>>(json).target;

        ClearCollection();

        Sprite[] loadedCardImages = Resources.LoadAll<Sprite>("CardImages");

        foreach (var cardData in cardDataList)
        {
            GameObject newCardUI = Instantiate(cardUIPrefab, gridParent);
            CardsForTakeCardsAndCollection cardComponent = newCardUI.GetComponent<CardsForTakeCardsAndCollection>();

            if (cardComponent == null)
            {
                Debug.LogError("Компонент 'Card' не найден на созданной карте.");
                continue;
            }

            // Загрузка спрайта по имени
            Sprite sprite = System.Array.Find(loadedCardImages, s => s.name == cardData.spriteName);
            if (sprite != null)
            {
                cardComponent.cardImage.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Изображение с именем {cardData.spriteName} не найдено.");
            }

            cardComponent.InitializeCard(
                cardData.leftValue,
                cardData.rightValue,
                cardData.topValue,
                cardData.bottomValue,
                cardData.isPlayer1Card,
                cardData.imageIndex
            );
        }

        Debug.Log($"Загружено {cardDataList.Count} карт в коллекцию.");
    }
    // Удаление всех текущих карт из коллекции
    private void ClearCollection()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        collectedCards.Clear();
    }   

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CardsFileName);
    }
    
}*/






/*
// Загрузка сохранённых карт
private void LoadCards()
{
    if (!File.Exists(GetFilePath()))
    {
        Debug.LogWarning("Нет сохранённых карт для отображения в коллекции.");
        return;
    }

    string json = File.ReadAllText(GetFilePath());
    var cardDataList = JsonUtility.FromJson<Serialization<List<CardData>>>(json).target;

    ClearCollection();

    foreach (var cardData in cardDataList)
    {
        GameObject newCardUI = Instantiate(cardUIPrefab, gridParent); // Создаём UI-карту

        if (newCardUI == null)
        {
            Debug.LogError("Не удалось создать UI-элемент карты.");
            continue;
        }

        CardsForTakeCardsAndCollection cardComponent = newCardUI.GetComponent<CardsForTakeCardsAndCollection>();

        if (cardComponent == null)
        {
            Debug.LogError("Компонент 'Card' не найден на созданной карте.");
            continue;
        }

        // Загружаем изображение карты из папки Resources/CardImages
        Sprite[] loadedCardImages = Resources.LoadAll<Sprite>("CardImages");

        if (cardData.imageIndex >= 0 && cardData.imageIndex < loadedCardImages.Length)
        {
            cardComponent.cardImage.sprite = loadedCardImages[cardData.imageIndex];
        }
        else
        {
            Debug.LogError($"Неверный индекс изображения: {cardData.imageIndex}");
        }

        // Заполняем карту данными без генерации новых значений
        cardComponent.InitializeCard(
            cardData.leftValue,
            cardData.rightValue,
            cardData.topValue,
            cardData.bottomValue,
            cardData.isPlayer1Card,
            cardData.imageIndex // Используем сохранённый индекс изображения
        );

        collectedCards.Add(cardComponent);

        Debug.Log($"Загруженная карта: left={cardData.leftValue}, right={cardData.rightValue}, " +
            $"top={cardData.topValue}, bottom={cardData.bottomValue}, isPlayer1Card={cardData.isPlayer1Card}, " +
            $"imageIndex={cardData.imageIndex}");
    }

    Debug.Log($"Загружено {collectedCards.Count} карт в коллекцию.");
}
*/
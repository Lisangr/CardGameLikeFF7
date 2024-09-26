using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class CardManagerToo : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<CardsForTakeCardsAndCollection> cardList;  // Все карты
    public List<CardsForTakeCardsAndCollection> playerCards;  // Карты игрока
    public List<Card> enemyCards;   // Карты врага
    private const string CardsFileName = "savedBattleCards.json";

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        LoadBattleCards(); // Загружаем карты из сохранённого файла

        if (cardList == null || cardList.Count == 0)
        {
            Debug.LogError("Card list is empty! Make sure cards are loaded from CardCollection.");
            return;
        }

        //SpawnCardsInGrid();
        CategorizeDistributedCards();  // Категоризация карт после их раздачи
    }
    private void LoadBattleCards()
    {
        string filePath = GetFilePath();
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Файл сохранённых карт не найден.");
            return;
        }

        string json = File.ReadAllText(filePath);
        var cardDataList = JsonUtility.FromJson<Serialization<List<CardData>>>(json).target;

        ClearGrid();

        Sprite[] loadedCardImages = Resources.LoadAll<Sprite>("CardImages");

        foreach (var cardData in cardDataList)
        {
            GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);
            CardsForTakeCardsAndCollection cardDisplay = cardObject.GetComponent<CardsForTakeCardsAndCollection>();

            if (cardDisplay != null)
            {
                // Находим спрайт по имени
                Sprite sprite = System.Array.Find(loadedCardImages, s => s.name == cardData.spriteName);

                if (sprite != null)
                {
                    cardDisplay.cardImage.sprite = sprite;
                    Debug.Log($"Карта загружена: {cardData.spriteName}");
                }
                else
                {
                    Debug.LogError($"Изображение с именем {cardData.spriteName} не найдено.");
                }

                cardDisplay.InitializeCard(
                    cardData.leftValue,
                    cardData.rightValue,
                    cardData.topValue,
                    cardData.bottomValue,
                    cardData.isPlayer1Card,
                    cardData.spriteName  // Используем только имя спрайта
                );

                cardList.Add(cardDisplay);
            }
        }

        Debug.Log($"Загружено {cardDataList.Count} карт из файла.");
    }

    // Метод для очистки сетки перед загрузкой новых карт
    private void ClearGrid()
    {
        foreach (Transform child in gridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        cardList.Clear();
    }

    // Метод для получения пути к файлу сохранения
    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CardsFileName);
    }
    
    void SpawnCardsInGrid()
    {
        if (cardList == null || cardList.Count == 0)
        {
            Debug.LogWarning("Card list is empty!");
            return;
        }

        for (int i = 0; i < cardList.Count; i++)
        {
            SpawnCard(i);
        }
    }

    void SpawnCard(int index)
    {
        GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);
        CardsForTakeCardsAndCollection cardDisplay = cardObject.GetComponent<CardsForTakeCardsAndCollection>();

        if (cardDisplay != null && cardList != null && index < cardList.Count)
        {
            var cardData = cardList[index];

            // Загружаем спрайт по имени
            Sprite sprite = System.Array.Find(Resources.LoadAll<Sprite>("CardImages"), s => s.name == cardData.spriteName);

            if (sprite != null)
            {
                cardDisplay.cardImage.sprite = sprite;
                Debug.Log($"Карта с именем {cardData.spriteName} создана.");
            }
            else
            {
                Debug.LogError($"Изображение с именем {cardData.spriteName} не найдено.");
            }

            cardDisplay.InitializeCard(
                cardData.leftValue,
                cardData.rightValue,
                cardData.topValue,
                cardData.bottomValue,
                cardData.isPlayer1Card,
                cardData.spriteName  // Инициализация по имени спрайта
            );
        }
        else
        {
            Debug.LogWarning("CardDisplay component missing or index out of range!");
        }
    }


    void CategorizeDistributedCards()
    {
        playerCards = new List<CardsForTakeCardsAndCollection>();
        enemyCards = new List<Card>();

        foreach (Transform child in gridLayoutGroup.transform)
        {
            CardsForTakeCardsAndCollection card = child.GetComponent<CardsForTakeCardsAndCollection>();
            Card card1 = child.GetComponent<Card>();

            if (card != null)
            {
                // Разделяем карты по владельцу
                if (card.isPlayer1Card)
                {
                    playerCards.Add(card);
                }
                else if (card1 != null) // исправляем дублирование
                {
                    enemyCards.Add(card1);
                }
            }
        }

        Debug.Log($"Игроку выдано {playerCards.Count} карт.");
        Debug.Log($"Врагу выдано {enemyCards.Count} карт.");
    }
    
    public Card GetEnemyCard()
    {
        if (enemyCards.Count > 0)
        {
            var card = enemyCards[enemyCards.Count - 1];
            enemyCards.RemoveAt(enemyCards.Count - 1);

            Debug.Log($"Врагу выдана карта: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}. Осталось {enemyCards.Count} карт.");
            return card;
        }
        Debug.LogWarning("No more cards available for enemy.");
        return null;
    }   
}












/*
    // Метод загрузки карт из сохранённого файла
    private void LoadBattleCards()
    {
        string filePath = GetFilePath();
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Файл сохранённых карт не найден.");
            return;
        }

        string json = File.ReadAllText(filePath);
        var cardDataList = JsonUtility.FromJson<Serialization<List<CardData>>>(json).target;

        ClearGrid();

        foreach (var cardData in cardDataList)
        {
            GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);
            CardsForTakeCardsAndCollection cardDisplay = cardObject.GetComponent<CardsForTakeCardsAndCollection>();

            if (cardDisplay != null)
            {
                cardDisplay.InitializeCard(
                    cardData.leftValue,
                    cardData.rightValue,
                    cardData.topValue,
                    cardData.bottomValue,
                    cardData.isPlayer1Card,
                    cardData.imageIndex
                );
                cardList.Add(cardDisplay);
                Debug.Log($"Карта загружена: L:{cardData.leftValue}, R:{cardData.rightValue}, T:{cardData.topValue}," +
                    $" B:{cardData.bottomValue}, ImageIndex:{cardData.imageIndex}");
            }
        }

        Debug.Log($"Загружено {cardDataList.Count} карт из файла.");
    }
    */
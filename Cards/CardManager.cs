using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<Card> cardList;  // Все карты
    public List<Card> playerCards;  // Карты игрока
    public List<Card> enemyCards;   // Карты врага

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        if (cardList == null || cardList.Count == 0)
        {
            InitializeCardList();  // Заполняем колоду карт
        }

        SpawnCardsInGrid();
        CategorizeDistributedCards();  // Категоризация карт после их раздачи
    }

    // Метод для инициализации карт в колоде
    void InitializeCardList()
    {
        cardList = new List<Card>();

        // Генерируем 20 карт с рандомными значениями (чтобы было достаточно для игры)
        for (int i = 0; i < 20; i++)
        {
            GameObject cardObject = Instantiate(cardUIPrefab);
            Card newCard = cardObject.GetComponent<Card>();

            // Устанавливаем случайные значения для карты
            newCard.leftValue = Random.Range(1, 10);
            newCard.rightValue = Random.Range(1, 10);
            newCard.topValue = Random.Range(1, 10);
            newCard.bottomValue = Random.Range(1, 10);

            // Устанавливаем владельца карты (например, чередуем между игроком и врагом)
            newCard.isPlayer1Card = (i % 2 == 0);  // Чередуем карты между игроком и врагом

            // Добавляем карту в список
            cardList.Add(newCard);
        }

        Debug.Log($"Инициализировано {cardList.Count} карт.");
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

        Card cardDisplay = cardObject.GetComponent<Card>();
        if (cardDisplay != null)
        {
            cardDisplay.SetCard();
        }
        else
        {
            Debug.LogWarning("CardDisplay component missing on card prefab!");
        }
    }

    // Метод для категоризации карт после раздачи
    void CategorizeDistributedCards()
    {
        playerCards = new List<Card>();
        enemyCards = new List<Card>();

        foreach (Transform child in gridLayoutGroup.transform)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                // Разделяем карты по владельцу
                if (card.isPlayer1Card)
                {
                    playerCards.Add(card);
                }
                else
                {
                    enemyCards.Add(card);
                }
            }
        }

        // Логирование карт для игрока и врага
        Debug.Log($"Игроку выдано {playerCards.Count} карт.");
        Debug.Log($"Врагу выдано {enemyCards.Count} карт.");

        foreach (Card playerCard in playerCards)
        {
            Debug.Log($"Карта игрока: L:{playerCard.leftValue}, R:{playerCard.rightValue}, T:{playerCard.topValue}, B:{playerCard.bottomValue}");
        }

        foreach (Card enemyCard in enemyCards)
        {
            Debug.Log($"Карта врага: L:{enemyCard.leftValue}, R:{enemyCard.rightValue}, T:{enemyCard.topValue}, B:{enemyCard.bottomValue}");
        }
    }

    // Новый метод для выдачи карты врагу
    public Card GetEnemyCard()
    {
        if (enemyCards.Count > 0)
        {
            Card card = enemyCards[enemyCards.Count - 1];
            enemyCards.RemoveAt(enemyCards.Count - 1);

            Debug.Log($"Врагу выдана карта: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}. Осталось {enemyCards.Count} карт.");
            return card;
        }
        Debug.LogWarning("No more cards available for enemy.");
        return null;
    }
}
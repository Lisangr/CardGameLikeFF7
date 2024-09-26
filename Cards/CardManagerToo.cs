using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class CardManagerToo : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<CardsForTakeCardsAndCollection> cardList;  // ��� �����
    public List<CardsForTakeCardsAndCollection> playerCards;  // ����� ������
    public List<Card> enemyCards;   // ����� �����
    private const string CardsFileName = "savedBattleCards.json";

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        LoadBattleCards(); // ��������� ����� �� ����������� �����

        if (cardList == null || cardList.Count == 0)
        {
            Debug.LogError("Card list is empty! Make sure cards are loaded from CardCollection.");
            return;
        }

        //SpawnCardsInGrid();
        CategorizeDistributedCards();  // ������������� ���� ����� �� �������
    }
    private void LoadBattleCards()
    {
        string filePath = GetFilePath();
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("���� ���������� ���� �� ������.");
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
                // ������� ������ �� �����
                Sprite sprite = System.Array.Find(loadedCardImages, s => s.name == cardData.spriteName);

                if (sprite != null)
                {
                    cardDisplay.cardImage.sprite = sprite;
                    Debug.Log($"����� ���������: {cardData.spriteName}");
                }
                else
                {
                    Debug.LogError($"����������� � ������ {cardData.spriteName} �� �������.");
                }

                cardDisplay.InitializeCard(
                    cardData.leftValue,
                    cardData.rightValue,
                    cardData.topValue,
                    cardData.bottomValue,
                    cardData.isPlayer1Card,
                    cardData.spriteName  // ���������� ������ ��� �������
                );

                cardList.Add(cardDisplay);
            }
        }

        Debug.Log($"��������� {cardDataList.Count} ���� �� �����.");
    }

    // ����� ��� ������� ����� ����� ��������� ����� ����
    private void ClearGrid()
    {
        foreach (Transform child in gridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        cardList.Clear();
    }

    // ����� ��� ��������� ���� � ����� ����������
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

            // ��������� ������ �� �����
            Sprite sprite = System.Array.Find(Resources.LoadAll<Sprite>("CardImages"), s => s.name == cardData.spriteName);

            if (sprite != null)
            {
                cardDisplay.cardImage.sprite = sprite;
                Debug.Log($"����� � ������ {cardData.spriteName} �������.");
            }
            else
            {
                Debug.LogError($"����������� � ������ {cardData.spriteName} �� �������.");
            }

            cardDisplay.InitializeCard(
                cardData.leftValue,
                cardData.rightValue,
                cardData.topValue,
                cardData.bottomValue,
                cardData.isPlayer1Card,
                cardData.spriteName  // ������������� �� ����� �������
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
                // ��������� ����� �� ���������
                if (card.isPlayer1Card)
                {
                    playerCards.Add(card);
                }
                else if (card1 != null) // ���������� ������������
                {
                    enemyCards.Add(card1);
                }
            }
        }

        Debug.Log($"������ ������ {playerCards.Count} ����.");
        Debug.Log($"����� ������ {enemyCards.Count} ����.");
    }
    
    public Card GetEnemyCard()
    {
        if (enemyCards.Count > 0)
        {
            var card = enemyCards[enemyCards.Count - 1];
            enemyCards.RemoveAt(enemyCards.Count - 1);

            Debug.Log($"����� ������ �����: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}. �������� {enemyCards.Count} ����.");
            return card;
        }
        Debug.LogWarning("No more cards available for enemy.");
        return null;
    }   
}












/*
    // ����� �������� ���� �� ����������� �����
    private void LoadBattleCards()
    {
        string filePath = GetFilePath();
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("���� ���������� ���� �� ������.");
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
                Debug.Log($"����� ���������: L:{cardData.leftValue}, R:{cardData.rightValue}, T:{cardData.topValue}," +
                    $" B:{cardData.bottomValue}, ImageIndex:{cardData.imageIndex}");
            }
        }

        Debug.Log($"��������� {cardDataList.Count} ���� �� �����.");
    }
    */
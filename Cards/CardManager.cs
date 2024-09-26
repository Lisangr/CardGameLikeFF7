using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<Card> cardList;  // ��� �����
    public List<Card> playerCards;  // ����� ������
    public List<Card> enemyCards;   // ����� �����

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        if (cardList == null || cardList.Count == 0)
        {
            InitializeCardList();  // ��������� ������ ����
        }

        SpawnCardsInGrid();
        CategorizeDistributedCards();  // ������������� ���� ����� �� �������
    }

    // ����� ��� ������������� ���� � ������
    void InitializeCardList()
    {
        cardList = new List<Card>();

        // ���������� 20 ���� � ���������� ���������� (����� ���� ���������� ��� ����)
        for (int i = 0; i < 20; i++)
        {
            GameObject cardObject = Instantiate(cardUIPrefab);
            Card newCard = cardObject.GetComponent<Card>();

            // ������������� ��������� �������� ��� �����
            newCard.leftValue = Random.Range(1, 10);
            newCard.rightValue = Random.Range(1, 10);
            newCard.topValue = Random.Range(1, 10);
            newCard.bottomValue = Random.Range(1, 10);

            // ������������� ��������� ����� (��������, �������� ����� ������� � ������)
            newCard.isPlayer1Card = (i % 2 == 0);  // �������� ����� ����� ������� � ������

            // ��������� ����� � ������
            cardList.Add(newCard);
        }

        Debug.Log($"���������������� {cardList.Count} ����.");
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

    // ����� ��� ������������� ���� ����� �������
    void CategorizeDistributedCards()
    {
        playerCards = new List<Card>();
        enemyCards = new List<Card>();

        foreach (Transform child in gridLayoutGroup.transform)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                // ��������� ����� �� ���������
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

        // ����������� ���� ��� ������ � �����
        Debug.Log($"������ ������ {playerCards.Count} ����.");
        Debug.Log($"����� ������ {enemyCards.Count} ����.");

        foreach (Card playerCard in playerCards)
        {
            Debug.Log($"����� ������: L:{playerCard.leftValue}, R:{playerCard.rightValue}, T:{playerCard.topValue}, B:{playerCard.bottomValue}");
        }

        foreach (Card enemyCard in enemyCards)
        {
            Debug.Log($"����� �����: L:{enemyCard.leftValue}, R:{enemyCard.rightValue}, T:{enemyCard.topValue}, B:{enemyCard.bottomValue}");
        }
    }

    // ����� ����� ��� ������ ����� �����
    public Card GetEnemyCard()
    {
        if (enemyCards.Count > 0)
        {
            Card card = enemyCards[enemyCards.Count - 1];
            enemyCards.RemoveAt(enemyCards.Count - 1);

            Debug.Log($"����� ������ �����: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}. �������� {enemyCards.Count} ����.");
            return card;
        }
        Debug.LogWarning("No more cards available for enemy.");
        return null;
    }
}
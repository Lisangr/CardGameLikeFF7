using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<Card> cardList;

    private AIPlayer aiPlayer;

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        aiPlayer = FindObjectOfType<AIPlayer>();

        if (aiPlayer == null)
        {
            Debug.LogError("AIPlayer �� ������ �� �����!");
        }

        if (cardList == null || cardList.Count == 0)
        {
            cardList = new List<Card>();
        }

        SpawnCardsInGrid();

        if (aiPlayer != null)
        {
            SpawnAICards();
        }
    }

    void SpawnCardsInGrid()
    {
        // ������� ����� 9 ����
        for (int i = 0; i < 9; i++)
        {
            SpawnCard(i);
        }

        Debug.Log($"������� � ��������� {cardList.Count} ���� � �����.");
    }

    void SpawnCard(int index)
    {
        // ������� ������ ����� � ��������� �� UI
        GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);

        // �������� ��������� Card � ������������� ��������� ��������
        Card cardDisplay = cardObject.GetComponent<Card>();

        if (cardDisplay != null)
        {
            // ������ ��������� �������� ��� �����
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = true; // ��� ����� ������

            // ��������� ����� � ������ ���� �� ������ �� UI
            cardList.Add(cardDisplay);

            // ��������� ����� �� UI
            cardDisplay.SetCard();
        }
        else
        {
            Debug.LogWarning("��������� Card �� ������ �� ������� �����!");
        }
    }

    // В методе SpawnAICards в CardManager
void SpawnAICards()
{
    // Получаем эталонный размер и масштаб из карт игрока
    Vector3 playerCardScale = Vector3.one;
    if (cardList.Count > 0)
    {
        playerCardScale = cardList[0].transform.localScale;
    }
    
    // Создаем 9 карт для AI
    for (int i = 0; i < 9; i++)
    {
        GameObject cardObject = Instantiate(cardUIPrefab);
        Card cardDisplay = cardObject.GetComponent<Card>();

        if (cardDisplay != null)
        {
            // Устанавливаем такой же масштаб как у карт игрока
            cardObject.transform.localScale = playerCardScale;
            
            // Задаем случайные значения
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);
            cardDisplay.isPlayer1Card = false; // Это карта AI
            
            // Обновляем текст на карте
            cardDisplay.SetCard();
            
            // Добавляем карту AI игроку
            aiPlayer.AddCard(cardDisplay);
        }
    }
}
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("UI Grid Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cardUIPrefab;
    public List<Card> cardList;  // ��� �����


    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        if (cardList == null || cardList.Count == 0)
        {
            cardList = new List<Card>();  // �������������� ������ ������ ����
        }

        SpawnCardsInGrid();
    }

    // ����� ��� ������ ���� �� UI
    void SpawnCardsInGrid()
    {
        // ������� ����� 9 ����
        for (int i = 0; i < 9; i++)
        {
            SpawnCard(i);
        }

        Debug.Log($"������� � ��������� {cardList.Count} ���� � ����.");
    }

    void SpawnCard(int index)
    {
        // ������� ������ ����� � ��������� �� UI
        GameObject cardObject = Instantiate(cardUIPrefab, gridLayoutGroup.transform);

        // �������� ��������� Card � ����������� ��������� ��������
        Card cardDisplay = cardObject.GetComponent<Card>();

        if (cardDisplay != null)
        {
            // ������ ��������� �������� ��� �����
            cardDisplay.leftValue = Random.Range(1, 10);
            cardDisplay.rightValue = Random.Range(1, 10);
            cardDisplay.topValue = Random.Range(1, 10);
            cardDisplay.bottomValue = Random.Range(1, 10);

            // ��������� ����� � ������ ����� �� ������ �� UI
            cardList.Add(cardDisplay);

            // ���������� ����� �� UI
            cardDisplay.SetCard();
        }
        else
        {
            Debug.LogWarning("��������� Card �� ������ �� ������� �����!");
        }
    }
    /*
    // ����� ��� �������� ����� �� ������ � UI
    public void RemoveCardFromDeck(Card card)
    {
        if (cardList.Contains(card))
        {
            // ������� ����� �� ������
            cardList.Remove(card);
            Debug.Log($"����� �������. �������� ����: {cardList.Count}");

            // ���������� ������ ����� �� UI
            Destroy(card.gameObject);

            // ���������, ���� ���� ������ �� ��������, �������� �������
            if (cardList.Count == 0)
            {
                    Debug.Log("������ �����! ������� OnDeckEmpty �������.");
                    OnDeckEmpty.Invoke();
                
            }
        }
        else
        {
            Debug.LogWarning("����� �� ������� � ������!");
        }
    }*/
}
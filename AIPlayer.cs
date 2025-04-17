using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : MonoBehaviour
{
    private CardManager cardManager;
    private AIBattleFieldManager battleFieldManager; // ������� ���
    private List<Card> aiCards = new List<Card>();

    [SerializeField]
    private float aiMoveDelay = 1.5f;

    private void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        battleFieldManager = FindObjectOfType<AIBattleFieldManager>(); // ���� AIBattleFieldManager

        // ������������� �� ������� ����� ����
        if (ChangeTurn.Instance != null)
        {
            Debug.Log("AI Player ���������� �� ������� ����� ����");
            ChangeTurn.Instance.OnTurnChanged += OnTurnChanged;
        }
        else
        {
            Debug.LogError("ChangeTurn.Instance is null");
        }
    }

    private void OnTurnChanged()
    {
        Debug.Log($"��������� ����� ����. IsPlayer1Turn: {ChangeTurn.IsPlayer1Turn}");

        if (!ChangeTurn.IsPlayer1Turn) // ���� ��� AI
        {
            Debug.Log("��� AI. �������� ��� ����� " + aiMoveDelay + " ������");
            Invoke("MakeAIMove", aiMoveDelay);
        }
    }
    private void MakeAIMove()
    {
        Debug.Log($"AI ������ ���. �������� ����: {aiCards.Count}");

        if (aiCards.Count == 0)
        {
            Debug.LogWarning("� AI ��� ����!");
            return;
        }

        List<AIGridCell> emptyCells = battleFieldManager.GetEmptyCells(); // ��� List<GridCellForPlayerVsPlayer>
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("��� ��������� ����� ��� ����!");
            return;
        }

        // ���������, ���� �� �� ���� ����� ������
        bool hasPlayerCards = false;
        List<AIGridCell> allCells = battleFieldManager.GetAllCells(); // ��� List<GridCellForPlayerVsPlayer>
        foreach (var cell in allCells)
        {
            if (cell.HasCard() && cell.GetCard().isPlayer1Card)
            {
                hasPlayerCards = true;
                break;
            }
        }

        // ���� ��� ������ ��� (��� ���� ������ �� ����)
        if (!hasPlayerCards)
        {
            // �������� ��������� �����
            Card randomCard = aiCards[Random.Range(0, aiCards.Count)];

            // �������� ������ ������ ��� ������� ���� (�������������)
            AIGridCell bestFirstCell = null;

            // ������������ ����������� ������ ��� ������� ����
            foreach (AIGridCell cell in emptyCells)
            {
                if (cell.gridPosition.x == 1 && cell.gridPosition.y == 1) // ����������� ������ ��� ����� 3x3
                {
                    bestFirstCell = cell;
                    break;
                }
            }

            // ���� ����� ����������, �������� ��������� ������� ������
            if (bestFirstCell == null)
            {
                List<AIGridCell> cornerCells = emptyCells.FindAll(cell =>
                    (cell.gridPosition.x == 0 || cell.gridPosition.x == 2) &&
                    (cell.gridPosition.y == 0 || cell.gridPosition.y == 2));

                if (cornerCells.Count > 0)
                {
                    bestFirstCell = cornerCells[Random.Range(0, cornerCells.Count)];
                }
                else // ���� � ������� ����������, ����� ����� ��������� ������
                {
                    bestFirstCell = emptyCells[Random.Range(0, emptyCells.Count)];
                }
            }

            Debug.Log($"AI ������ ������ ��� � ������ [{bestFirstCell.gridPosition.x},{bestFirstCell.gridPosition.y}]");
            PlaceCardOnCell(randomCard, bestFirstCell);
            return;
        }

        // ������� ������ ������ ����, ���� ��� ���� ����� �� ����
        Card bestCard = null;
        AIGridCell bestCell = null;
        int bestScore = int.MinValue;

        foreach (Card card in aiCards)
        {
            foreach (AIGridCell cell in emptyCells)
            {
                int score = EvaluateMove(card, cell);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCard = card;
                    bestCell = cell;
                }
            }
        }

        // ���� �� ����� ������� ��� (��� ����������� ��������� �����), �������� ��������� ����� � ������
        if (bestScore <= 0) // ���� ��� �������� �����
        {
            bestCard = aiCards[Random.Range(0, aiCards.Count)];
            bestCell = emptyCells[Random.Range(0, emptyCells.Count)];
        }

        if (bestCard != null && bestCell != null)
        {
            Debug.Log($"AI ������ ����� � ���������� L:{bestCard.leftValue} R:{bestCard.rightValue} T:{bestCard.topValue} B:{bestCard.bottomValue}");
            Debug.Log($"AI �������� ����� � ������ [{bestCell.gridPosition.x},{bestCell.gridPosition.y}]");

            PlaceCardOnCell(bestCard, bestCell);
        }
        else
        {
            Debug.LogError("�� ������� ������� ����� ��� ������ ��� ���� AI");
        }
    }
    private int EvaluateMove(Card card, AIGridCell cell) // ������� ���
    {
        // ��������� ��������� ������� ��� ������ ������� ����
        int score = 0;

        // ��������� ����������� ������� �������� ����
        if (cell.leftNeighbor != null && cell.leftNeighbor.HasCard())
        {
            Card neighborCard = cell.leftNeighbor.GetCard();
            if (neighborCard.isPlayer1Card && card.leftValue > neighborCard.rightValue)
                score += 10;
        }

        // ���������� ��� ������ �����������...
        if (cell.rightNeighbor != null && cell.rightNeighbor.HasCard())
        {
            Card neighborCard = cell.rightNeighbor.GetCard();
            if (neighborCard.isPlayer1Card && card.rightValue > neighborCard.leftValue)
                score += 10;
        }

        if (cell.topNeighbor != null && cell.topNeighbor.HasCard())
        {
            Card neighborCard = cell.topNeighbor.GetCard();
            if (neighborCard.isPlayer1Card && card.topValue > neighborCard.bottomValue)
                score += 10;
        }

        if (cell.bottomNeighbor != null && cell.bottomNeighbor.HasCard())
        {
            Card neighborCard = cell.bottomNeighbor.GetCard();
            if (neighborCard.isPlayer1Card && card.bottomValue > neighborCard.topValue)
                score += 10;
        }

        // ����� �� ������������� ������ ������� (���� � �����)
        if ((cell.gridPosition.x == 0 || cell.gridPosition.x == 2) &&
            (cell.gridPosition.y == 0 || cell.gridPosition.y == 2))
        {
            score += 5; // ������� ������
        }
        else if (cell.gridPosition.x == 1 && cell.gridPosition.y == 1)
        {
            score += 3; // ����������� ������
        }

        return score;
    }

    private void PlaceCardOnCell(Card card, AIGridCell cell)
    {
        // Показываем карту перед перемещением
        card.gameObject.SetActive(true);

        // Перемещаем карту на ячейку
        card.transform.SetParent(cell.transform);
        card.transform.localPosition = Vector3.zero;

        // Устанавливаем масштаб 0.99
        card.transform.localScale = new Vector3(0.99f, 0.99f, 0.99f);

        // Настраиваем размеры через RectTransform
        RectTransform cardRect = card.GetComponent<RectTransform>();
        RectTransform cellRect = cell.GetComponent<RectTransform>();

        // Устанавливаем размер карты равным размеру ячейки
        cardRect.sizeDelta = cellRect.sizeDelta;
        cardRect.anchorMin = new Vector2(0, 0);
        cardRect.anchorMax = new Vector2(1, 1);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;

        // Устанавливаем начальный нейтральный цвет для карты
        Image cardImage = card.GetComponent<Image>();
        if (cardImage != null)
        {
            // Нейтральный цвет по умолчанию
            cardImage.color = Color.white;
        }

        // Устанавливаем ссылку на карту в ячейке
        cell.currentCard = card;

        // Удаляем карту из списка доступных карт AI
        aiCards.Remove(card);

        // Сравниваем с соседями
        cell.CompareWithNeighbors(card);

        // Передаем ход игроку
        ChangeTurn.Instance.SwitchTurn(cell.gridPosition);
    }
    // ����� ��� ���������� ����� AI
    public void AddCard(Card card)
    {
        if (card != null)
        {
            card.isPlayer1Card = false; // ��� ����� AI (�� ������ 1)
            aiCards.Add(card);

            // �������� �����, ���� �� �� ���������
            card.gameObject.SetActive(false);

            Debug.Log($"��������� ����� AI. ����� ����: {aiCards.Count}");
        }
    }

}
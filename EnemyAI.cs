using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public BattleFieldGridManager gridManager;
    public CardManager cardManager;  // ������ �� CardManager ��� ������ � ������� �����

    private bool hasPlacedCardThisTurn = true;
    private Card currentCard;
    private Color customGreen;
    private Color customRed;
    void OnEnable()
    {
        GridCell.OnCardPlaced += HandleCardPlacement;
    }

    void OnDisable()
    {
        GridCell.OnCardPlaced -= HandleCardPlacement;
    }
    private void Start()
    {
        // ����������� ��������� ���� � ������ Color
        if (!ColorUtility.TryParseHtmlString("#A3FF86", out customGreen))
        {
            Debug.LogError("Invalid color string for #A3FF86");
        }
        // ����������� ��������� ���� � ������ Color
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }
    }

    // ����� ����������, ����� ����� ��������� �����
    private void HandleCardPlacement(CardsForTakeCardsAndCollection card, GridCell cell, Vector2Int position)
    {
        if (card.isPlayer1Card && !hasPlacedCardThisTurn)
        {
            Debug.Log($"����� ��������� ����� � ������� {position}. ���� ����������� ���.");
            EnemyTurn(position);  // ��������� �������� ����� ����� �����
        }
    }

    public void EnemyTurn(Vector2Int playerPosition)
    {
        if (cardManager.enemyCards.Count == 0)
        {
            Debug.LogWarning("� ����� ��� ���� ��� ����������.");
            return;
        }

        Card enemyCard = cardManager.enemyCards[cardManager.enemyCards.Count - 1];
        cardManager.enemyCards.RemoveAt(cardManager.enemyCards.Count - 1);

        GridCell bestCell = null;
        int bestScore = int.MinValue;
        int bestDifference = int.MaxValue;

        // ��������� ����� ����� �� ��������� �������
        int searchRadius = 1;  // �������� � ������� 1 ������

        while (bestCell == null && searchRadius <= 5)  // ����������� ������ �� 5 �����, ���� ������ �� �������
        {
            GridCell[] neighborCells = GetNeighborCellsInRadius(playerPosition, searchRadius);
            Debug.Log($"������� {neighborCells.Length} �������� ������ ��� ������� ������ {playerPosition} � ������� {searchRadius}.");

            foreach (GridCell cell in neighborCells)
            {
                Debug.Log($"��������� ������ �� ������� {cell.gridPosition}. ������ �� ��� ������: {cell.HasCard()}");

                if (cell.HasCard() || cell.HasEnemyCard()) continue;  // ���������� ������ � �������

                int score = EvaluateCell(cell, enemyCard, playerPosition);
                int difference = CalculateDifference(cell, enemyCard);

                Debug.Log($"������ ��� ������ �� ������� {cell.gridPosition}: {score}, �������: {difference}");

                if (score > bestScore || (score == bestScore && difference < bestDifference))
                {
                    bestScore = score;
                    bestCell = cell;
                    bestDifference = difference;
                }
            }

            if (bestCell == null)
            {
                searchRadius++;  // ����������� ������ ������, ���� �� ������� ���������� �����
            }
        }

        if (bestCell != null)
        {
            Debug.Log($"������ ������ ������� �� ������� {bestCell.gridPosition} � ������� {bestScore} � �������� {bestDifference}. ��������� �����.");
            PlaceEnemyCard(enemyCard, bestCell);
        }
        else
        {
            Debug.LogWarning("����� �� ������� ����� ���������� ������.");
        }

        hasPlacedCardThisTurn = true;
    }
    // ����� ��� ��������� ����� � �������� ������� �� ������� ������ (������ ��������� � �����������)
    private GridCell[] GetNeighborCellsInRadius(Vector2Int playerPosition, int radius)
    {
        var neighbors = new List<GridCell>();
        int columns, rows;
        gridManager.GetGridDimensions(out columns, out rows);

        // ��������� ������ ������������ � �������������� ������ (��������� ������������)
        for (int x = -radius; x <= radius; x++)
        {
            int checkX = playerPosition.x + x;
            int checkY = playerPosition.y;

            // ��������, ��� ������ ��������� � �������� ������ ����� �� �����������
            if (checkX >= 0 && checkX < columns)
            {
                neighbors.Add(gridManager.gridCells[checkX, checkY]);
            }
        }

        for (int y = -radius; y <= radius; y++)
        {
            int checkX = playerPosition.x;
            int checkY = playerPosition.y + y;

            // ��������, ��� ������ ��������� � �������� ������ ����� �� ���������
            if (checkY >= 0 && checkY < rows)
            {
                neighbors.Add(gridManager.gridCells[checkX, checkY]);
            }
        }

        return neighbors.ToArray();
    }

    // ����� ����� ��� ���������� ������� ����� ����������
    private int CalculateDifference(GridCell cell, Card enemyCard)
    {
        int difference = 0;

        // ��������� ������ �����
        if (cell.leftNeighbor != null && cell.leftNeighbor.HasCard())
        {
            var leftCard = cell.leftNeighbor.GetCard();
            if (leftCard != null)  // ���������, ��� ����� �� null
            {
                difference += Mathf.Abs(enemyCard.leftValue - leftCard.rightValue);
            }
        }

        // ��������� ������ ������
        if (cell.rightNeighbor != null && cell.rightNeighbor.HasCard())
        {
            var rightCard = cell.rightNeighbor.GetCard();
            if (rightCard != null)  // ���������, ��� ����� �� null
            {
                difference += Mathf.Abs(enemyCard.rightValue - rightCard.leftValue);
            }
        }

        // ��������� ������ ������
        if (cell.topNeighbor != null && cell.topNeighbor.HasCard())
        {
            var topCard = cell.topNeighbor.GetCard();
            if (topCard != null)  // ���������, ��� ����� �� null
            {
                difference += Mathf.Abs(enemyCard.topValue - topCard.bottomValue);
            }
        }

        // ��������� ������ �����
        if (cell.bottomNeighbor != null && cell.bottomNeighbor.HasCard())
        {
            var bottomCard = cell.bottomNeighbor.GetCard();
            if (bottomCard != null)  // ���������, ��� ����� �� null
            {
                difference += Mathf.Abs(enemyCard.bottomValue - bottomCard.topValue);
            }
        }

        return difference;
    }


    // ��������� �������� ������ ������ ����� ������ (������ �� ���������� +/-1)
    private GridCell[] GetNeighborCells(Vector2Int playerPosition)
    {
        var neighbors = new System.Collections.Generic.List<GridCell>();

        int columns, rows;
        gridManager.GetGridDimensions(out columns, out rows);

        if (playerPosition.x > 0)
            neighbors.Add(gridManager.gridCells[playerPosition.x - 1, playerPosition.y]);

        if (playerPosition.x < columns - 1)
            neighbors.Add(gridManager.gridCells[playerPosition.x + 1, playerPosition.y]);

        if (playerPosition.y > 0)
            neighbors.Add(gridManager.gridCells[playerPosition.x, playerPosition.y - 1]);

        if (playerPosition.y < rows - 1)
            neighbors.Add(gridManager.gridCells[playerPosition.x, playerPosition.y + 1]);

        return neighbors.ToArray();
    }
    private int EvaluateCell(GridCell cell, Card enemyCard, Vector2Int playerPosition)
    {
        int score = 0;

        // ���������� ��� �������� ���������
        int leftDiff = int.MinValue;
        int rightDiff = int.MinValue;
        int topDiff = int.MinValue;
        int bottomDiff = int.MinValue;

        // �������� ������ �����
        if (cell.leftNeighbor != null && (cell.leftNeighbor.HasCard() || cell.leftNeighbor.GetCard() != null))
        {
            var leftCard = cell.leftNeighbor.GetCard();  // �������� ����� ��� ���������
            if (leftCard != null)  // ���������, ��� ����� �� null
            {
                Debug.Log($"��������� �����: ��������� ����� L:{enemyCard.leftValue} � ������ R:{leftCard.rightValue}");
                if (enemyCard.leftValue > leftCard.rightValue)
                {
                    score += 3;
                }
                else if (enemyCard.leftValue == leftCard.rightValue)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }

                leftDiff = enemyCard.leftValue - leftCard.rightValue;
                if (leftDiff > 0)
                {
                    score += 2;
                }
                else if (leftDiff == 0)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }
            }
        }

        // �������� ������ ������
        if (cell.rightNeighbor != null && (cell.rightNeighbor.HasCard() || cell.rightNeighbor.GetCard() != null))
        {
            var rightCard = cell.rightNeighbor.GetCard();  // �������� ����� ��� ���������
            if (rightCard != null)  // ���������, ��� ����� �� null
            {
                Debug.Log($"��������� ������: ��������� ����� R:{enemyCard.rightValue} � ������ L:{rightCard.leftValue}");
                if (enemyCard.rightValue > rightCard.leftValue)
                {
                    score += 3;
                }
                else if (enemyCard.rightValue == rightCard.leftValue)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }

                rightDiff = enemyCard.rightValue - rightCard.leftValue;
                if (rightDiff > 0)
                {
                    score += 2;
                }
                else if (rightDiff == 0)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }
            }
        }

        // �������� ������ ������
        if (cell.topNeighbor != null && (cell.topNeighbor.HasCard() || cell.topNeighbor.GetCard() != null))
        {
            var topCard = cell.topNeighbor.GetCard();  // �������� ����� ��� ���������
            if (topCard != null)  // ���������, ��� ����� �� null
            {
                Debug.Log($"��������� ������: ��������� ����� T:{enemyCard.topValue} � ������ B:{topCard.bottomValue}");
                if (enemyCard.topValue > topCard.bottomValue)
                {
                    score += 3;
                }
                else if (enemyCard.topValue == topCard.bottomValue)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }

                topDiff = enemyCard.topValue - topCard.bottomValue;
                if (topDiff > 0)
                {
                    score += 2;
                }
                else if (topDiff == 0)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }
            }
        }

        // �������� ������ �����
        if (cell.bottomNeighbor != null && (cell.bottomNeighbor.HasCard() || cell.bottomNeighbor.GetCard() != null))
        {
            var bottomCard = cell.bottomNeighbor.GetCard();  // �������� ����� ��� ���������
            if (bottomCard != null)  // ���������, ��� ����� �� null
            {
                Debug.Log($"��������� �����: ��������� ����� B:{enemyCard.bottomValue} � ������ T:{bottomCard.topValue}");
                if (enemyCard.bottomValue > bottomCard.topValue)
                {
                    score += 3;
                }
                else if (enemyCard.bottomValue == bottomCard.topValue)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }

                bottomDiff = enemyCard.bottomValue - bottomCard.topValue;
                if (bottomDiff > 0)
                {
                    score += 2;
                }
                else if (bottomDiff == 0)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }
            }
        }

        // ������� ������������ �������� ����� ���� �����������
        int maxDiff = Math.Max(Math.Max(leftDiff, rightDiff), Math.Max(topDiff, bottomDiff));

        // ����������� score, ���� �������� ����������� � ���������� �����������
        if (leftDiff == maxDiff && leftDiff > 0)
        {
            score += 2;
        }
        if (rightDiff == maxDiff && rightDiff > 0)
        {
            score += 2;
        }
        if (topDiff == maxDiff && topDiff > 0)
        {
            score += 2;
        }
        if (bottomDiff == maxDiff && bottomDiff > 0)
        {
            score += 2;
        }

        // �������������� ������ �� �������� � ����� ������
        score -= (int)Vector2Int.Distance(cell.gridPosition, playerPosition);

        Debug.Log($"������ ��� ������ {cell.gridPosition} � ������ �����: L:{enemyCard.leftValue}, R:{enemyCard.rightValue}, T:{enemyCard.topValue}, B:{enemyCard.bottomValue} - ������ {score}");

        return score;
    }
    private void PlaceEnemyCard(Card enemyCard, GridCell cell)
    {
        if (cell.HasCard())  // ���������, ��� � ������ ��� ����� ����� ��� ������
        {
            Debug.LogWarning($"������ {cell.gridPosition} ��� ������ ������.");
            return;  // ���� � ������ ���� �����, �� ��������� ����� �����
        }

        // ��������� ����� � ������
        enemyCard.transform.SetParent(cell.transform);
        enemyCard.transform.localPosition = Vector3.zero;
        enemyCard.transform.rotation = Quaternion.identity;

        cell.currentCard = enemyCard;  // ������������� ����� � ������

        // ��������� ����� ������� � �������� ���� ����� �����, ���� ����� ������ ������
        CompareWithNeighbors(cell, enemyCard);

        Debug.Log($"����� ����� ��������� � ������ {cell.gridPosition}");

        // �������� ��� ������
        ChangeTurn.Instance.SwitchTurn(cell.gridPosition);

        hasPlacedCardThisTurn = false;  // ���������� ���� ��� ���������� ���� ������
    }

    /*
    private void PlaceEnemyCard(Card enemyCard, GridCell cell)
    {
        if (cell.HasCard())  // ���������, ��� � ������ ��� ����� ����� ��� ������
        {
            Debug.LogWarning($"������ {cell.gridPosition} ��� ������ ������ �����.");
            return;  // ���� � ������ ���� ����� �����, �� ��������� ����� �����
        }

        // ��������� ����� � ������
        enemyCard.transform.SetParent(cell.transform);
        enemyCard.transform.localPosition = Vector3.zero;
        enemyCard.transform.rotation = Quaternion.identity;

        cell.currentCard = enemyCard;  // ������������� ����� � ������

        Debug.Log($"����� ����� ��������� � ������ {cell.gridPosition}");

        // �������� ��� ������
        ChangeTurn.Instance.SwitchTurn(cell.gridPosition);

        hasPlacedCardThisTurn = false;  // ���������� ���� ��� ���������� ���� ������
    }
    */
    public bool HasCard()
    {
        return currentCard != null;
    }

    public Card GetCard()
    {
        return currentCard;
    }
    private void CompareWithNeighbors(GridCell cell, Card enemyCard)
    {
        // �������� ������ �����
        if (cell.leftNeighbor != null && cell.leftNeighbor.HasCard())
        {
            var leftCard = cell.leftNeighbor.GetCard();
            if (leftCard != null && leftCard.isPlayer1Card)  // ���� ����� ������
            {
                if (leftCard.rightValue > enemyCard.leftValue)
                {
                    // ����� ������ ������ - ���������� ����� ����� � ������� ����
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }

        // �������� ������ ������
        if (cell.rightNeighbor != null && cell.rightNeighbor.HasCard())
        {
            var rightCard = cell.rightNeighbor.GetCard();
            if (rightCard != null && rightCard.isPlayer1Card)  // ���� ����� ������
            {
                if (rightCard.leftValue > enemyCard.rightValue)
                {
                    // ����� ������ ������ - ���������� ����� ����� � ������� ����
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }

        // �������� ������ ������
        if (cell.topNeighbor != null && cell.topNeighbor.HasCard())
        {
            var topCard = cell.topNeighbor.GetCard();
            if (topCard != null && topCard.isPlayer1Card)  // ���� ����� ������
            {
                if (topCard.bottomValue > enemyCard.topValue)
                {
                    // ����� ������ ������ - ���������� ����� ����� � ������� ����
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }

        // �������� ������ �����
        if (cell.bottomNeighbor != null && cell.bottomNeighbor.HasCard())
        {
            var bottomCard = cell.bottomNeighbor.GetCard();
            if (bottomCard != null && bottomCard.isPlayer1Card)  // ���� ����� ������
            {
                if (bottomCard.topValue > enemyCard.bottomValue)
                {
                    // ����� ������ ������ - ���������� ����� ����� � ������� ����
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }
    }

    public void ChangeCardColor(Color color)
    {
        if (HasCard())
        {
            Image cardImage = transform.GetChild(0).GetComponent<Image>();
            if (cardImage != null)
            {
                Debug.Log($"Changing card color to {color}.");
                cardImage.color = color; // �������� ���� �����
            }
            else
            {
                Debug.LogError("Image component not found on card!");
            }
        }
        else
        {
            Debug.LogError("No card found in this cell!");
        }
    }
    private void ChangeCardColor(Card enemyCard, Color color)
    {
        Image cardImage = enemyCard.GetComponentInChildren<Image>();  // �����������, ��� ����� ����� Image ���������
        if (cardImage != null)
        {
            Debug.Log($"��������� ����� ����� �� {color}");
            cardImage.color = color;
        }
        else
        {
            Debug.LogError("Image component not found on enemy card!");
        }
    }

}
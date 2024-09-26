using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public BattleFieldGridManager gridManager;
    public CardManager cardManager;  // Ссылка на CardManager для работы с колодой врага

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
        // Преобразуем строковый цвет в объект Color
        if (!ColorUtility.TryParseHtmlString("#A3FF86", out customGreen))
        {
            Debug.LogError("Invalid color string for #A3FF86");
        }
        // Преобразуем строковый цвет в объект Color
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }
    }

    // Метод вызывается, когда игрок размещает карту
    private void HandleCardPlacement(CardsForTakeCardsAndCollection card, GridCell cell, Vector2Int position)
    {
        if (card.isPlayer1Card && !hasPlacedCardThisTurn)
        {
            Debug.Log($"Игрок разместил карту в позиции {position}. Враг анализирует ход.");
            EnemyTurn(position);  // Добавляем задержку перед ходом врага
        }
    }

    public void EnemyTurn(Vector2Int playerPosition)
    {
        if (cardManager.enemyCards.Count == 0)
        {
            Debug.LogWarning("У врага нет карт для размещения.");
            return;
        }

        Card enemyCard = cardManager.enemyCards[cardManager.enemyCards.Count - 1];
        cardManager.enemyCards.RemoveAt(cardManager.enemyCards.Count - 1);

        GridCell bestCell = null;
        int bestScore = int.MinValue;
        int bestDifference = int.MaxValue;

        // Расширяем поиск ячеек на несколько уровней
        int searchRadius = 1;  // Начинаем с радиуса 1 ячейка

        while (bestCell == null && searchRadius <= 5)  // Увеличиваем радиус до 5 ячеек, если ничего не найдено
        {
            GridCell[] neighborCells = GetNeighborCellsInRadius(playerPosition, searchRadius);
            Debug.Log($"Найдено {neighborCells.Length} соседних клеток для позиции игрока {playerPosition} в радиусе {searchRadius}.");

            foreach (GridCell cell in neighborCells)
            {
                Debug.Log($"Проверяем клетку на позиции {cell.gridPosition}. Занята ли она картой: {cell.HasCard()}");

                if (cell.HasCard() || cell.HasEnemyCard()) continue;  // Пропускаем клетки с картами

                int score = EvaluateCell(cell, enemyCard, playerPosition);
                int difference = CalculateDifference(cell, enemyCard);

                Debug.Log($"Оценка для клетки на позиции {cell.gridPosition}: {score}, Разница: {difference}");

                if (score > bestScore || (score == bestScore && difference < bestDifference))
                {
                    bestScore = score;
                    bestCell = cell;
                    bestDifference = difference;
                }
            }

            if (bestCell == null)
            {
                searchRadius++;  // Увеличиваем радиус поиска, если не найдено подходящих ячеек
            }
        }

        if (bestCell != null)
        {
            Debug.Log($"Лучшая клетка найдена на позиции {bestCell.gridPosition} с оценкой {bestScore} и разницей {bestDifference}. Размещаем карту.");
            PlaceEnemyCard(enemyCard, bestCell);
        }
        else
        {
            Debug.LogWarning("Врагу не удалось найти подходящую ячейку.");
        }

        hasPlacedCardThisTurn = true;
    }
    // Метод для получения ячеек в заданном радиусе от позиции игрока (только вертикаль и горизонталь)
    private GridCell[] GetNeighborCellsInRadius(Vector2Int playerPosition, int radius)
    {
        var neighbors = new List<GridCell>();
        int columns, rows;
        gridManager.GetGridDimensions(out columns, out rows);

        // Проверяем только вертикальные и горизонтальные клетки (исключаем диагональные)
        for (int x = -radius; x <= radius; x++)
        {
            int checkX = playerPosition.x + x;
            int checkY = playerPosition.y;

            // Убедимся, что ячейка находится в пределах границ сетки по горизонтали
            if (checkX >= 0 && checkX < columns)
            {
                neighbors.Add(gridManager.gridCells[checkX, checkY]);
            }
        }

        for (int y = -radius; y <= radius; y++)
        {
            int checkX = playerPosition.x;
            int checkY = playerPosition.y + y;

            // Убедимся, что ячейка находится в пределах границ сетки по вертикали
            if (checkY >= 0 && checkY < rows)
            {
                neighbors.Add(gridManager.gridCells[checkX, checkY]);
            }
        }

        return neighbors.ToArray();
    }

    // Новый метод для вычисления разницы между значениями
    private int CalculateDifference(GridCell cell, Card enemyCard)
    {
        int difference = 0;

        // Проверяем соседа слева
        if (cell.leftNeighbor != null && cell.leftNeighbor.HasCard())
        {
            var leftCard = cell.leftNeighbor.GetCard();
            if (leftCard != null)  // Проверяем, что карта не null
            {
                difference += Mathf.Abs(enemyCard.leftValue - leftCard.rightValue);
            }
        }

        // Проверяем соседа справа
        if (cell.rightNeighbor != null && cell.rightNeighbor.HasCard())
        {
            var rightCard = cell.rightNeighbor.GetCard();
            if (rightCard != null)  // Проверяем, что карта не null
            {
                difference += Mathf.Abs(enemyCard.rightValue - rightCard.leftValue);
            }
        }

        // Проверяем соседа сверху
        if (cell.topNeighbor != null && cell.topNeighbor.HasCard())
        {
            var topCard = cell.topNeighbor.GetCard();
            if (topCard != null)  // Проверяем, что карта не null
            {
                difference += Mathf.Abs(enemyCard.topValue - topCard.bottomValue);
            }
        }

        // Проверяем соседа снизу
        if (cell.bottomNeighbor != null && cell.bottomNeighbor.HasCard())
        {
            var bottomCard = cell.bottomNeighbor.GetCard();
            if (bottomCard != null)  // Проверяем, что карта не null
            {
                difference += Mathf.Abs(enemyCard.bottomValue - bottomCard.topValue);
            }
        }

        return difference;
    }


    // Получение соседних клеток вокруг карты игрока (только на расстоянии +/-1)
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

        // Переменные для хранения разностей
        int leftDiff = int.MinValue;
        int rightDiff = int.MinValue;
        int topDiff = int.MinValue;
        int bottomDiff = int.MinValue;

        // Проверка соседа слева
        if (cell.leftNeighbor != null && (cell.leftNeighbor.HasCard() || cell.leftNeighbor.GetCard() != null))
        {
            var leftCard = cell.leftNeighbor.GetCard();  // Получаем карту для сравнения
            if (leftCard != null)  // Проверяем, что карта не null
            {
                Debug.Log($"Сравнение слева: Вражеская карта L:{enemyCard.leftValue} с картой R:{leftCard.rightValue}");
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

        // Проверка соседа справа
        if (cell.rightNeighbor != null && (cell.rightNeighbor.HasCard() || cell.rightNeighbor.GetCard() != null))
        {
            var rightCard = cell.rightNeighbor.GetCard();  // Получаем карту для сравнения
            if (rightCard != null)  // Проверяем, что карта не null
            {
                Debug.Log($"Сравнение справа: Вражеская карта R:{enemyCard.rightValue} с картой L:{rightCard.leftValue}");
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

        // Проверка соседа сверху
        if (cell.topNeighbor != null && (cell.topNeighbor.HasCard() || cell.topNeighbor.GetCard() != null))
        {
            var topCard = cell.topNeighbor.GetCard();  // Получаем карту для сравнения
            if (topCard != null)  // Проверяем, что карта не null
            {
                Debug.Log($"Сравнение сверху: Вражеская карта T:{enemyCard.topValue} с картой B:{topCard.bottomValue}");
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

        // Проверка соседа снизу
        if (cell.bottomNeighbor != null && (cell.bottomNeighbor.HasCard() || cell.bottomNeighbor.GetCard() != null))
        {
            var bottomCard = cell.bottomNeighbor.GetCard();  // Получаем карту для сравнения
            if (bottomCard != null)  // Проверяем, что карта не null
            {
                Debug.Log($"Сравнение снизу: Вражеская карта B:{enemyCard.bottomValue} с картой T:{bottomCard.topValue}");
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

        // Находим максимальную разность среди всех направлений
        int maxDiff = Math.Max(Math.Max(leftDiff, rightDiff), Math.Max(topDiff, bottomDiff));

        // Увеличиваем score, если разность максимальна в конкретном направлении
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

        // Дополнительная оценка за близость к карте игрока
        score -= (int)Vector2Int.Distance(cell.gridPosition, playerPosition);

        Debug.Log($"Оценка для клетки {cell.gridPosition} с картой врага: L:{enemyCard.leftValue}, R:{enemyCard.rightValue}, T:{enemyCard.topValue}, B:{enemyCard.bottomValue} - оценка {score}");

        return score;
    }
    private void PlaceEnemyCard(Card enemyCard, GridCell cell)
    {
        if (cell.HasCard())  // Проверяем, что в ячейке нет карты врага или игрока
        {
            Debug.LogWarning($"Ячейка {cell.gridPosition} уже занята картой.");
            return;  // Если в ячейке есть карта, не размещаем новую карту
        }

        // Размещаем карту в клетке
        enemyCard.transform.SetParent(cell.transform);
        enemyCard.transform.localPosition = Vector3.zero;
        enemyCard.transform.rotation = Quaternion.identity;

        cell.currentCard = enemyCard;  // Устанавливаем карту в клетку

        // Проверяем карты соседей и изменяем цвет карты врага, если карта игрока больше
        CompareWithNeighbors(cell, enemyCard);

        Debug.Log($"Карта врага размещена в ячейке {cell.gridPosition}");

        // Передаем ход игроку
        ChangeTurn.Instance.SwitchTurn(cell.gridPosition);

        hasPlacedCardThisTurn = false;  // Сбрасываем флаг для следующего хода игрока
    }

    /*
    private void PlaceEnemyCard(Card enemyCard, GridCell cell)
    {
        if (cell.HasCard())  // Проверяем, что в ячейке нет карты врага или игрока
        {
            Debug.LogWarning($"Ячейка {cell.gridPosition} уже занята картой врага.");
            return;  // Если в ячейке есть карта врага, не размещаем новую карту
        }

        // Размещаем карту в клетке
        enemyCard.transform.SetParent(cell.transform);
        enemyCard.transform.localPosition = Vector3.zero;
        enemyCard.transform.rotation = Quaternion.identity;

        cell.currentCard = enemyCard;  // Устанавливаем карту в клетку

        Debug.Log($"Карта врага размещена в ячейке {cell.gridPosition}");

        // Передаем ход игроку
        ChangeTurn.Instance.SwitchTurn(cell.gridPosition);

        hasPlacedCardThisTurn = false;  // Сбрасываем флаг для следующего хода игрока
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
        // Проверка соседа слева
        if (cell.leftNeighbor != null && cell.leftNeighbor.HasCard())
        {
            var leftCard = cell.leftNeighbor.GetCard();
            if (leftCard != null && leftCard.isPlayer1Card)  // Если карта игрока
            {
                if (leftCard.rightValue > enemyCard.leftValue)
                {
                    // Карта игрока больше - окрашиваем карту врага в красный цвет
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }

        // Проверка соседа справа
        if (cell.rightNeighbor != null && cell.rightNeighbor.HasCard())
        {
            var rightCard = cell.rightNeighbor.GetCard();
            if (rightCard != null && rightCard.isPlayer1Card)  // Если карта игрока
            {
                if (rightCard.leftValue > enemyCard.rightValue)
                {
                    // Карта игрока больше - окрашиваем карту врага в красный цвет
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }

        // Проверка соседа сверху
        if (cell.topNeighbor != null && cell.topNeighbor.HasCard())
        {
            var topCard = cell.topNeighbor.GetCard();
            if (topCard != null && topCard.isPlayer1Card)  // Если карта игрока
            {
                if (topCard.bottomValue > enemyCard.topValue)
                {
                    // Карта игрока больше - окрашиваем карту врага в красный цвет
                    ChangeCardColor(enemyCard, customRed);
                }
            }
        }

        // Проверка соседа снизу
        if (cell.bottomNeighbor != null && cell.bottomNeighbor.HasCard())
        {
            var bottomCard = cell.bottomNeighbor.GetCard();
            if (bottomCard != null && bottomCard.isPlayer1Card)  // Если карта игрока
            {
                if (bottomCard.topValue > enemyCard.bottomValue)
                {
                    // Карта игрока больше - окрашиваем карту врага в красный цвет
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
                cardImage.color = color; // Изменяем цвет карты
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
        Image cardImage = enemyCard.GetComponentInChildren<Image>();  // Предположим, что карта имеет Image компонент
        if (cardImage != null)
        {
            Debug.Log($"Изменение цвета карты на {color}");
            cardImage.color = color;
        }
        else
        {
            Debug.LogError("Image component not found on enemy card!");
        }
    }

}
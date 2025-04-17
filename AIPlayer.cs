using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : MonoBehaviour
{
    private CardManager cardManager;
    private AIBattleFieldManager battleFieldManager;
    private List<Card> aiCards = new List<Card>();

    [SerializeField]
    private float aiMoveDelay = 1.5f;
    private float initialMoveDelay = 2.0f; // Задержка для первого хода

    private Transform cardsParent; // Для хранения ссылки на родительский объект карт

    private void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        battleFieldManager = FindObjectOfType<AIBattleFieldManager>();

        // Очищаем список карт от невалидных ссылок при запуске
        Invoke("ValidateCardList", 0.5f);

        // Подписываемся на событие смены хода
        if (ChangeTurn.Instance != null)
        {
            Debug.Log("AI Player подписался на события смены хода");
            ChangeTurn.Instance.OnTurnChanged += OnTurnChanged;

            // Проверяем, должен ли AI ходить первым
            Invoke("CheckInitialTurn", initialMoveDelay);
        }
        else
        {
            Debug.LogError("ChangeTurn.Instance is null");
        }
    }

    private void ValidateCardList()
    {
        Debug.Log("Проверка списка карт AI...");
        CleanupNullCards();
        
        // Если после очистки у ИИ нет карт, но CardManager доступен, создаем новые карты
        if (aiCards.Count == 0 && cardManager != null)
        {
            Debug.Log("Список карт ИИ пуст. Запрашиваем создание новых карт.");
            cardManager.SpawnAICards(); // Убедитесь, что этот метод доступен в CardManager
        }
        
        Debug.Log($"Валидация карт завершена. У ИИ {aiCards.Count} карт.");
    }

    // Новый метод для проверки, чей первый ход
    private void CheckInitialTurn()
    {
        Debug.Log("Проверка, чей первый ход...");

        // Если сейчас ход ИИ, делаем ход
        if (!ChangeTurn.IsPlayer1Turn)
        {
            Debug.Log("Обнаружено, что первый ход - ход ИИ");
            MakeAIMove();
        }
        else
        {
            Debug.Log("Первый ход принадлежит игроку");
        }
    }

    private void OnTurnChanged()
    {
        Debug.Log($"Обработка смены хода. IsPlayer1Turn: {ChangeTurn.IsPlayer1Turn}");

        if (!ChangeTurn.IsPlayer1Turn) // Если ход AI
        {
            Debug.Log("Ход AI. Планирую ход через " + aiMoveDelay + " секунд");
            Invoke("MakeAIMove", aiMoveDelay);
        }
    }

    // Добавляем публичный метод, чтобы можно было получить количество карт
    public int GetCardCount()
    {
        return aiCards.Count;
    }

    private void MakeAIMove()
    {
        Debug.Log($"AI делает ход. Доступно карт: {aiCards.Count}");
        
        // Очищаем список от null-ссылок
        CleanupNullCards();
        
        // Проверяем количество карт после очистки
        Debug.Log($"После очистки null-ссылок: {aiCards.Count} карт");
        
        if (aiCards.Count == 0)
        {
            Debug.LogWarning("У AI нет карт после очистки!");
            // Здесь можно добавить код для создания новых карт при необходимости
            return;
        }

        // Проверяем все карты в списке
        for (int i = 0; i < aiCards.Count; i++)
        {
            Card card = aiCards[i];
            if (card == null)
            {
                Debug.LogError($"Карта в позиции {i} все еще равна null после очистки!");
                continue;
            }
            
            Debug.Log($"Карта {i}: L:{card.leftValue} R:{card.rightValue} T:{card.topValue} B:{card.bottomValue}, Активна: {card.gameObject.activeSelf}, Родитель: {card.transform.parent?.name ?? "null"}");
        }

        if (aiCards.Count == 0)
        {
            Debug.LogWarning("У AI нет карт!");
            return;
        }

        List<AIGridCell> emptyCells = battleFieldManager.GetEmptyCells(); // List<GridCellForPlayerVsPlayer>
        if (emptyCells.Count == 0)
        {
            Debug.LogWarning("Невозможно сделать ход! Все ячейки заняты");
            return;
        }

        // Проверяем все карты в списке
        for (int i = 0; i < aiCards.Count; i++)
        {
            Card card = aiCards[i];
            if (card == null)
            {
                Debug.LogError($"Карта в позиции {i} равна null!");
                continue;
            }
            
            Debug.Log($"Карта {i}: L:{card.leftValue} R:{card.rightValue} T:{card.topValue} B:{card.bottomValue}, Активна: {card.gameObject.activeSelf}, Родитель: {card.transform.parent?.name ?? "null"}");
        }

        // Проверяем, есть ли у ИИ карты, которые могут побить все карты игрока
        bool hasPlayerCards = false;
        List<AIGridCell> allCells = battleFieldManager.GetAllCells(); // List<GridCellForPlayerVsPlayer>
        foreach (var cell in allCells)
        {
            if (cell.HasCard() && cell.GetCard().isPlayer1Card)
            {
                hasPlayerCards = true;
                break;
            }
        }

        // Если у ИИ нет карт, которые могут побить все карты игрока, он выбирает случайную карту
        if (!hasPlayerCards)
        {
            Card randomCard = aiCards[Random.Range(0, aiCards.Count)];

            AIGridCell bestFirstCell = null;

            foreach (AIGridCell cell in emptyCells)
            {
                if (cell.gridPosition.x == 1 && cell.gridPosition.y == 1) // Центральная ячейка 3x3
                {
                    bestFirstCell = cell;
                    break;
                }
            }

            if (bestFirstCell == null)
            {
                List<AIGridCell> cornerCells = emptyCells.FindAll(cell =>
                    (cell.gridPosition.x == 0 || cell.gridPosition.x == 2) &&
                    (cell.gridPosition.y == 0 || cell.gridPosition.y == 2));

                if (cornerCells.Count > 0)
                {
                    bestFirstCell = cornerCells[Random.Range(0, cornerCells.Count)];
                }
                else // Если нет угловых ячеек, выбираем случайную ячейку
                {
                    bestFirstCell = emptyCells[Random.Range(0, emptyCells.Count)];
                }
            }

            Debug.Log($"AI выбрал карту с значениями L:{randomCard.leftValue} R:{randomCard.rightValue} T:{randomCard.topValue} B:{randomCard.bottomValue}");
            Debug.Log($"AI помещает карту в ячейку [{bestFirstCell.gridPosition.x},{bestFirstCell.gridPosition.y}]");

            // Проверка наличия карты в списке доступных карт
            if (!aiCards.Contains(randomCard))
            {
                Debug.LogError("Выбранной карты нет в списке доступных карт ИИ!");
                if (aiCards.Count > 0)
                {
                    // Выбираем первую доступную карту
                    randomCard = aiCards[0];
                    Debug.Log($"Используем другую карту: L:{randomCard.leftValue} R:{randomCard.rightValue} T:{randomCard.topValue} B:{randomCard.bottomValue}");
                }
                else
                {
                    Debug.LogError("У ИИ нет карт!");
                    return;
                }
            }

            PlaceCardOnCell(randomCard, bestFirstCell);
            return;
        }

        // Если у ИИ есть карты, которые могут побить все карты игрока, он выбирает лучшую из них
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

        // Если лучший результат меньше или равен нулю, выбираем случайную карту
        if (bestScore <= 0 && aiCards.Count > 0) // Добавляем проверку наличия карт
        {
            bestCard = aiCards[Random.Range(0, aiCards.Count)];
            bestCell = emptyCells[Random.Range(0, emptyCells.Count)];
        }

        if (bestCard != null && bestCell != null)
        {
            Debug.Log($"AI выбрал карту с значениями L:{bestCard.leftValue} R:{bestCard.rightValue} T:{bestCard.topValue} B:{bestCard.bottomValue}");
            Debug.Log($"AI помещает карту в ячейку [{bestCell.gridPosition.x},{bestCell.gridPosition.y}]");

            // Проверка наличия карты в списке доступных карт
            if (!aiCards.Contains(bestCard))
            {
                Debug.LogError("Выбранной карты нет в списке доступных карт ИИ!");
                if (aiCards.Count > 0)
                {
                    // Выбираем первую доступную карту
                    bestCard = aiCards[0];
                    Debug.Log($"Используем другую карту: L:{bestCard.leftValue} R:{bestCard.rightValue} T:{bestCard.topValue} B:{bestCard.bottomValue}");
                }
                else
                {
                    Debug.LogError("У ИИ нет карт!");
                    return;
                }
            }

            PlaceCardOnCell(bestCard, bestCell);
        }
    }

    private int EvaluateMove(Card card, AIGridCell cell)
    {
        int score = 0;

        // Проверяем возможность захвата соседних карт
        if (cell.leftNeighbor != null && cell.leftNeighbor.HasCard())
        {
            Card neighborCard = cell.leftNeighbor.GetCard();
            // Для левого соседа сравниваем левое значение нашей карты с правым значением соседа
            if (neighborCard.isPlayer1Card && card.leftValue > neighborCard.rightValue)
            {
                score += 10;
                Debug.Log($"Оценка хода: карта может побить левого соседа {card.leftValue} > {neighborCard.rightValue}");
            }
        }

        if (cell.rightNeighbor != null && cell.rightNeighbor.HasCard())
        {
            Card neighborCard = cell.rightNeighbor.GetCard();
            // Для правого соседа сравниваем правое значение нашей карты с левым значением соседа
            if (neighborCard.isPlayer1Card && card.rightValue > neighborCard.leftValue)
            {
                score += 10;
                Debug.Log($"Оценка хода: карта может побить правого соседа {card.rightValue} > {neighborCard.leftValue}");
            }
        }

        if (cell.topNeighbor != null && cell.topNeighbor.HasCard())
        {
            Card neighborCard = cell.topNeighbor.GetCard();
            // ВАЖНО: Для верхнего соседа сравниваем верхнее значение нашей карты с нижним значением соседа
            if (neighborCard.isPlayer1Card && card.topValue > neighborCard.bottomValue)
            {
                score += 10;
                Debug.Log($"Оценка хода: карта может побить верхнего соседа {card.topValue} > {neighborCard.bottomValue}");
            }
        }

        if (cell.bottomNeighbor != null && cell.bottomNeighbor.HasCard())
        {
            Card neighborCard = cell.bottomNeighbor.GetCard();
            // ВАЖНО: Для нижнего соседа сравниваем нижнее значение нашей карты с верхним значением соседа
            if (neighborCard.isPlayer1Card && card.bottomValue > neighborCard.topValue)
            {
                score += 10;
                Debug.Log($"Оценка хода: карта может побить нижнего соседа {card.bottomValue} > {neighborCard.topValue}");
            }
        }

        // Бонус за стратегически важные позиции (углы и центр)
        if ((cell.gridPosition.x == 0 || cell.gridPosition.x == 2) &&
            (cell.gridPosition.y == 0 || cell.gridPosition.y == 2))
        {
            score += 5; // Угловые ячейки
        }
        else if (cell.gridPosition.x == 1 && cell.gridPosition.y == 1)
        {
            score += 3; // Центральная ячейка
        }

        return score;
    }

    private void PlaceCardOnCell(Card card, AIGridCell cell)
    {
        // Проверяем, что карта и ячейка существуют
        if (card == null)
        {
            Debug.LogError("Попытка разместить null-карту");
            return;
        }
        
        if (cell == null)
        {
            Debug.LogError("Попытка разместить карту в null-ячейку");
            return;
        }
        
        try
        {
            Debug.Log($"Размещаем карту ИИ: L:{card.leftValue} R:{card.rightValue} T:{card.topValue} B:{card.bottomValue}");
            
            if (!aiCards.Contains(card))
            {
                Debug.LogError($"Карты нет в списке доступных карт ИИ!");
                return;
            }
            
            // Проверяем, активен ли объект карты
            if (card.gameObject == null)
            {
                Debug.LogError("GameObject карты равен null!");
                aiCards.Remove(card); // Удаляем невалидную карту из списка
                return;
            }
            
            // Проверяем, находится ли карта в правильном состоянии
            Debug.Log($"Статус карты перед размещением: активна = {card.gameObject.activeSelf}, родитель = {card.transform.parent?.name ?? "null"}");
            
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
            Debug.Log($"Карта удалена из списка карт ИИ. Осталось карт: {aiCards.Count}");

            // Сравниваем с соседями
            cell.CompareWithNeighbors(card);

            // Передаем ход игроку
            ChangeTurn.Instance.SwitchTurn(cell.gridPosition);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при размещении карты: {e.Message}\n{e.StackTrace}");
            
            // Удаляем проблемную карту из списка
            if (card != null && aiCards.Contains(card))
            {
                aiCards.Remove(card);
                Debug.Log("Проблемная карта удалена из списка");
            }
        }
    }

    public void ClearCards()
    {
        // Уничтожаем все существующие карты
        foreach (Card card in aiCards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }

        // Очищаем список
        aiCards.Clear();
        Debug.Log("Список карт ИИ очищен");
    }

    public void AddCard(Card card)
    {
        if (card == null)
        {
            Debug.LogError("Попытка добавить null-карту в список ИИ");
            return;
        }
        
        try
        {
            // Сохраняем ссылку на родительский объект, если это первая карта
            if (cardsParent == null && card.transform.parent != null)
            {
                cardsParent = card.transform.parent;
                Debug.Log($"Установлен родительский объект для карт ИИ: {cardsParent.name}");
            }
            
            // Проверяем, нет ли уже такой карты в списке
            if (aiCards.Contains(card))
            {
                Debug.LogWarning($"Карта уже присутствует в списке AI");
                return;
            }
            
            // Устанавливаем, что это карта ИИ
            card.isPlayer1Card = false;
            
            // Проверяем родителя карты
            string parentName = card.transform.parent?.name ?? "null";
            Debug.Log($"Добавляем карту AI. Текущий родитель: {parentName}");
            
            // Добавляем карту в список ИИ
            aiCards.Add(card);
            
            // Скрываем карту, пока ее не разместят
            card.gameObject.SetActive(true);
            
            Debug.Log($"Добавлена карта AI (L:{card.leftValue} R:{card.rightValue} T:{card.topValue} B:{card.bottomValue}). Всего карт: {aiCards.Count}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при добавлении карты: {e.Message}\n{e.StackTrace}");
        }
    }

    private void CleanupNullCards()
    {
        for (int i = aiCards.Count - 1; i >= 0; i--)
        {
            if (aiCards[i] == null)
            {
                aiCards.RemoveAt(i);
                Debug.Log($"Удалена null-ссылка из списка карт ИИ. Осталось карт: {aiCards.Count}");
            }
        }
    }
}
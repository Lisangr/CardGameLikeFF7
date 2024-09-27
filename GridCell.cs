using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IDropHandler
{
    public GridCell leftNeighbor;
    public GridCell rightNeighbor;
    public GridCell topNeighbor;
    public GridCell bottomNeighbor;

    public Vector2Int gridPosition;
    public CardsForTakeCardsAndCollection currentCardsForTakeCardsAndCollection;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    public delegate void CardPlacedHandler(Card card, GridCell cell, Vector2Int position);
    public static event CardPlacedHandler OnCardPlaced;

    void Start()
    {
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }

        if (!ColorUtility.TryParseHtmlString("#A3FF86", out customGreen))
        {
            Debug.LogError("Invalid color string for #A3FF86");
        }
    }

       public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableCard>())
        {
            DraggableCard draggableCard = droppedObject.GetComponent<DraggableCard>();

            // Проверяем, принадлежит ли карта активному игроку
            if ((draggableCard.card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) ||
                (!draggableCard.card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
            {
                Debug.LogWarning("Нельзя сбросить карту другого игрока.");
                return;
            }

            // Перемещаем карту в эту ячейку
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // Обновляем состояние ячейки, устанавливаем текущую карту
            currentCard = draggableCard.card;

            // Блокируем карту и меняем ход
            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            // Сравниваем новую карту с соседями
            CompareWithNeighbors(draggableCard.card);

            OnCardPlaced?.Invoke(currentCard, this, gridPosition);
        }
    }

    public void CompareWithNeighbors(Card newCard)
    {        
            //с левой картой
            if (leftNeighbor != null && leftNeighbor.HasCard())
            {
                Card leftCard = leftNeighbor.GetCard();
                Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of neighbor card: {leftCard.rightValue}");
                if (!currentCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
                {
                    Debug.Log("Changing player card color to green.");
                    leftNeighbor.ChangeCardColor(customGreen); // Окрашиваем карту игрока
                }
                else if (currentCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
                {
                    leftNeighbor.ChangeCardColor(customRed);
                }

            }
            //с правой картой
            if (rightNeighbor != null && rightNeighbor.HasCard())
            {
                Card rightCard = rightNeighbor.GetCard();
                Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of neighbor card: {rightCard.leftValue}");
                if (!currentCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
                {
                    Debug.Log("Changing player card color to green.");
                    rightNeighbor.ChangeCardColor(customGreen); // Окрашиваем карту игрока
                }
                else if (currentCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
                {
                    rightNeighbor.ChangeCardColor(customRed);
                }
            }
            //с верхней картой
            if (topNeighbor != null && topNeighbor.HasCard())
            {
                Card topCard = topNeighbor.GetCard();
                Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue}   vs top value of neighbor card:   {topCard.topValue}");
                if (!currentCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
                {
                    Debug.Log("Changing player card color to green.");
                    topNeighbor.ChangeCardColor(customGreen); // Окрашиваем карту игрока
                }
                else if (currentCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
                {
                    topNeighbor.ChangeCardColor(customRed);
                }
            }
            //с нижней картой
            if (bottomNeighbor != null && bottomNeighbor.HasCard())
            {
                Card bottomCard = bottomNeighbor.GetCard();
                Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of neighbor card: {bottomCard.bottomValue}");
                if (!currentCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
                {
                    Debug.Log("Changing player card color to green.");
                    bottomNeighbor.ChangeCardColor(customGreen); // Окрашиваем карту игрока
                }
                else if (currentCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
                {
                    bottomNeighbor.ChangeCardColor(customRed);
                }
            }        
    }
    
    public bool HasCard()
    {
        return currentCard != null;
    }

    public Card GetCard()
    {
        return currentCard;
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
                cardImage.SetAllDirty(); // Принудительное обновление цвета
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

}





















/*

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCell : MonoBehaviour, IDropHandler
{
    public GridCell leftNeighbor;
    public GridCell rightNeighbor;
    public GridCell topNeighbor;
    public GridCell bottomNeighbor;

    public Vector2Int gridPosition;
    public CardsForTakeCardsAndCollection currentCardsForTakeCardsAndCollection;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    public delegate void CardPlacedHandler(CardsForTakeCardsAndCollection card, GridCell cell, Vector2Int position);
    public static event CardPlacedHandler OnCardPlaced;

    void Start()
    {
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }

        if (!ColorUtility.TryParseHtmlString("#A3FF86", out customGreen))
        {
            Debug.LogError("Invalid color string for #A3FF86");
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.GetComponent<DraggableBattleCard>())
        {
            DraggableBattleCard draggableCard = droppedObject.GetComponent<DraggableBattleCard>();

            // Проверка на то, что карта уже размещена
            if (draggableCard.IsLocked())
            {
                Debug.LogWarning("Эта карта уже размещена и не может быть перемещена.");
                return;
            }

            // Проверка на то, что в ячейке нет карты
            if (HasCard())
            {
                Debug.LogWarning("Ячейка уже занята, невозможно разместить карту.");
                return;
            }

            // Остальная логика размещения карты
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            currentCardsForTakeCardsAndCollection = draggableCard.card;

            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            CompareWithNeighbors(draggableCard.card);

            OnCardPlaced?.Invoke(currentCardsForTakeCardsAndCollection, this, gridPosition);
        }
    }

    public void CompareWithNeighbors(CardsForTakeCardsAndCollection newCard)
    {
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection leftCard = leftNeighbor.GetPlayerCard();
            if (leftCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of neighbor card: {leftCard.rightValue}");
                if (!newCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
                {
                    leftNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
                {
                    leftNeighbor.ChangeCardColor(customRed);
                }
            }
        }

        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection rightCard = rightNeighbor.GetPlayerCard();
            if (rightCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of neighbor card: {rightCard.leftValue}");
                if (!newCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
                {
                    rightNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
                {
                    rightNeighbor.ChangeCardColor(customRed);
                }
            }
        }

        if (topNeighbor != null && topNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection topCard = topNeighbor.GetPlayerCard();
            if (topCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue} vs top value of neighbor card: {topCard.topValue}");
                if (!newCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
                {
                    topNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
                {
                    topNeighbor.ChangeCardColor(customRed);
                }
            }
        }

        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            CardsForTakeCardsAndCollection bottomCard = bottomNeighbor.GetPlayerCard();
            if (bottomCard != null)  // Добавляем проверку на null
            {
                Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of neighbor card: {bottomCard.bottomValue}");
                if (!newCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
                {
                    bottomNeighbor.ChangeCardColor(customGreen);
                }
                else if (newCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
                {
                    bottomNeighbor.ChangeCardColor(customRed);
                }
            }
        }
    }

    public bool HasEnemyCard()
    {
        return currentCard != null && !currentCard.isPlayer1Card;
    }

    public bool HasCard()
    {
        return currentCard != null;
    }

    public Card GetCard()
    {
        return currentCard;
    }
    public CardsForTakeCardsAndCollection GetPlayerCard()
    {
        return currentCardsForTakeCardsAndCollection;
    }
    public void ChangeCardColor(Color color)
    {
        if (HasCard())
        {
            Image cardImage = transform.GetChild(0).GetComponent<Image>();
            if (cardImage != null)
            {
                Debug.Log($"Changing card color to {color}.");
                cardImage.color = color;
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
}*/
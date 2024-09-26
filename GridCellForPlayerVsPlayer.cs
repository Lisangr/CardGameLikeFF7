using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCellForPlayerVsPlayer : MonoBehaviour, IDropHandler
{
    public GridCellForPlayerVsPlayer leftNeighbor;
    public GridCellForPlayerVsPlayer rightNeighbor;
    public GridCellForPlayerVsPlayer topNeighbor;
    public GridCellForPlayerVsPlayer bottomNeighbor;

    public Vector2Int gridPosition;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;


    void Start()
    {
        // ����������� ��������� ���� � ������ Color
        if (!ColorUtility.TryParseHtmlString("#FF9494", out customRed))
        {
            Debug.LogError("Invalid color string for #FF9494");
        }

        // ����������� ��������� ���� � ������ Color
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

            // ���������, ����������� �� ����� ��������� ������
            if ((draggableCard.card.isPlayer1Card && !ChangeTurn.IsPlayer1Turn) ||
                (!draggableCard.card.isPlayer1Card && ChangeTurn.IsPlayer1Turn))
            {
                Debug.LogWarning("������ �������� ����� ������� ������.");
                return;
            }

            // ���������� ����� � ��� ������
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            // ��������� ��������� ������, ������������� ������� �����
            currentCard = draggableCard.card;

            // ��������� ����� � ������ ���
            draggableCard.LockCard();
            ChangeTurn.Instance.SwitchTurn(gridPosition);

            // ���������� ����� ����� � ��������
            CompareWithNeighbors(draggableCard.card);
        }
    }
    public void CompareWithNeighbors(Card newCard)
    {
        //� ����� ������
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of neighbor card: {leftCard.rightValue}");
            if (!currentCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
            {
                Debug.Log("Changing player card color to green.");
                leftNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
            {
                leftNeighbor.ChangeCardColor(customRed);
            }
        }
        //� ������ ������
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of neighbor card: {rightCard.leftValue}");
            if (!currentCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
            {
                Debug.Log("Changing player card color to green.");
                rightNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
            {
                rightNeighbor.ChangeCardColor(customRed);
            }
        }
        //� ������� ������
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue}   vs top value of neighbor card:   {topCard.topValue}");
            if (!currentCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
            {
                Debug.Log("Changing player card color to green.");
                topNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
            {
                topNeighbor.ChangeCardColor(customRed);
            }
        }
        //� ������ ������
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of neighbor card: {bottomCard.bottomValue}");
            if (!currentCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
            {
                Debug.Log("Changing player card color to green.");
                bottomNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
            {
                bottomNeighbor.ChangeCardColor(customRed);
            }
        }
    }
    public void CompareWithNeighborsAI(CardsForTakeCardsAndCollection newCard)
    {
        //� ����� ������
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            Debug.Log($"Comparing right value of new card: {newCard.leftValue} vs left value of neighbor card: {leftCard.rightValue}");
            if (!currentCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
            {
                Debug.Log("Changing player card color to green.");
                leftNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.leftValue > leftCard.rightValue)
            {
                leftNeighbor.ChangeCardColor(customRed);
            }
        }
        //� ������ ������
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            Debug.Log($"Comparing left value of new card: {newCard.rightValue} vs right value of neighbor card: {rightCard.leftValue}");
            if (!currentCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
            {
                Debug.Log("Changing player card color to green.");
                rightNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.rightValue > rightCard.leftValue)
            {
                rightNeighbor.ChangeCardColor(customRed);
            }
        }
        //� ������� ������
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            Debug.Log($"Comparing bottom value of new card: {newCard.bottomValue}   vs top value of neighbor card:   {topCard.topValue}");
            if (!currentCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
            {
                Debug.Log("Changing player card color to green.");
                topNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
            }
            else if (currentCard.isPlayer1Card && newCard.bottomValue > topCard.topValue)
            {
                topNeighbor.ChangeCardColor(customRed);
            }
        }
        //� ������ ������
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            Debug.Log($"Comparing top value of new card: {newCard.topValue} vs bottom value of neighbor card: {bottomCard.bottomValue}");
            if (!currentCard.isPlayer1Card && newCard.topValue > bottomCard.bottomValue)
            {
                Debug.Log("Changing player card color to green.");
                bottomNeighbor.ChangeCardColor(customGreen); // ���������� ����� ������
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
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCellForPlayerVsPlayer : MonoBehaviour, IDropHandler
{
    public GridCellForPlayerVsPlayer leftNeighbor;
    public GridCellForPlayerVsPlayer rightNeighbor;
    public GridCellForPlayerVsPlayer topNeighbor;
    public GridCellForPlayerVsPlayer bottomNeighbor;
    //public CardManager cardManager;

    public Vector2Int gridPosition;
    public Card currentCard;
    private Color customRed;
    private Color customGreen;

    public delegate void CounterForPlayer();
    public static event CounterForPlayer OnCardTookByPlayer1;
    public static event CounterForPlayer OnCardTookByPlayer2;

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
        // ���������, ��� � ������ ��� ��� �����
        if (currentCard != null)
        {
            Debug.LogWarning("������ ��� ������ ������.");
            return;
        }

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
        // � ����� ������
        if (leftNeighbor != null && leftNeighbor.HasCard())
        {
            Card leftCard = leftNeighbor.GetCard();
            if (CanBeatCard(newCard, leftCard, newCard.leftValue, leftCard.rightValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    leftNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    leftNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // � ������ ������
        if (rightNeighbor != null && rightNeighbor.HasCard())
        {
            Card rightCard = rightNeighbor.GetCard();
            if (CanBeatCard(newCard, rightCard, newCard.rightValue, rightCard.leftValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    rightNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    rightNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // � ������� ������
        if (topNeighbor != null && topNeighbor.HasCard())
        {
            Card topCard = topNeighbor.GetCard();
            if (CanBeatCard(newCard, topCard, newCard.bottomValue, topCard.topValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    topNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    topNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }

        // � ������ ������
        if (bottomNeighbor != null && bottomNeighbor.HasCard())
        {
            Card bottomCard = bottomNeighbor.GetCard();
            if (CanBeatCard(newCard, bottomCard, newCard.topValue, bottomCard.bottomValue))
            {
                if (!newCard.isPlayer1Card)
                {
                    bottomNeighbor.ChangeCardColor(customGreen);
                    OnCardTookByPlayer2?.Invoke();
                }
                else
                {
                    bottomNeighbor.ChangeCardColor(customRed);
                    OnCardTookByPlayer1?.Invoke();
                }
            }
        }
    }

    // ����� ��� ��������, ����� �� ����� ����� ������ ��������
    private bool CanBeatCard(Card newCard, Card neighborCard, int newCardValue, int neighborCardValue)
    {
        // ���� ����� ���� ��������� (�������� ����), ��������� ��������, ���� ���� ����� ���� �� ������
        bool wasBeaten = IsCardColored(neighborCard);

        // ���� ����� ���� �� ������ � �� ���� ��������� � �� ����������
        if (newCard.isPlayer1Card == neighborCard.isPlayer1Card && !wasBeaten)
        {
            return false;
        }

        // ��������� �������� ����, ����� �� ��������
        return newCardValue > neighborCardValue;
    }

    // ���������, ���� �� ����� ��� ��������� (��������)
    private bool IsCardColored(Card neighborCard)
    {
        Image neighborCardImage = neighborCard.GetComponent<Image>();
        if (neighborCardImage != null)
        {
            return neighborCardImage.color == customRed || neighborCardImage.color == customGreen;
        }
        return false;
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
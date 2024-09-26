using UnityEngine;
using UnityEngine.UI;

public class CardsForTakeCardsAndCollection : MonoBehaviour
{
    [Header("Card Values")]
    public int leftValue;   // �������� ��� ������ ����
    public int rightValue;  // �������� ��� ������� ����
    public int topValue;    // �������� ��� �������� ����
    public int bottomValue; // �������� ��� ������� ����
    public bool isPlayer1Card; // ���������, ��� ��� ����� ������ 1

    [Header("UI Elements")]
    public Image cardImage;      // UI ������� ��� ����������� �����
    public Text leftText;        // ��������� ���� ��� �������� �����
    public Text rightText;
    public Text topText;
    public Text bottomText;

    [Header("Image Data")]
    public Sprite[] cardImages;  // ������ � ���������� ������������� ����
    public int imageIndex;       // ���� ��� ������� �����������
    public string spriteName;    // ���� ��� ����� ������� (���������)

    // ����� ��� ������������� ����� � �������
    public void InitializeCard(int left, int right, int top, int bottom, bool isPlayer1, string spriteName)
    {
        leftValue = left;
        rightValue = right;
        topValue = top;
        bottomValue = bottom;
        isPlayer1Card = isPlayer1;

        // ��������� ��� ������� ��� ����������� �������������
        this.spriteName = spriteName;

        // ������ ������ �� ����� � ������� cardImages
        Sprite sprite = System.Array.Find(cardImages, s => s.name == spriteName);

        // ���� ������ ������, ����������� ���
        if (sprite != null)
        {
            cardImage.sprite = sprite;
            Debug.Log($"����� ���������������� � ������������: {spriteName}");
        }
        else
        {
            Debug.LogError($"����������� � ������ {spriteName} �� �������.");
        }

        // ��������� ��������� �������� �� �����
        SetCard();
    }

    // ����� ��� ���������� ����������� ������ �� �����
    public void SetCard()
    {
        leftText.text = leftValue.ToString();
        rightText.text = rightValue.ToString();
        topText.text = topValue.ToString();
        bottomText.text = bottomValue.ToString();
    }
}
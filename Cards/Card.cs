
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Values")]
    public int leftValue;   // �������� ��� ������ ����
    public int rightValue;  // �������� ��� ������� ����
    public int topValue;    // �������� ��� �������� ����
    public int bottomValue; // �������� ��� ������� ����
    public GameObject cardPrefab;
    public Image cardImage;
    public bool isPlayer1Card; // ���������, ��� ��� ����� ������ 1

    [Header("Image Values")]
    public Sprite[] cardImages; // ������ � ���������� ������������� ����

    [Header("Text Values")]
    public Text leftText;    // ��������� ���� ��� �������� �����
    public Text rightText;
    public Text topText;
    public Text bottomText;

    void Start()
    {
        // ����������� ��������� �������� ��� ����� �� 1 �� 9
        leftValue = Random.Range(1, 10);   // �������� � Unity: �� 1 �� 9 ������������
        rightValue = Random.Range(1, 10);
        topValue = Random.Range(1, 10);
        bottomValue = Random.Range(1, 10);

        // ��������� ����� ����������� �����
        int randomIndex = Random.Range(0, cardImages.Length); // �������� ��������� �����������
        cardImage.sprite = cardImages[randomIndex];           // ������������� ��������� �����������

        SetCard();
    }

    // ����� ��� ��������� ������ ����� � �� �����������
    public void SetCard()
    {
        leftText.text = leftValue.ToString();
        rightText.text = rightValue.ToString();
        topText.text = topValue.ToString();
        bottomText.text = bottomValue.ToString();
    }
}
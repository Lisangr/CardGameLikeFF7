
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Values")]
    public int leftValue;   // Значение для левого поля
    public int rightValue;  // Значение для правого поля
    public int topValue;    // Значение для верхнего поля
    public int bottomValue; // Значение для нижнего поля
    public GameObject cardPrefab;
    public Image cardImage;
    public bool isPlayer1Card; // Указываем, что это карта игрока 1

    [Header("Image Values")]
    public Sprite[] cardImages; // Массив с возможными изображениями карт

    [Header("Text Values")]
    public Text leftText;    // Текстовые поля для значений карты
    public Text rightText;
    public Text topText;
    public Text bottomText;

    void Start()
    {
        // Определение случайных значений для полей от 1 до 9
        leftValue = Random.Range(1, 10);   // Диапазон в Unity: от 1 до 9 включительно
        rightValue = Random.Range(1, 10);
        topValue = Random.Range(1, 10);
        bottomValue = Random.Range(1, 10);

        // Случайный выбор изображения карты
        int randomIndex = Random.Range(0, cardImages.Length); // Выбираем случайное изображение
        cardImage.sprite = cardImages[randomIndex];           // Устанавливаем выбранное изображение

        SetCard();
    }

    // Метод для установки данных карты и их отображения
    public void SetCard()
    {
        leftText.text = leftValue.ToString();
        rightText.text = rightValue.ToString();
        topText.text = topValue.ToString();
        bottomText.text = bottomValue.ToString();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class CardsForTakeCardsAndCollection : MonoBehaviour
{
    [Header("Card Values")]
    public int leftValue;   // Значение для левого поля
    public int rightValue;  // Значение для правого поля
    public int topValue;    // Значение для верхнего поля
    public int bottomValue; // Значение для нижнего поля
    public bool isPlayer1Card; // Указываем, что это карта игрока 1

    [Header("UI Elements")]
    public Image cardImage;      // UI элемент для изображения карты
    public Text leftText;        // Текстовые поля для значений карты
    public Text rightText;
    public Text topText;
    public Text bottomText;

    [Header("Image Data")]
    public Sprite[] cardImages;  // Массив с возможными изображениями карт
    public int imageIndex;       // Поле для индекса изображения
    public string spriteName;    // Поле для имени спрайта (добавлено)

    // Метод для инициализации карты с данными
    public void InitializeCard(int left, int right, int top, int bottom, bool isPlayer1, string spriteName)
    {
        leftValue = left;
        rightValue = right;
        topValue = top;
        bottomValue = bottom;
        isPlayer1Card = isPlayer1;

        // Сохраняем имя спрайта для дальнейшего использования
        this.spriteName = spriteName;

        // Найдем спрайт по имени в массиве cardImages
        Sprite sprite = System.Array.Find(cardImages, s => s.name == spriteName);

        // Если спрайт найден, присваиваем его
        if (sprite != null)
        {
            cardImage.sprite = sprite;
            Debug.Log($"Карта инициализирована с изображением: {spriteName}");
        }
        else
        {
            Debug.LogError($"Изображение с именем {spriteName} не найдено.");
        }

        // Обновляем текстовые значения на карте
        SetCard();
    }

    // Метод для обновления отображения текста на карте
    public void SetCard()
    {
        leftText.text = leftValue.ToString();
        rightText.text = rightValue.ToString();
        topText.text = topValue.ToString();
        bottomText.text = bottomValue.ToString();
    }
}
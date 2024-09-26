using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class CardData
{
    public int leftValue;
    public int rightValue;
    public int topValue;
    public int bottomValue;
    public bool isPlayer1Card;
    public string spriteName; // Убираем imageIndex и сохраняем только имя спрайта
}

public class TakeCards : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform gridParent;
    public List<CardsForTakeCardsAndCollection> spawnedCards = new List<CardsForTakeCardsAndCollection>();

    private const string CardsFileName = "savedCards.json";

    private void OnEnable()
    {
        StartCoroutine(GenerateCardsAfterDelay(1f));
    }

    private IEnumerator GenerateCardsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GenerateCards();
    }

    private void GenerateCards()
    {
        // Загрузить уже существующие карты перед генерацией
        List<CardData> existingCards = LoadExistingCards();

        ClearCards();

        // Загрузим все возможные спрайты
        Sprite[] availableSprites = Resources.LoadAll<Sprite>("CardImages");

        // Генерация новых карт
        for (int i = 0; i < 10; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridParent);
            CardsForTakeCardsAndCollection cardComponent = newCard.GetComponent<CardsForTakeCardsAndCollection>();

            int leftValue = Random.Range(1, 10);
            int rightValue = Random.Range(1, 10);
            int topValue = Random.Range(1, 10);
            int bottomValue = Random.Range(1, 10);
            bool isPlayer1 = true;

            // Рандомно выбираем спрайт из доступных
            Sprite selectedSprite = availableSprites[Random.Range(0, availableSprites.Length)];

            // Инициализируем карту с использованием имени спрайта
            cardComponent.InitializeCard(leftValue, rightValue, topValue, bottomValue, isPlayer1, selectedSprite.name);

            spawnedCards.Add(cardComponent);

            // Добавляем новую карту в существующий список
            existingCards.Add(new CardData
            {
                leftValue = leftValue,
                rightValue = rightValue,
                topValue = topValue,
                bottomValue = bottomValue,
                isPlayer1Card = isPlayer1,
                spriteName = selectedSprite.name  // Сохраняем имя спрайта
            });
        }

        // Сохранить обновленный список карт
        SaveCards(existingCards);

        Debug.Log("Карты сгенерированы и добавлены в список.");
    }

    private List<CardData> LoadExistingCards()
    {
        if (!File.Exists(GetFilePath()))
        {
            return new List<CardData>(); // Возвращаем пустой список, если файла нет
        }

        string json = File.ReadAllText(GetFilePath());
        return JsonUtility.FromJson<Serialization<List<CardData>>>(json).target;
    }

    private void SaveCards(List<CardData> cardDataList)
    {
        string json = JsonUtility.ToJson(new Serialization<List<CardData>>(cardDataList), true);
        File.WriteAllText(GetFilePath(), json);
        Debug.Log("Карты успешно сохранены в файл.");
    }

    private void ClearCards()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        spawnedCards.Clear();
        Debug.Log("Старые карты удалены.");
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CardsFileName);
    }
}

// Вспомогательный класс для сериализации List в JSON
[System.Serializable]
public class Serialization<T>
{
    public T target;
    public Serialization(T target) { this.target = target; }
}






/*
private void GenerateCards()
{
    // Загрузить уже существующие карты перед генерацией
    List<CardData> existingCards = LoadExistingCards();

    ClearCards();

    // Генерация новых карт
    for (int i = 0; i < 10; i++)
    {
        GameObject newCard = Instantiate(cardPrefab, gridParent);
        CardsForTakeCardsAndCollection cardComponent = newCard.GetComponent<CardsForTakeCardsAndCollection>();

        int leftValue = Random.Range(1, 10);
        int rightValue = Random.Range(1, 10);
        int topValue = Random.Range(1, 10);
        int bottomValue = Random.Range(1, 10);
        bool isPlayer1 = true;
        int imageIndex = Random.Range(0, cardComponent.cardImages.Length);

        cardComponent.InitializeCard(leftValue, rightValue, topValue, bottomValue, isPlayer1, imageIndex);

        spawnedCards.Add(cardComponent);

        // Добавляем новую карту в существующий список
        existingCards.Add(new CardData
        {
            leftValue = leftValue,
            rightValue = rightValue,
            topValue = topValue,
            bottomValue = bottomValue,
            isPlayer1Card = isPlayer1,
            imageIndex = imageIndex
        });
    }

    // Сохранить обновленный список карт
    SaveCards(existingCards);

    Debug.Log("Карты сгенерированы и добавлены в список.");
}
*/
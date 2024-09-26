using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    private DraggableCard draggableCard; // Компонент для перетаскивания

    private void Awake()
    {
        draggableCard = GetComponent<DraggableCard>();
    }

    // Метод для обновления цвета карты
    public void SetCardColor(Color newColor)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = newColor;
        }
    }
}

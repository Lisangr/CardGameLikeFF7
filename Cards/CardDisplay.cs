using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    private DraggableCard draggableCard; // ��������� ��� ��������������

    private void Awake()
    {
        draggableCard = GetComponent<DraggableCard>();
    }

    // ����� ��� ���������� ����� �����
    public void SetCardColor(Color newColor)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = newColor;
        }
    }
}

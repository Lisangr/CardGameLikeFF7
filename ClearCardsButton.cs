using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ClearCardsButton : MonoBehaviour
{
    private const string CardsFileName = "savedCards.json"; // ��� �����

    void Start()
    {
        // ������� ������ � ��������� �� ��������
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ClearCardData);
        }
    }

    // �����, ������� ���������� ��� ������� ������
    public void ClearCardData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, CardsFileName);

        // ���������, ���������� �� ����
        if (File.Exists(filePath))
        {
            // ���������� ������ ������ � ����
            File.WriteAllText(filePath, JsonUtility.ToJson(new Serialization<List<CardData>>(new List<CardData>()), true));
            Debug.Log("JSON-���� ������.");
        }
        else
        {
            Debug.LogWarning("���� �� ������, ������ �������.");
        }
    }
}

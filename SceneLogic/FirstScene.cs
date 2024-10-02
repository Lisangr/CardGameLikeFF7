using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class StartGame : MonoBehaviour
{
    public GameObject settingsPanel;
    private void Awake()
    {
        OnPanelClosePanel();
    }
    public void OnPanelClosePanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    public void OnClickStart(string scene)
    {
        /*AudioSource audioSource = FindAnyObjectByType<AudioSource>().GetComponent<AudioSource>();
        if (audioSource != null) { Debug.Log("��������� ������ ������ ���� �� �����"); }
        Time.timeScale = 0f;
        audioSource.Pause();*/

        //YandexGame.FullscreenShow();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    public void OnPanelClick()
    { 
        settingsPanel.SetActive(true);
    }
}


/*
 * using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class StartGame : MonoBehaviour
{
    public GameObject panel;
    public GameObject panelCollection;
    public GameObject panelPrepairForBattle;
    public GameObject button;

    public Transform battleCellsParent; // �������� ��� �����, ���������� �����
    private const string CardsFileName = "savedBattleCards.json"; // ��� ����� ��� ���������� ����

    private void SaveBattleCards()
    {
        List<CardData> battleCardDataList = new List<CardData>();

        Sprite[] cardImages = Resources.LoadAll<Sprite>("CardImages");
        System.Array.Sort(cardImages, (x, y) => string.Compare(x.name, y.name));

        foreach (Transform cell in battleCellsParent)
        {
            var prepairCell = cell.GetComponent<PrepairForBattleCell>();

            if (prepairCell != null && prepairCell.currentCard != null)
            {
                CardsForTakeCardsAndCollection card = prepairCell.currentCard;

                string spriteName = card.cardImage.sprite.name;  // �������� ��� �������

                // ������� ������ �����
                CardData cardData = new CardData
                {
                    leftValue = card.leftValue,
                    rightValue = card.rightValue,
                    topValue = card.topValue,
                    bottomValue = card.bottomValue,
                    isPlayer1Card = card.isPlayer1Card,
                    spriteName = spriteName  // ��������� ��� �����������
                };
                Debug.Log($"��������� ����� � ������������: {spriteName}");

                battleCardDataList.Add(cardData);
                Debug.Log($"����� ���������: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}, ImageName:{spriteName}");
            }
        }

        string json = JsonUtility.ToJson(new Serialization<List<CardData>>(battleCardDataList));
        File.WriteAllText(GetFilePath(), json);
    }

    // ����� ��� ������ ������� �������
    private int FindSpriteIndex(Sprite[] cardImages, Sprite sprite)
    {
        for (int i = 0; i < cardImages.Length; i++)
        {
            if (cardImages[i].name == sprite.name) // ���������� �� �����
            {
                return i;
            }
        }
        return -1; // ���� ������ �� ������
    }

    // �������� ���� � ����� ����������
    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CardsFileName);
    }

    private void Awake()
    {
        panel.SetActive(false);
        panelCollection.SetActive(false);
        panelPrepairForBattle.SetActive(false);
    }

    public void OnClickStart(string scene)
    {
        SaveBattleCards(); // ��������� ����� ����� ������� ����
        YandexGame.FullscreenShow();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void OnClickShowPanel()
    {
        panel.SetActive(true);
        button.SetActive(false);
    }

    public void OnClickShowCollectionPanel()
    {
        panelCollection.SetActive(true);
        button.SetActive(false);
    }

    public void OnClickShowPrepairForBattlePanel()
    {
        panelPrepairForBattle.SetActive(true);
        button.SetActive(false);
    }

    public void ClosePrepairForBattlePanel()
    { 
        panelPrepairForBattle.SetActive(false);
        button.SetActive(true);
    }
}


*/
/*
// ��������� ������� ��������� ����, ������� ��������� �� ������� ��� ���
private void SaveBattleCards()
{
    List<CardData> battleCardDataList = new List<CardData>();

    // ��������� ��� ����������� ���� �� ��������
    Sprite[] cardImages = Resources.LoadAll<Sprite>("CardImages");
    Debug.Log($"��������� {cardImages.Length} ����������� ��� ����.");

    foreach (Transform cell in battleCellsParent)
    {
        var prepairCell = cell.GetComponent<PrepairForBattleCell>();

        if (prepairCell != null && prepairCell.currentCard != null)
        {
            CardsForTakeCardsAndCollection card = prepairCell.currentCard;

            // ����� ������ ����������� � ������� ����������� �����������
            int imageIndex = System.Array.IndexOf(cardImages, card.cardImage.sprite);

            if (imageIndex == -1)
            {
                Debug.LogError($"����������� ����� �� ������� � ����������� �������� ��� ����� � ����������: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}");
                continue;
            }

            // ������� ������ �����
            CardData cardData = new CardData
            {
                leftValue = card.leftValue,
                rightValue = card.rightValue,
                topValue = card.topValue,
                bottomValue = card.bottomValue,
                isPlayer1Card = card.isPlayer1Card,
                imageIndex = imageIndex // ���������� ��������� ������ �����������
            };

            battleCardDataList.Add(cardData);
            Debug.Log($"����� ���������: L:{card.leftValue}, R:{card.rightValue}, T:{card.topValue}, B:{card.bottomValue}, ImageIndex:{imageIndex}");
        }
    }

    // ��������� ������ ���� � ����
    string json = JsonUtility.ToJson(new Serialization<List<CardData>>(battleCardDataList));
    File.WriteAllText(GetFilePath(), json);
    Debug.Log($"��������� ����� ��� ���: {battleCardDataList.Count} ����.");
}
*/
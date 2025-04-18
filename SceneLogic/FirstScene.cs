using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    public void OnPanelClick()
    { 
        settingsPanel.SetActive(true);
    }
}
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public GameObject button;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.GetKey(KeyCode.Escape))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), clickPosition))
            {
                gameObject.SetActive(false);
                button.SetActive(true);
            }
        }
    }
}

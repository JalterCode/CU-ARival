using UnityEngine;
using UnityEngine.UI;

public class HideButton : MonoBehaviour
{
    public Button targetButton;

    void Start()
    {
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(HideElement);
        }
    }

    public void HideElement()
    {
        if (targetButton != null)
        {
            targetButton.gameObject.SetActive(false);
        }

        if (targetButton.gameObject.name == "ArrivedUI") {
            
        }
    }
}

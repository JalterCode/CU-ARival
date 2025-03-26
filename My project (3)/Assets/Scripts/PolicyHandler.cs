using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PolicyHandler : MonoBehaviour
{
    public Button acceptButton;

    void Start()
    {
        acceptButton.onClick.AddListener(AcceptPolicy);
    }

    void AcceptPolicy()
    {
        SceneManager.UnloadSceneAsync("PolicyScene");
    }
}
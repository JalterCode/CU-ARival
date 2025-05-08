using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        //loads opens scene when building is selected
        SceneManager.LoadScene(sceneName);
    }
}

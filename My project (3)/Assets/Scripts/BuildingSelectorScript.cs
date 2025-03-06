using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class BuildingSelectorScript : MonoBehaviour
{
    //public Button NinediyanButton;
    public Button CanalButton;
    //public Button MackenzieButton;
    //public Button ToryButton;
    // Start is called before the first frame update
    void Start()
    {
       // NinediyanButton.onClick.AddListener(() => OnNavigateButtonClicked(NinediyanButton));
        CanalButton.onClick.AddListener(() => OnNavigateButtonClicked(CanalButton));
       // MackenzieButton.onClick.AddListener(() => OnNavigateButtonClicked(MackenzieButton));
        //ToryButton.onClick.AddListener(() => OnNavigateButtonClicked(ToryButton));
        
        
    }
    private void OnNavigateButtonClicked(Button button){

        if (button == CanalButton)
        {
            SceneManager.LoadScene("NavScene", LoadSceneMode.Single);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
public class BuildingSelectorScript : MonoBehaviour
{
    public Button CanalButton;
    public Button MackenzieButton;
    public Button NinediyanButton;
    public Button ToryButton;
    
    // Start is called before the first frame update
    void Start()
    {
        CanalButton.onClick.AddListener(() => OnNavigateButtonClicked(CanalButton));
        MackenzieButton.onClick.AddListener(() => OnNavigateButtonClicked(MackenzieButton));
        NinediyanButton.onClick.AddListener(() => OnNavigateButtonClicked(NinediyanButton));
        ToryButton.onClick.AddListener(() => OnNavigateButtonClicked(ToryButton));
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnNavigateButtonClicked(Button button){}
}

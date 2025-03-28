using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisclaimerScript : MonoBehaviour
{
    public Button acceptButton;
    public GameObject disclaimer;
    // Start is called before the first frame update
    void Start()
    {
        acceptButton.onClick.AddListener(() => Accept());
    }

    // Update is called once per frame
    void Accept(){
        disclaimer.SetActive(false);
    }
}

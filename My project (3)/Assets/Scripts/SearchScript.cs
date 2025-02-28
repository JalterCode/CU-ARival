using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class SearchScript : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    public RectTransform panel;
    // Start is called before the first frame update
    private List<Transform> roomButtons;

    void Start()
    {
        roomButtons = new List<Transform>();
        InitializeRoomButtons();
    }

    private void InitializeRoomButtons()
    {
        foreach(Transform child in panel) {
            if(char.IsDigit(child.gameObject.name[0]) && !roomButtons.Contains(child)) {
                roomButtons.Add(child);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        string result = inputField.text.Length > 1 ? inputField.text.Trim().Replace("\u200B", "") : "";
        if(!string.IsNullOrWhiteSpace(result)) {
            foreach(Transform child in roomButtons) {
                child.gameObject.SetActive(child.gameObject.name.StartsWith(result.Trim()));  
            }
        } else {
            foreach(Transform child in roomButtons) {
                child.gameObject.SetActive(true);  
            }
        }
    }
}
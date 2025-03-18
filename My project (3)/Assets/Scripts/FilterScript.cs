using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI; 
using System;

public class FilterScript : MonoBehaviour
{
    public RectTransform panel;
    private List<Transform> roomButtons;    
    public Button filterButton;
    public GameObject filterListPanel;
    public Button closeFilterList;
    public Button filterFavorite;
    private bool filterToggle = false;

    void Start()
    {
        filterListPanel.SetActive(false);
        roomButtons = new List<Transform>();
        InitializeRoomButtons();
        filterButton.onClick.AddListener(() => enableFilter());
        closeFilterList.onClick.AddListener(() => closeFilter());
        filterFavorite.onClick.AddListener(() => Favorite());
    }

    private void InitializeRoomButtons()
    {
        foreach(Transform child in panel) {
            if(char.IsDigit(child.gameObject.name[0]) && !roomButtons.Contains(child)) {
                roomButtons.Add(child);
            }
        }
    }

    void Favorite()
    {
        if (filterToggle ==false) 
        {
            foreach (Transform child in roomButtons)
            {
                ButtonADT buttonADT = child.GetComponent<ButtonADT>();
                
                child.gameObject.SetActive(true); 
                
            }
            filterToggle = true;
        }
        else{
            foreach(Transform child in panel) {
                child.gameObject.SetActive(false);  
            }
            filterToggle =false;
        }
    }
    
    void enableFilter(){
        filterListPanel.SetActive(true);
    }
    void closeFilter(){
        filterListPanel.SetActive(false);
    }
}

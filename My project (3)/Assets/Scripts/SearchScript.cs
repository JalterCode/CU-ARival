using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SearchScript : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    public RectTransform panel;
    // Start is called before the first frame update
    private List<Transform> roomButtons;
    private string previousInput = "";

    //filter
    public Button filterButton;
    public GameObject filterListPanel;
    public Button closeFilterList;
    public Button filterFavorite;
    private bool filterToggle = false;



    void Start()
    {
        filterListPanel.SetActive(false);
        roomButtons = new List<Transform>();
        previousInput = inputField.text;
        InitializeRoomButtons();

        filterButton.onClick.AddListener(() => enableFilter());
        closeFilterList.onClick.AddListener(() => closeFilter());
        filterFavorite.onClick.AddListener(() => Favorite());
        foreach(Transform but in roomButtons){
            Button buttonComponent = but.GetComponent<Button>(); 
            Transform starImage = but.Find("Starlmage"); 
            Button starButton = starImage.AddComponent<Button>();
            starButton.onClick.AddListener(() => StarButton(but));
        }
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
            filterToggle = true;
        }
        else{
            filterToggle =false;
        }
    }
    
    void enableFilter(){
        filterListPanel.SetActive(true);
    }
    void closeFilter(){
        filterListPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        string currentInput = inputField.text.Trim().Replace("\u200B", "");

        if (currentInput != previousInput)
        {
            previousInput = currentInput; // Update previousInput

            string result = currentInput.Length > 1 ? currentInput : "";

            if (!string.IsNullOrWhiteSpace(result))
            {
                foreach (Transform child in roomButtons)
                {
                    if(filterToggle){
                        child.gameObject.SetActive(child.gameObject.name.StartsWith(result.Trim())); // && child.Favorite
                    }else{
                        child.gameObject.SetActive(child.gameObject.name.StartsWith(result.Trim())); 
                    }
                }
            }
            else
            {
                foreach (Transform child in roomButtons)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }

    void StarButton(Transform child){
        Transform starlmage = child.Find("Starlmage"); 
        Image image = starlmage.GetComponent<Image>();
        image.color = Color.yellow;
        ButtonADT buttonADT = child.GetComponent<ButtonADT>();
        buttonADT.Favorite = true;
    }
}
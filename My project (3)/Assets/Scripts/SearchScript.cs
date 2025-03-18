using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class SearchScript : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    public RectTransform panel;
    // Start is called before the first frame update
    private List<Transform> roomButtons;


    //filter
    public Button filterButton;
    public GameObject filterListPanel;
    public Button closeFilterList;
    public Button filterFavorite;
    private bool filterToggle = false;
    public Sprite starSprite;
    public Sprite yellowStarSprite;

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
        if (filterToggle ==false){
            filterToggle = true;
            filterFavorite.GetComponent<Image>().color = Color.green;
            }
        else{
            filterToggle =false;
            filterFavorite.GetComponent<Image>().color = Color.white;
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
        string result = currentInput.Length > 0 ? currentInput : "";

        

        if (!string.IsNullOrWhiteSpace(result))
        {
            foreach (Transform child in roomButtons)
            {
                Transform starImageTransform = child.Find("StarImage");
                Image starImage = starImageTransform.GetComponent<Image>();
                if (filterToggle)
                {
                    child.gameObject.SetActive(starImage.sprite == yellowStarSprite && child.gameObject.name.StartsWith(result.Trim()));
                }
                else
                {
                    child.gameObject.SetActive(child.gameObject.name.StartsWith(result.Trim()));
                }
            }
        }
        else
        {
            foreach (Transform child in roomButtons)
            {
                Transform starImageTransform = child.Find("StarImage");
                Image starImage = starImageTransform.GetComponent<Image>();
                if (filterToggle)
                {
                    child.gameObject.SetActive(starImage.sprite == yellowStarSprite);
                }
                else
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
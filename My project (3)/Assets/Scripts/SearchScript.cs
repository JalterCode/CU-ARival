using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using UnityEditor.VersionControl;

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

    // schedule
    
    public Button scheduleButton;
    public GameObject schedulePanel;
    public Button closeSchedulePanel;
    public TextMeshProUGUI className;
    public TextMeshProUGUI startTime;
    public TextMeshProUGUI endTime;
    public Button addClass;

    //days of week
    
    public GameObject daysOfWeekPanel;
    public Button closeDaysOfWeekPanel;
    public Button Mon;
    public Button Tue;
    public Button Wed;
    public Button Thu;
    public Button Fri;
    public Button submitButton;
    private bool mondayToggle = false;
    private bool tuesdayToggle = false;
    private bool wednesdayToggle = false;
    private bool thursdayToggle = false;
    private bool fridayToggle = false;

    public Button calendar; 
    void Start()
    {
        filterListPanel.SetActive(false);
        roomButtons = new List<Transform>();
        InitializeRoomButtons();
        filterButton.onClick.AddListener(() => enableFilter());
        closeFilterList.onClick.AddListener(() => closeFilter());
        filterFavorite.onClick.AddListener(() => Favorite());
    
        schedulePanel.SetActive(false);
        scheduleButton.onClick.AddListener(() => enableSchedule());
        closeSchedulePanel.onClick.AddListener(() => closeSchedule());
        addClass.onClick.AddListener(() => AddClass());

        daysOfWeekPanel.SetActive(false);
        closeDaysOfWeekPanel.onClick.AddListener(() => closeDOW());
        Mon.onClick.AddListener(() => Monday());
        Tue.onClick.AddListener(() => Tuesday());
        Wed.onClick.AddListener(() => Wednesday());
        Thu.onClick.AddListener(() => Thursday());
        Fri.onClick.AddListener(() => Friday());
        submitButton.onClick.AddListener(() => submit());
        calendar.onClick.AddListener(() => Calendar());
    }

    private void Calendar()
    {
        
        schedulePanel.SetActive(true);
        
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

    void AddClass() {
        schedulePanel.SetActive(false);
        daysOfWeekPanel.SetActive(true);
    }
    void enableFilter(){
        schedulePanel.SetActive(false);
        filterListPanel.SetActive(true);
    }
    void closeFilter(){
        filterListPanel.SetActive(false);
    }

     private void enableSchedule()
    {
        schedulePanel.SetActive(true);
        filterListPanel.SetActive(false);
    }

     private void closeSchedule()
    {
        schedulePanel.SetActive(false);
    }

    private void enableDOW()
    {
        daysOfWeekPanel.SetActive(true);
        filterListPanel.SetActive(false);
    }

     private void closeDOW()
    {
        daysOfWeekPanel.SetActive(false);
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

    private void Monday()
    {
        if (mondayToggle == false)
        {
            mondayToggle = true;
            Mon.GetComponent<Image>().color = Color.green;
        }
        else
        {
            mondayToggle = false;
            Mon.GetComponent<Image>().color = Color.white;
        }
    }

    private void Tuesday()
    {
        if (tuesdayToggle == false)
        {
            tuesdayToggle = true;
            Tue.GetComponent<Image>().color = Color.green;
        }
        else
        {
            tuesdayToggle = false;
            Tue.GetComponent<Image>().color = Color.white;
        }
    }

    private void Wednesday()
    {
        if (wednesdayToggle == false)
        {
            wednesdayToggle = true;
            Wed.GetComponent<Image>().color = Color.green;
        }
        else
        {
            wednesdayToggle = false;
            Wed.GetComponent<Image>().color = Color.white;
        }
    }

    private void Thursday()
    {
        if (thursdayToggle == false)
        {
            thursdayToggle = true;
            Thu.GetComponent<Image>().color = Color.green;
        }
        else
        {
            thursdayToggle = false;
            Thu.GetComponent<Image>().color = Color.white;
        }
    }

    private void Friday()
    {
        if (fridayToggle == false)
        {
            fridayToggle = true;
            Fri.GetComponent<Image>().color = Color.green;
        }
        else
        {
            fridayToggle = false;
            Fri.GetComponent<Image>().color = Color.white;
        }
    }

    void submit() {
        daysOfWeekPanel.SetActive(false);
        List<Days> days = new List<Days>();
        ScheduleManager sm = new ScheduleManager();
        if (mondayToggle) {
            days.Add(Days.Monday);
        }
        if (tuesdayToggle) {
            days.Add(Days.Tuesday);
        }
        if (wednesdayToggle) {
            days.Add(Days.Wednesday);
        }
        if (thursdayToggle) {
            days.Add(Days.Thursday);
        }
        if (fridayToggle) {
            days.Add(Days.Friday);
        }

        sm.AddSchedule(className.text, days, startTime.text, endTime.text);
    }

}
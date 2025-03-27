using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsScript : MonoBehaviour
{   
    public Button toggleLanguage;
    public bool isFrench = false;
    
    public Button muteSounds;
    public bool isMute = false;
    //settings menu text
    public TMP_Text settingsText;
    public TMP_Text staircaseNavText;
    public TMP_Text muteSoundsText;
    public TMP_Text frenchToggleText;

    //search panel
    public TMP_Text searchPanelInfoText;
    public TMP_Text placeholderSearch;
    public TMP_Text filterButtonText;
    public TMP_Text sortButtonText;

    //greenUI
    public TMP_Text navigatingText;

    //scanUI
    public TMP_Text scanUIText;

    //arrived UI
    public TMP_Text arrivedUIText;

    //Scan notif
    public TMP_Text scanNotifText;



    void Start()
    {
        toggleLanguage.onClick.AddListener(ToggleFrench);
        muteSounds.onClick.AddListener(ToggleMute);
    }

    void ToggleFrench()
    {
        isFrench = !isFrench;

       if(isFrench){
            //settings menu
            settingsText.text = "Appuyez Pour Activer un Paramètre";
            staircaseNavText.text = "Navigation d'Escalier";
            muteSoundsText.text = "Couper le Son";
            frenchToggleText.text = "Langue: Français";
            //search panel
            searchPanelInfoText.text = "Touchez un Location pour Naviguer";
            placeholderSearch.text = "Rechercher";
            filterButtonText.text = "Filtrer";
            sortButtonText.text = "Trier";
            //greenUI
            navigatingText.text = "Accéder à la salle";
            //scanUI
            scanUIText.text = "SVP scanner l'emplacement de départ";
            //arrived text
            arrivedUIText.text = "Arrivée à destination. (Appuyez pour fermer)";
            //scan notif
            scanNotifText.text = "Pièce scannée";
        }
        else{
            //settings menu
            settingsText.text = "Tap to Toggle a Setting";
            staircaseNavText.text = "Toggle Staircase Navigaiton";
            muteSoundsText.text = "Mute Audio";
            frenchToggleText.text = "Language: English";
            //search panel
            searchPanelInfoText.text = "Tap a Location to Start Navigation";
            placeholderSearch.text = "Search";
            filterButtonText.text = "Filter";
            sortButtonText.text = "Sort";
            //greenUI
            navigatingText.text = "Navigating to Room";
            //scanUI
            scanUIText.text = "Please Scan Starting Location";
            //arrived text
            arrivedUIText.text = "Arrived at Location. (Tap to Close)";
            //scan notif
            scanNotifText.text = "Successfully Scanned Room";
            
        }
    }

    void ToggleMute()
    {
        isMute = !isMute;
    }
}
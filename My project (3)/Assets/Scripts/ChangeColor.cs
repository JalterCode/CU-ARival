using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonImageColor : MonoBehaviour
{
    public Button button;
    private bool isGreen = false;

    private Color defaultColor;
    private Color greenColor = Color.green;

    void Start()
    {
        // Get the default color from the Image component attached to the Button
        defaultColor = button.GetComponent<Image>().color;

        // Add a listener for the button click event
        button.onClick.AddListener(ToggleColor);
    }

    private void ToggleColor()
    {
        // Toggle the color between green and the default color
        var image = button.GetComponent<Image>();
        image.color = isGreen ? defaultColor : greenColor;

        isGreen = !isGreen;
    }
}
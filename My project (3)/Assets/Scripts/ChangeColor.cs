using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeImageColorOnClick : MonoBehaviour, IPointerClickHandler
{
    public Color targetColor = Color.green; // The color to change to
    private Image imageComponent; // Reference to the Image component
    private Color originalColor; // Store the original color

    void Start()
    {
        // Get the Image component attached to this GameObject
        imageComponent = GetComponent<Image>();

        // Store the original color for later restoration
        if (imageComponent != null)
        {
            originalColor = imageComponent.color;
        }
        else
        {
            Debug.LogError("Image component not found on this GameObject!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (imageComponent != null)
        {
            imageComponent.color = targetColor;
        }
    }
}
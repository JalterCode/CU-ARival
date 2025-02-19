using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils; // For XR Origin
using TMPro;
using System.Collections;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Transform startingPoint; // Reference to AR Camera
    private Transform endPoint;
    public LineRenderer lineRenderer;
    float elapsed;
    public NavMeshPath path;
    [SerializeField] private Animator animator;

    [SerializeField] private Animator scanUI;

    [SerializeField] private ARTrackedImageManager trackedImageManager;
    private XROrigin xrOrigin; // Reference to XR Origin
    private bool isScanningEnabled = true;


    // distance management
    public Button button; // Assign your button in the inspector
    public TextMeshProUGUI textMeshPro; 
    private bool isUpdating = false;
    private Text distanceText;
    private WaitForSeconds waitTime;

    void Start()
    {
        scanUI.SetBool("DropUIDown", true);
        
        path = new NavMeshPath();
        elapsed = 0.0f;
        instance = this;

        // Get reference to XR Origin
        xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not found!");
        }

        GameObject roomObject = GameObject.Find(findRoomScript.GetDestination());
        if (roomObject != null)
        {
            endPoint = roomObject.transform;
        }
        //distance check
        distanceText = GetComponent<Text>();
        waitTime = new WaitForSeconds(0.5f); 
        button.onClick.AddListener(StartUpdatingDistance);
    }

    private void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;

    private void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;

    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (!isScanningEnabled) return;

        Debug.Log("Image detected");

        foreach (var newImage in eventArgs.added)
        {
            scanUI.SetBool("DropUIDown", false);
            scanUI.SetBool("Scanned",true);
            string imageName = newImage.referenceImage.name;
            GameObject targetObject = GameObject.Find(imageName);

            if (targetObject != null)
            {
                // Calculate the position difference between the detected image and the target object
                Vector3 offsetPosition = targetObject.transform.position - newImage.transform.position;
                Quaternion offsetRotation = targetObject.transform.rotation * Quaternion.Inverse(newImage.transform.rotation);

                // Manually update the XR Origin's position and rotation
                xrOrigin.transform.position += offsetPosition;
                xrOrigin.transform.rotation = offsetRotation;

                animator.SetBool("ButtonPress",true);

                Debug.Log($"XR Origin manually adjusted to align with {imageName} location.");
            }
            else
            {
                Debug.LogError($"Could not find object named: {imageName}");
            }
        
            trackedImageManager.enabled = false;
            isScanningEnabled = false;

        }
    }

    void Update()
    {
        float remainingLen = 0;
        if (path.status == NavMeshPathStatus.PathComplete)
        {   
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                remainingLen += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                
            }
        }

        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            if (endPoint != null)
            {
                NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }
        }
    }

    public void UpdateNavigationTarget(string roomName)
    {
        GameObject roomObject = GameObject.Find(roomName);

        if (roomObject != null)
        {
            endPoint = roomObject.transform;
            Debug.Log("Navigation room updated: " + roomName);
        }
        else
        {
            Debug.LogError("Room not found: " + roomName);
        }
    }
    //DISTANCE CHECK

    void StartUpdatingDistance()
    {
        textMeshPro.text = "PLEASE";
        if (!isUpdating)
        {
            isUpdating = true;
            StartCoroutine(UpdateDistance());
        }
    }
   IEnumerator UpdateDistance(){

        while (true) 
        {
            if (endPoint != null)
            {
                NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    float pathLength = 0;
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                    }

                    textMeshPro.text =  pathLength.ToString("F1") + " m";
                }
                else
                {
                    textMeshPro.text = "No path found.";
                }
            }
            else
            {
                textMeshPro.text = "No destination set.";
            }

            yield return waitTime; 
        }
    }

    public void EnableScanning(){
        trackedImageManager.enabled = true;
        isScanningEnabled = true;
        scanUI.SetBool("Scanned", false);
        animator.SetBool("ButtonPress",false);
        scanUI.SetBool("CamButtonPressed",true);
    }

}
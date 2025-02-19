using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
<<<<<<< Updated upstream
=======
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils; // For XR Origin
using TMPro;
using System.Collections;
>>>>>>> Stashed changes

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Transform startingPoint;

    private Transform endPoint;

    public LineRenderer lineRenderer;

    float elapsed; 
    public NavMeshPath path;
<<<<<<< Updated upstream
    // Start is called before the first frame update
=======
    [SerializeField] private Animator animator;

    [SerializeField] private Animator scanUI;

    [SerializeField] private ARTrackedImageManager trackedImageManager;
    private XROrigin xrOrigin; // Reference to XR Origin
    private bool isScanningEnabled = true;


    // distance management
    public TextMeshProUGUI textMeshPro; 
    private bool isUpdating = false;
    private Text distanceText;
    private WaitForSeconds waitTime;

    public Button arrived;

>>>>>>> Stashed changes
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
        instance = this;
        GameObject roomObject = GameObject.Find(findRoomScript.GetDestination());
<<<<<<< Updated upstream
        endPoint = roomObject.transform;
=======
        if (roomObject != null)
        {
            endPoint = roomObject.transform;
        }
        //distance check
        distanceText = GetComponent<Text>();
        waitTime = new WaitForSeconds(0.5f); 
        StartUpdatingDistance();
    }

    private void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;

    private void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;

    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
{
    if (!isScanningEnabled) return;

    Debug.Log("Image detected");

    foreach (var newImage in eventArgs.added)
    {
       
        scanUI.SetBool("Scanned", true);
        string imageName = newImage.referenceImage.name;
        GameObject targetObject = GameObject.Find(imageName);

        if (targetObject != null)
        {
            xrOrigin.transform.position = Vector3.zero;
            // Calculate the offset between the detected image and the target object
            Vector3 offset = newImage.transform.position - targetObject.transform.position ;

            // Shift the XR Origin to align the AR space with the real-world plaque
            xrOrigin.transform.position += offset;

            // Optionally, adjust rotation
            Quaternion rotationOffset = Quaternion.Inverse(newImage.transform.rotation) * targetObject.transform.rotation;
            xrOrigin.transform.rotation *= rotationOffset;

            animator.SetBool("ButtonPress", true);

            Debug.Log($"XR Origin adjusted to align with {imageName} location.");
        }
        else
        {
            Debug.LogError($"Could not find object named: {imageName}");
        }

        trackedImageManager.enabled = false;
        isScanningEnabled = false;
>>>>>>> Stashed changes
    }
}

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed > 1.0f){
            elapsed -= 1.0f;
<<<<<<< Updated upstream
            NavMesh.CalculatePath(startingPoint.position, endPoint.position,NavMesh.AllAreas, path);
=======
            if (endPoint != null)
            {
                NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
                Debug.Log($"Path status: {path.status}, Corners count: {path.corners.Length}");
            }
>>>>>>> Stashed changes
        }

        lineRenderer.positionCount=path.corners.Length;
        lineRenderer.SetPositions(path.corners);
    }


<<<<<<< Updated upstream
    public void NavigateTo(GameObject go){}

=======

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

                    //arrived
                    if (pathLength<=3 && pathLength!=0){
                        arrived.gameObject.SetActive(true);
                    }
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
>>>>>>> Stashed changes

}

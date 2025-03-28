using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils; // For XR Origin
using TMPro;
using System.Collections;
using System.Threading;
using System;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Transform startingPoint; // Reference to AR Camera
    private Transform endPoint;
    public LineRenderer lineRenderer;
    float elapsed;
    public NavMeshPath path;

    public findRoomScript roomScript;

    [SerializeField] private Animator notificationAnimator;

    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Animator animator;

    [SerializeField] private Animator scanUI;

    [SerializeField] private ARTrackedImageManager trackedImageManager;
    private XROrigin xrOrigin; // Reference to XR Origin
    private bool isScanningEnabled = true;


    // distance management
    public TextMeshProUGUI textMeshPro; 
    private bool isUpdating = false;
    private WaitForSeconds waitTime;

    public Button arrived;
    private string imageName;
    private int count = 0;

    [SerializeField] private TextMeshProUGUI scanText;

    [SerializeField] private Animator greenUI;

    public TMP_Text greenUIText;

    private bool isMultiFloorNavigating = false;

    private int rescanCount = 0;

    //greenUI arrows
    public Image greenUIImage;
    public Sprite straightArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;

    
    public TMP_Text nextTurnTMP;

    public AudioClip leftTurnSound;
    public AudioClip rightTurnSound;
    public AudioClip straightSound;

    public AudioSource turnSound;
    private int audioDelay = 0;


    void Start()
    {
        scanUI.SetBool("NotScanned",true);
        arrived.gameObject.SetActive(false);
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
                

        if (lineRenderer != null)
        {
            // Create a uniform width curve
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0, 0.3f);
            curve.AddKey(1, 0.3f);

            lineRenderer.widthCurve = curve;
        }
        //distance check
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
        imageName = newImage.referenceImage.name;
        GameObject targetObject = GameObject.Find(imageName);

        if (targetObject != null)
        {
            xrOrigin.MoveCameraToWorldLocation(targetObject.transform.position);
            xrOrigin.MatchOriginUpCameraForward(targetObject.transform.up, targetObject.transform.forward);

            animator.SetBool("ButtonPress", true);
            Debug.Log($"XR Origin adjusted to align with {imageName} location.");

            StartCoroutine(GenerateButtonsDelayed(targetObject));
        }
        else
        {
            Debug.LogError($"Could not find object named: {imageName}");
        }

        trackedImageManager.enabled = false;
        isScanningEnabled = false;
        scanUI.SetBool("NotScanned",false);

        notificationAnimator.SetTrigger("Scanned");
        notificationText.text = $"Successfully scanned room {imageName}";

        if(isMultiFloorNavigating){
            greenUI.SetTrigger("Navigating");
        }

    }
}

private IEnumerator GenerateButtonsDelayed(GameObject startingLocation)
{
    yield return null;

    roomScript.GenerateButtons(startingLocation);


}

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            if (endPoint != null)
            {
                NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
                Debug.Log($"Path status: {path.status}, Corners count: {path.corners.Length}");
            }
        }
    }

    public void UpdateNavigationTarget(string roomName)
    {
        GameObject roomObject = GameObject.Find(roomName);


        if (roomObject != null)
        {
            endPoint = roomObject.transform;
            count = 0;
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
                    if (pathLength<=1 && pathLength!=0 && count == 0){
                        arrived.gameObject.SetActive(true);
                        count++;
                        imageName = "";
                        greenUI.SetTrigger("Not Navigating");
                        if(isMultiFloorNavigating){
                            isMultiFloorNavigating = false;
                        }
                        //penis
                        if(endPoint.name.StartsWith("stairs") || endPoint.name.StartsWith("elevator")) {
                            scanText.text = $"Please scan on floor {findRoomScript.RealDestination()[..1]}";
                            endPoint = GameObject.Find(findRoomScript.RealDestination()).transform;
                            isMultiFloorNavigating = true;
                            EnableScanning();
                            greenUIText.text = $"Navigating to room {findRoomScript.RealDestination()}";
                            Debug.Log($"WWWWWWWWWWWW {findRoomScript.RealDestination()}");
                        }
                    } 
                    int turnDirection = 0;
                    float turnDistance;
                    (turnDirection, turnDistance) = turn();
                    //nextTurnTMP.text = turnDistance.ToString("F1") + " m";
                    if (turnDirection == 1)
                    {
                        greenUIImage.sprite = rightArrow;
                        
                        
                        if (turnDistance<3 && audioDelay>=10){
                            turnSound.PlayOneShot(rightTurnSound);
                            audioDelay = 0;
                        }
                    }
                    else if (turnDirection == 2)
                    {
                        greenUIImage.sprite = leftArrow;
                        if (turnDistance<3 &&audioDelay>=10){
                            
                            turnSound.PlayOneShot(leftTurnSound);
                            audioDelay = 0;
                        }
                    }
                    else
                    {
                        greenUIImage.sprite = straightArrow;
                        if (audioDelay>=10){
                            
                            turnSound.PlayOneShot(straightSound);
                            audioDelay = 0;
                        }
                    }
                    audioDelay++;
                
                }
                else
                {
                    textMeshPro.text = "No path found.";
                    rescanCount++;
                    if (rescanCount >=15){
                        rescanCount = 0;
                        scanText.text = $"Calibration lose, please rescan";
                        EnableScanning();
                    }
                }
            }
            else
            {
                textMeshPro.text = "No destination set.";
            }

            yield return waitTime; 
        }
    }
    private float TurnCheck(Vector3 previousCorner, Vector3 currentCorner, Vector3 nextCorner)
    {
        Vector3 previousDirection = (currentCorner - previousCorner).normalized;
        Vector3 nextDirection = (nextCorner - currentCorner).normalized;

        float crossProductY = Vector3.Cross(previousDirection, nextDirection).y;

        return crossProductY; // Positive y means left turn
    }

public (int, float) turn()
    {
        if (path == null || path.corners.Length < 3)
        {
            return (0, 0f); // Straight, no distance
        }

        for (int i = 1; i < path.corners.Length - 1; i++)
        {
            float turnValue = TurnCheck(path.corners[i - 1], path.corners[i], path.corners[i + 1]);
            if (turnValue > 0)
            {
                // Right turn
                float distanceToTurn = Vector3.Distance(startingPoint.position, path.corners[i]);
                return (1, distanceToTurn);
            }
            else if (turnValue < 0)
            {
                // Left turn
                float distanceToTurn = Vector3.Distance(startingPoint.position, path.corners[i]);
                return (2, distanceToTurn);
            }
        }

        // Straight 
        return (0, Vector3.Distance(startingPoint.position, path.corners[path.corners.Length - 1])); // Straight, distance to end
    }






public void EnableScanning(){
    trackedImageManager.enabled = true;
    isScanningEnabled = true;
    scanUI.SetBool("Scanned", false);
    scanUI.SetBool("NotScanned",true);

    animator.SetBool("ButtonPress",false);
    scanUI.SetBool("CamButtonPressed",true);
    Debug.Log("Rescan Button Pressed");
}

public string GetImageName() {
    return imageName;
}



    public void ToggleEndpoint()
{
    if (endPoint == null) return;



    string currentEndpointName = endPoint.gameObject.name;
    string floorNumber = "";

    if (currentEndpointName.StartsWith("elevator"))
    {
        floorNumber = currentEndpointName.Substring(8);
        GameObject stairsObject = GameObject.Find("stairs" + floorNumber);
        if (stairsObject != null)
        {
            endPoint = stairsObject.transform;
            if (roomScript != null)
            {
                roomScript.SetlocationText("stairs" + floorNumber);
                Debug.LogError("SetlocationText set to stairs");
            }
            else
            {
                Debug.LogError("roomScript is not assigned in NavigationManager!");
            }
            Debug.Log($"Switched to Stairs{floorNumber}");
        }
    }
    else if (currentEndpointName.StartsWith("stairs"))
    {
        floorNumber = currentEndpointName.Substring(6);
        GameObject elevatorObject = GameObject.Find("elevator" + floorNumber);
        if (elevatorObject != null)
        {
            endPoint = elevatorObject.transform;
            if (roomScript != null)
            {
                roomScript.SetlocationText("elevator" + floorNumber); 
                Debug.LogError("SetlocationText set to Elevator");
            }
            else
            {
                Debug.LogError("roomScript is not assigned in NavigationManager!");
            }
            Debug.Log($"Switched to Elevator{floorNumber}");
        }
    }

    greenUIText.text = $"Navigating to room {endPoint.name}";

    }
}
 
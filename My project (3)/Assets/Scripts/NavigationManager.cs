using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils; // For XR Origin
using TMPro;
using System.Collections;
using System.Threading; // Note: System.Threading might not be needed based on this code alone.
using System;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Transform startingPoint; // Reference to AR Camera (Ensure this is assigned correctly!)
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

    void Start()
    {
        scanUI.SetBool("NotScanned", true);
        arrived.gameObject.SetActive(false);
        path = new NavMeshPath();
        elapsed = 0.0f;
        instance = this;

        // Get reference to XR Origin
        xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not found in the scene!");
        }
        // *** DEBUG: Log startingPoint reference ***
        if (startingPoint == null)
        {
            Debug.LogError("!!!!!! STARTING POINT IS NOT ASSIGNED IN THE INSPECTOR !!!!!!");
        }
        else
        {
            Debug.Log($"NavigationManager using startingPoint: {startingPoint.name} with tag {startingPoint.tag}");
        }


        // Initial endpoint check - might be null if findRoomScript hasn't set a destination yet
        GameObject initialRoomObject = GameObject.Find(findRoomScript.GetDestination());
        if (initialRoomObject != null)
        {
            endPoint = initialRoomObject.transform;
            Debug.Log($"Initial endpoint set to {endPoint.name} in Start()");
        }


        if (lineRenderer != null)
        {
            // Create a uniform width curve
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0, 0.3f);
            curve.AddKey(1, 0.3f);
            lineRenderer.widthCurve = curve;
        }
        else
        {
            Debug.LogError("LineRenderer is not assigned in the Inspector!");
        }

        //distance check
        waitTime = new WaitForSeconds(0.5f);
        StartUpdatingDistance();
    }

    private void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnChanged;
        }
        else
        {
            Debug.LogError("ARTrackedImageManager is not assigned in the Inspector!");
        }
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnChanged;
        }
    }

    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (!isScanningEnabled) return;

        Debug.Log("AR Image Event triggered."); // Changed log slightly

        foreach (var newImage in eventArgs.added)
        {
            Debug.Log($"Image detected: {newImage.referenceImage.name}"); // Log detected image name
            scanUI.SetBool("Scanned", true);
            imageName = newImage.referenceImage.name;
            GameObject targetObject = GameObject.Find(imageName);

            if (targetObject != null)
            {
                if (xrOrigin == null)
                {
                     Debug.LogError("Cannot move XR Origin because it was not found!");
                     return; // Stop processing if xrOrigin is missing
                }
                if (startingPoint == null)
                {
                     Debug.LogError("Cannot log startingPoint position because it's not assigned!");
                     // Continue, but logs below will fail or be misleading
                }

                // *** Store position BEFORE move for comparison (optional) ***
                // Vector3 posBeforeMove = startingPoint != null ? startingPoint.position : Vector3.zero;
                // Debug.Log($"OnChanged: startingPoint position BEFORE move: {posBeforeMove}");

                xrOrigin.MoveCameraToWorldLocation(targetObject.transform.position);
                xrOrigin.MatchOriginUpCameraForward(targetObject.transform.up, targetObject.transform.forward);

                Debug.Log($"XR Origin adjusted to align with {imageName} location.");

                // *** ADDED DEBUG LOGS FOR POSITIONS AFTER MOVE ***
                Debug.Log($"OnChanged: targetObject '{targetObject.name}' ACTUAL position: {targetObject.transform.position}");
                // Check if startingPoint is assigned before trying to access it
                if (startingPoint != null) {
                    Debug.Log($"OnChanged: startingPoint '{startingPoint.name}' position immediately AFTER camera move: {startingPoint.position}");
                } else {
                     Debug.LogError("OnChanged: startingPoint is NULL, cannot log its position after move.");
                }
                // Log the XROrigin root position for comparison - already checked xrOrigin != null above
                Debug.Log($"OnChanged: xrOrigin root '{xrOrigin.name}' position immediately AFTER camera move: {xrOrigin.transform.position}");
                // *** END ADDED DEBUG LOGS ***


                if (animator != null) animator.SetBool("ButtonPress", true); // Check for null
                else Debug.LogWarning("Animator 'animator' not assigned.");


                // Use startingPoint's parent if startingPoint is camera, else startingPoint itself might be okay
                // Pass targetObject for GenerateButtons, as it represents the physical location scanned
                StartCoroutine(GenerateButtonsDelayed(targetObject));
            }
            else
            {
                Debug.LogError($"Could not find target GameObject named: {imageName}");
            }

            // Disable further scanning only if successful processing happened
            if (targetObject != null)
            {
                 trackedImageManager.enabled = false;
                 isScanningEnabled = false;
                 scanUI.SetBool("NotScanned", false);

                 if (notificationAnimator != null) notificationAnimator.SetTrigger("Scanned"); // Check for null
                 else Debug.LogWarning("Animator 'notificationAnimator' not assigned.");

                 if (notificationText != null) notificationText.text = $"Successfully scanned room {imageName}"; // Check for null
                 else Debug.LogWarning("TextMeshProUGUI 'notificationText' not assigned.");


                 if (isMultiFloorNavigating)
                 {
                     if (greenUI != null) greenUI.SetTrigger("Navigating"); // Check for null
                     else Debug.LogWarning("Animator 'greenUI' not assigned.");
                 }
                 Debug.Log($"Scanning disabled. Ready for navigation target selection."); // Changed log
            }
        }
         // Optional: Log updated/removed images if needed for debugging tracking stability
         // foreach (var updatedImage in eventArgs.updated) { ... }
         // foreach (var removedImage in eventArgs.removed) { ... }
    }

    private IEnumerator GenerateButtonsDelayed(GameObject locationForButtons) // Renamed for clarity
    {
        yield return null; // Wait one frame to ensure transforms are updated

        if (roomScript != null)
        {
             // Pass the GameObject representing the scanned location
             roomScript.GenerateButtons(locationForButtons);
             Debug.Log($"GenerateButtons called on roomScript using location: {locationForButtons.name}");
        }
        else
        {
             Debug.LogError("roomScript reference is not assigned in NavigationManager!");
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f) // Update path every second
        {
            elapsed -= 1.0f;
            if (endPoint != null && startingPoint != null) // Added null check for startingPoint
            {
                // *** ADDED PRE-CALC CHECK LOG ***
                Debug.Log($"Update(): PRE-CALC CHECK - Start: {startingPoint.position}, End: {endPoint.position} (End object: {endPoint.name})");
                // *** END ADDED PRE-CALC CHECK LOG ***

                NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);

                // Status Checking (already present from user)
                if (path.status == NavMeshPathStatus.PathInvalid)
                {
                    Debug.LogError($"Update(): Path calculation failed - PathInvalid. StartPos: {startingPoint.position}, EndPos: {endPoint.position}. Are both points on the NavMesh?");
                }
                else if (path.status == NavMeshPathStatus.PathPartial)
                {
                    Debug.LogWarning($"Update(): Path calculation resulted in a partial path. StartPos: {startingPoint.position}, EndPos: {endPoint.position}. Cannot fully reach destination.");
                }

                if (lineRenderer != null) // Check lineRenderer null
                {
                     lineRenderer.positionCount = path.corners.Length;
                     lineRenderer.SetPositions(path.corners);
                }

            }
            else
            {
                // Path calculation not possible if start or end is missing
                if(lineRenderer != null) lineRenderer.positionCount = 0; // Clear the line

                // Optional log for why calculation isn't happening
                // if(startingPoint == null) Debug.LogWarning("Update: Cannot calculate path - startingPoint is null.");
                // if(endPoint == null) Debug.Log("Update: Cannot calculate path - endPoint is null.");
            }
        }
    }

    public void UpdateNavigationTarget(string roomName)
    {
        Debug.Log($"Attempting to update navigation target to: {roomName}");
        GameObject roomObject = GameObject.Find(roomName);

        if (roomObject != null)
        {
            // Logging World/Local/Parent (already present from user)
            Debug.Log($"UpdateNavigationTarget: Found '{roomObject.name}'. " +
                      $"WORLD Pos: {roomObject.transform.position}. " +
                      $"LOCAL Pos: {roomObject.transform.localPosition}. " +
                      $"Parent: {(roomObject.transform.parent != null ? roomObject.transform.parent.name : "None")}");

            endPoint = roomObject.transform;
            count = 0; // Reset arrival count
            Debug.Log($"UpdateNavigationTarget: endPoint variable is now '{endPoint.name}' at position {endPoint.position}"); // Added confirmation log
        }
        else
        {
            endPoint = null;
            Debug.LogError($"UpdateNavigationTarget FAILED: Room GameObject '{roomName}' not found!");
        }
    }

    //DISTANCE CHECK
    void StartUpdatingDistance()
    {
        // Avoid starting if already updating
        if (!isUpdating && this.gameObject.activeInHierarchy) // Check if GO is active
        {
             if (textMeshPro != null) textMeshPro.text = "Calculating..."; // Initial text
             else Debug.LogWarning("TextMeshPro 'textMeshPro' not assigned.");

             isUpdating = true;
             StartCoroutine(UpdateDistance());
        }
    }

    IEnumerator UpdateDistance()
    {
        while (true) // Careful: This runs indefinitely until the object is disabled/destroyed
        {
            if (endPoint != null && startingPoint != null) // Added null check for startingPoint
            {
                // *** ADDED PRE-CALC CHECK LOG ***
                // Debug.Log($"UpdateDistance(): PRE-CALC CHECK - Start: {startingPoint.position}, End: {endPoint.position} (End object: {endPoint.name})"); // Can be noisy, uncomment if needed
                // *** END ADDED PRE-CALC CHECK LOG ***

                // Re-calculate path specifically for distance/arrival checks
                // Note: This recalculates path potentially twice per second (here and in Update) - consider optimizing if performance is an issue
                NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    float pathLength = 0;
                    if (path.corners.Length > 1) // Need at least two points to calculate length
                    {
                         for (int i = 0; i < path.corners.Length - 1; i++)
                         {
                             pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                         }
                    }

                    if (textMeshPro != null) textMeshPro.text = pathLength.ToString("F1") + " m";

                    //arrived logic
                    if (pathLength <= 1.5f && pathLength > 0 && count == 0) // Increased threshold slightly, ensure not exactly 0
                    {
                        Debug.Log($"Arrival detected! Path Length: {pathLength}"); // Log arrival
                        if (arrived != null) arrived.gameObject.SetActive(true);
                        else Debug.LogWarning("Button 'arrived' not assigned.");

                        count++; // Increment count to prevent repeated triggering
                        imageName = ""; // Clear scanned image name

                        if (greenUI != null) greenUI.SetTrigger("Not Navigating");
                        else Debug.LogWarning("Animator 'greenUI' not assigned.");

                        isMultiFloorNavigating = false; // Reset multi-floor flag on arrival

                        // Multi-floor transition logic
                        if (endPoint.name.StartsWith("stairs") || endPoint.name.StartsWith("elevator"))
                        {
                             string realDestName = findRoomScript.RealDestination();
                             if (!string.IsNullOrEmpty(realDestName))
                             {
                                 if(scanText != null) scanText.text = $"Arrived at {endPoint.name}. Please scan QR code on floor {realDestName[..1]}";
                                 else Debug.LogWarning("TextMeshProUGUI 'scanText' not assigned.");

                                 GameObject realDestObject = GameObject.Find(realDestName);
                                 if(realDestObject != null)
                                 {
                                     endPoint = realDestObject.transform; // Set new target
                                     isMultiFloorNavigating = true;
                                     EnableScanning(); // Re-enable scanning for next floor
                                     if (greenUIText != null) greenUIText.text = $"Navigating to room {realDestName}";
                                     else Debug.LogWarning("TextMeshProUGUI 'greenUIText' not assigned.");
                                     Debug.Log($"Multi-floor transition: Now targeting {realDestName}. Scanning enabled.");
                                     count = 0; // Reset arrival count for the next leg
                                 }
                                 else
                                 {
                                     Debug.LogError($"Cannot find real destination object: {realDestName}");
                                     // Optionally handle this error, e.g., stop navigation
                                     endPoint = null; // Stop navigation if final dest not found
                                 }
                             }
                             else {
                                 Debug.LogWarning("RealDestination from findRoomScript is null or empty.");
                             }
                        }
                        else // Arrived at final non-transition destination
                        {
                             Debug.Log("Navigation Complete to final destination.");
                             // Optionally clear endpoint or stop updates
                             // endPoint = null;
                        }
                    } // End arrival check

                    // Update Turn Arrow logic
                    int turnDirection = turn(); // Calculate turn direction
                     if(greenUIImage != null) // Check null
                     {
                         if (turnDirection == 1) greenUIImage.sprite = rightArrow;
                         else if (turnDirection == 2) greenUIImage.sprite = leftArrow;
                         else greenUIImage.sprite = straightArrow;
                     } else { Debug.LogWarning("Image 'greenUIImage' not assigned."); }

                } // End PathComplete check
                else // Path not complete
                {
                    if (textMeshPro != null) textMeshPro.text = "No path found"; // Simpler text
                    // Log warning about path status
                    // Debug.LogWarning($"UpdateDistance(): Path not complete. Status: {path.status}. Start: {startingPoint.position}, End: {(endPoint != null ? endPoint.position.ToString() : "NULL")}"); // Can be noisy

                    // Rescan logic (consider if this is the desired behavior on path failure)
                    rescanCount++;
                    if (rescanCount >= 15) // Threshold for requesting rescan
                    {
                        Debug.LogWarning("Path consistently not found, requesting rescan due to potential calibration loss.");
                        rescanCount = 0;
                        if (scanText != null) scanText.text = $"Calibration lost? Please rescan QR code.";
                        else Debug.LogWarning("TextMeshProUGUI 'scanText' not assigned.");
                        EnableScanning();
                        // Consider stopping navigation attempts until rescan
                        // endPoint = null;
                        // lineRenderer.positionCount = 0;
                    }
                }
            } // End null check for endpoint/startpoint
            else
            {
                if (textMeshPro != null) textMeshPro.text = "No destination"; // Update text if no target
            }

            yield return waitTime; // Wait before next iteration
        }
    } // End UpdateDistance Coroutine


    // Turn detection logic (seems okay, but check performance if path is complex)
    private float TurnCheck(Vector3 previousCorner, Vector3 currentCorner, Vector3 nextCorner)
    {
        // Use only XZ plane for turn detection to ignore vertical changes
        Vector3 prevDir = currentCorner - previousCorner;
        Vector3 nextDir = nextCorner - currentCorner;
        prevDir.y = 0;
        nextDir.y = 0;

        // Check for zero vectors (if points are identical)
        if (prevDir.sqrMagnitude < 0.001f || nextDir.sqrMagnitude < 0.001f) return 0;

        prevDir.Normalize();
        nextDir.Normalize();

        // Signed angle calculation is more robust than just cross product Y
        float angle = Vector3.SignedAngle(prevDir, nextDir, Vector3.up);
        return angle;

        // Original Cross product method (can be sensitive)
        // float crossProductY = Vector3.Cross(prevDir, nextDir).y;
        // return crossProductY;
    }

    public int turn()
    {
        if (path == null || path.corners.Length < 2) // Need at least 2 points for a direction
        {
            return 0; // Straight/No Turn
        }

        // Use the direction from the current position (startingPoint) to the *next* corner
        Vector3 currentPos = startingPoint.position;
        Vector3 nextCorner = path.corners[1]; // The immediate next corner to head towards

        Vector3 currentForward = startingPoint.forward; // Camera's forward direction
        Vector3 directionToNextCorner = nextCorner - currentPos;

        // Ignore Y component for turn calculation
        currentForward.y = 0;
        directionToNextCorner.y = 0;

        // Check for very short distance (already at the corner)
        if (directionToNextCorner.sqrMagnitude < 0.1f)
        {
             // If close to the next corner, check direction to the corner after that (if exists)
             if(path.corners.Length >= 3)
             {
                 Vector3 cornerAfterNext = path.corners[2];
                 directionToNextCorner = cornerAfterNext - nextCorner;
                 directionToNextCorner.y = 0;
             } else {
                 return 0; // At the last corner, go straight
             }
        }

        if (currentForward.sqrMagnitude < 0.001f || directionToNextCorner.sqrMagnitude < 0.001f) return 0; // Avoid issues with zero vectors

        currentForward.Normalize();
        directionToNextCorner.Normalize();

        float angle = Vector3.SignedAngle(currentForward, directionToNextCorner, Vector3.up);

        // Define thresholds for left/right turns
        float turnThreshold = 15.0f; // Degrees

        if (angle > turnThreshold)
        {
            // Debug.Log($"Turn Right detected. Angle: {angle}");
            return 1; // Right
        }
        else if (angle < -turnThreshold)
        {
            // Debug.Log($"Turn Left detected. Angle: {angle}");
            return 2; // Left
        }
        else
        {
            // Debug.Log($"Going Straight. Angle: {angle}");
            return 0; // Straight
        }


        /* // Original corner-based turn logic (less intuitive for user direction)
        if (path.corners.Length < 3) return 0; // Need 3 points for a turn angle

        for (int i = 1; i < path.corners.Length - 1; i++)
        {
            // Check the angle at the upcoming corner 'i'
            float angle = TurnCheck(path.corners[i - 1], path.corners[i], path.corners[i + 1]);
            float turnThreshold = 10.0f; // Angle threshold in degrees

             // Check distance to this corner - only show turn if approaching it
             float distToCorner = Vector3.Distance(startingPoint.position, path.corners[i]);
             if(distToCorner < 3.0f) // Only show turn if within 3 meters of the corner
             {
                 if (angle > turnThreshold) return 1; // Right turn (positive angle)
                 else if (angle < -turnThreshold) return 2; // Left turn (negative angle)
                 // else it's considered straight at this corner, continue checking next
             } else {
                 // If far from this corner, check the very first segment's direction instead
                 if (i == 1) {
                      Vector3 firstSegmentDir = path.corners[1] - path.corners[0]; // Direction of the first path segment
                      Vector3 currentForward = startingPoint.forward;
                      firstSegmentDir.y = 0;
                      currentForward.y = 0;
                      float initialAngle = Vector3.SignedAngle(currentForward.normalized, firstSegmentDir.normalized, Vector3.up);
                      if (initialAngle > turnThreshold) return 1;
                      else if (initialAngle < -turnThreshold) return 2;
                      else return 0; // Go straight initially
                 }
             }
        }
        return 0; // Default to straight if no significant turns detected or near corners
        */
    }


    // Enable Scanning Logic
    public void EnableScanning()
    {
        if (trackedImageManager != null) trackedImageManager.enabled = true;
        isScanningEnabled = true;

        if (scanUI != null)
        {
             scanUI.SetBool("Scanned", false);
             scanUI.SetBool("NotScanned", true);
             scanUI.SetBool("CamButtonPressed",true); // Assuming this hides/shows UI elements correctly
        }
         else Debug.LogWarning("Animator 'scanUI' not assigned.");

        if (animator != null) animator.SetBool("ButtonPress", false);
        else Debug.LogWarning("Animator 'animator' not assigned.");

        Debug.Log("Scanning re-enabled.");
    }

    // Getter for image name (used by findRoomScript for sorting)
    public string GetImageName()
    {
        // Return the last successfully scanned image name
        return imageName;
    }


    // Toggle Endpoint Logic (Stairs/Elevator)
    public void ToggleEndpoint()
    {
        if (endPoint == null)
        {
            Debug.LogWarning("Cannot toggle endpoint, no destination set.");
            return;
        }

        string currentEndpointName = endPoint.gameObject.name;
        string floorNumber = "";
        Transform newEndPoint = null;
        string newEndPointType = "";

        // Determine current floor based on endpoint name
        if (currentEndpointName.StartsWith("elevator")) floorNumber = currentEndpointName.Substring(8); // Length of "elevator"
        else if (currentEndpointName.StartsWith("stairs")) floorNumber = currentEndpointName.Substring(6); // Length of "stairs"
        else
        {
            Debug.LogWarning($"Current endpoint '{currentEndpointName}' is not stairs or elevator, cannot toggle.");
            return;
        }

        if (string.IsNullOrEmpty(floorNumber))
        {
             Debug.LogError($"Could not extract floor number from endpoint name: {currentEndpointName}");
             return;
        }


        if (currentEndpointName.StartsWith("elevator"))
        {
            // Try switching to stairs
            string stairsName = "stairs" + floorNumber;
            GameObject stairsObject = GameObject.Find(stairsName);
            if (stairsObject != null)
            {
                newEndPoint = stairsObject.transform;
                newEndPointType = "Stairs";
            } else { Debug.LogWarning($"Could not find {stairsName} to toggle to."); }
        }
        else // Must be stairs
        {
            // Try switching to elevator
             string elevatorName = "elevator" + floorNumber;
             GameObject elevatorObject = GameObject.Find(elevatorName);
             if (elevatorObject != null)
             {
                 newEndPoint = elevatorObject.transform;
                 newEndPointType = "Elevator";
             } else { Debug.LogWarning($"Could not find {elevatorName} to toggle to."); }
        }

        // Update if a new endpoint was found
        if (newEndPoint != null)
        {
             endPoint = newEndPoint;
             Debug.Log($"Switched navigation target to {newEndPointType}{floorNumber}");

             // Update UI Text (using null checks)
             if (roomScript != null && roomScript.GetlocationText() != null) // Use getter for safety
             {
                 roomScript.SetlocationText(newEndPoint.name); // Update the main location text via roomScript
             }
             else { Debug.LogError("Cannot update locationText via roomScript!"); }

             if (greenUIText != null)
             {
                 greenUIText.text = $"Navigating to {endPoint.name}";
             }
             else { Debug.LogWarning("TextMeshProUGUI 'greenUIText' not assigned."); }

             // Reset arrival count for the new target
             count = 0;
             if (arrived != null) arrived.gameObject.SetActive(false); // Hide arrival button

             // Immediately recalculate path (optional, Update will catch it)
             // if(startingPoint != null) NavMesh.CalculatePath(startingPoint.position, endPoint.position, NavMesh.AllAreas, path);
        }
        else
        {
             Debug.LogWarning("Could not find alternative (stairs/elevator) on this floor to toggle to.");
        }
    } // End ToggleEndpoint
}
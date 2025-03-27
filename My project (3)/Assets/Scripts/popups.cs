using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    [SerializeField] private Transform popupPrefab;

    private Transform currentPopup;
    private Camera playerPosition; 
    

    private List<Transform> rooms = new();

    void Start()
    {
        List<Transform> roomParents = GetFloorLocations();
        foreach (Transform parent in roomParents)
        {
            foreach (Transform room in parent)
            {   
                rooms.Add(room);
            }
        }
    }

    void Update()
    {
    
        float minDistance = 3.0f;
        Transform closestRoom = null;

        foreach (Transform room in rooms){
            float distance = Vector3.Distance(playerPosition.transform.position, room.position);
            if(distance < minDistance){
                minDistance = distance;
                closestRoom = room;

            }



        }
    

    }


    public List<Transform> GetFloorLocations() {
        List<Transform> locations = new List<Transform>();

        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.name.EndsWith("please"))
            {
                locations.Add(obj);
            }
        }
        return locations;
    }
}

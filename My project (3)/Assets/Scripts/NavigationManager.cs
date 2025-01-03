using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{

    public Transform startingPoint;

    public Transform endPoint;

    public LineRenderer lineRenderer;

    float elapsed; 
    public NavMeshPath path;
    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed > 1.0f){
            elapsed -= 1.0f;
            NavMesh.CalculatePath(startingPoint.position, endPoint.position,NavMesh.AllAreas, path);
        }

        lineRenderer.positionCount=path.corners.Length;
        lineRenderer.SetPositions(path.corners);
    }


    public void NavigateTo(GameObject go){}
}

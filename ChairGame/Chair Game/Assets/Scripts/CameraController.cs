using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    public GameObject cameraCenter;
    public Camera cam;
    public float xOffsetCam;
    public float yOffsetCam;
    public float zOffsetCam;
    public float xMax;
    public float xMin;
    public float yMax;
    public float yMin;

    [Space]
    public bool isCutSceneCam = false;

    [Space]
    public float speed = 0.075f;
    public float max = 60f;
    Vector3 velocity;


    void Start()
    {
        
    }

    
    void Update()
    {
        if (isCutSceneCam == false)
        {
            Vector3 newPlayer = new Vector3(Mathf.Clamp(player.transform.position.x + xOffsetCam, xMin, xMax), Mathf.Clamp(player.transform.position.y + yOffsetCam, yMin, yMax), gameObject.transform.position.z + zOffsetCam);
            gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, newPlayer, ref velocity, speed, max);
        }
    }
}

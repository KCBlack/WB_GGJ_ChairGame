using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject character;
    public float multiplyForce;
    public float maxDist;
    public float offsetCos;
    public GameObject centerOfMass;


    bool drag;
    bool release;
    bool readyToLaunch;
    bool closeToGround;
    Rigidbody2D rb;
    Collider2D col;
    Vector2 force;
    Camera mainCam;
    Vector3 mousePos;
    float newDist;

    void Start()
    {
        drag = false;
        rb = gameObject.GetComponent<Rigidbody2D>();
        //rb.centerOfMass = new Vector3(-1.3f, -0.99f, 0f);
        rb.centerOfMass = centerOfMass.transform.localPosition;
        col = character.GetComponent<Collider2D>();
        mainCam = Camera.main;
        

        readyToLaunch = true;
    }

    //click and hold near player
    //drag away in direction for throw
    //distance difines strength of throw
    //release click adds impulse force to player flinging them around


    private void FixedUpdate()
    {
        //gets mouse position and clamps to max dist
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float dist = Vector3.Distance(rb.centerOfMass, new Vector3(mousePos.x, mousePos.y));
        newDist = Mathf.Clamp(dist, 0.1f, maxDist);


        Vector3 direction = -(character.transform.position - mousePos).normalized;
        if (release == true && readyToLaunch == true)
        {
            rb.AddForce(direction * (offsetCos * (-Mathf.Cos((newDist * multiplyForce) / 3.3f) + 2f) + offsetCos), ForceMode2D.Impulse);
            readyToLaunch = false;
            release = false;
        }
        

    }

    void Update()
    {
        if (Input.GetButtonUp("Fire1") && release == false && readyToLaunch == true)
        {
            Debug.Log("Click_Up");
            if (closeToGround == true)
            {
                // may not be needed?
            }
            release = true;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (readyToLaunch == false)
        {
            readyToLaunch = true;
            Debug.Log("Collision");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //incase the reset in collisionEnter doesnt work
        if (readyToLaunch == false)
        {
            readyToLaunch = true;
            Debug.Log("CollisionStayed");
        }
    }

}

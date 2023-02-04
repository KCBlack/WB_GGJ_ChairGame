using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject character;
    public float multiplyForce;
    public float maxDist;
    float offsetCos;


    bool drag;
    bool release;
    bool readyToLaunch;
    Rigidbody2D rb;
    Collider2D col;
    Vector2 force;
    Camera mainCam;
    Vector3 mousePos;
    float newDist;

    void Start()
    {
        drag = false;
        rb = character.GetComponent<Rigidbody2D>();
        rb.centerOfMass = new Vector3(-1.3f, -0.99f, 0f);
        col = character.GetComponent<EdgeCollider2D>();
        mainCam = Camera.main;
        offsetCos = 6;

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
            rb.AddForce(direction * (offsetCos * (-Mathf.Cos((newDist * multiplyForce) / 5)) + offsetCos), ForceMode2D.Impulse);
            readyToLaunch = false;
            release = false;
        }
        

    }

    void Update()
    {
        if (Input.GetButtonUp("Fire1") && release == false && readyToLaunch == true)
        {
            Debug.Log("Click_Up");
            release = true;
        }


        /* --ignore this--
        Vector2 __pos = new Vector2(character.transform.position.x, character.transform.position.y);
        Vector3 vec3 = __pos + rb.centerOfMass;
        Quaternion quaChar = character.transform.rotation;


        //Debug.DrawLine(__pos, __pos + new Vector2(0, -6));
        */
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (readyToLaunch == false)
        {
            readyToLaunch = true;
            Debug.Log("Collision");
        }
    }

}

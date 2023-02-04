using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject character;
    public float multiplyForce;

    bool drag;






    

    void Start()
    {
        drag = false;
    }




    //click and hold near player
    //drag away in direction for throw
    //distance difines strength of throw
    //release click adds impulse force to player flinging them around


    private void FixedUpdate()
    {
        






    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Click_Down");
            drag = true;
        }
        if (Input.GetButton("Fire1"))
        {
            Debug.Log("Click_Held");
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Click_Up");
            drag = false;
        }
        /*
        while (drag == true)
        {
            return;
        }*/
    }
}

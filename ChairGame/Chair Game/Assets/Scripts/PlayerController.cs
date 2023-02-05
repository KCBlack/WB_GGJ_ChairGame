using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.AnimatedValues;
using UnityEditor.Rendering.LookDev;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;
    [SerializeField]
    private Transform centerOfMass;
    [SerializeField]
    private float rotationForce = 5.0f;
    [SerializeField]
    private float forceMultiplier = 2.0f;
    [SerializeField]
    [Tooltip("How long it takes to reach full charge in seconds")]
    private float chargeSpeed = 1.0f;
    [SerializeField]
    //private readonly float offsetCos;
    private float forceOffset = 2.0f;
    [SerializeField]
    private float groundedAngle = 45f;
    [SerializeField]
    private float groundedGrace = 0.2f;

    private float chargeValue = 0.0f;

    private float groundedGraceTimer = 0.0f;
    private List<(Collider2D collider, Vector2 normal)> currentColliders = new List<(Collider2D, Vector2)>();

    private bool drag;
    private Rigidbody2D rb;
    private Camera mainCam;


    private bool charging = false;
    private Vector2 aimDirection = Vector2.up;
    private Vector2 aimVector;
    private AimType aimType = AimType.Circle;

    private float rotationDirection;

    private enum AimType
    {
        Free,
        Circle
    }

    public void Input_Charge(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            charging = true;
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            charging = false;
        }
    }

    public void Input_AimFree(InputAction.CallbackContext context)
    {
        if (rb == null) return;
        aimType = AimType.Free;
        aimVector = mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void Input_AimCircle(InputAction.CallbackContext context)
    {
        aimType = AimType.Circle;

        Vector2 inputDirection = context.ReadValue<Vector2>();
        aimVector = inputDirection;
    }
    
    public void Input_Rotate(InputAction.CallbackContext context)
    {
        rotationDirection = -context.ReadValue<float>();
    }

    void Start()
    {
        drag = false;
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = rb.transform.InverseTransformPoint(centerOfMass.position);
        mainCam = Camera.main;
    }

    //click and hold near player
    //drag away in direction for throw
    //distance difines strength of throw
    //release click adds impulse force to player flinging them around


    void Update()
    {
        if (charging)
        {
            chargeValue += (Time.deltaTime / chargeSpeed);
        }

        switch(aimType)
        { 
            case AimType.Circle:
                if (aimVector == Vector2.zero)
                    aimDirection = Vector2.up;
                else
                    aimDirection = aimVector.normalized; 
                break;
            case AimType.Free:
                aimDirection = -((Vector2)rb.transform.TransformPoint(rb.centerOfMass) - aimVector).normalized;
                break;
            default:
                aimDirection = Vector2.up;
                break;
        }
        //Debug.DrawRay(rb.transform.TransformPoint(rb.centerOfMass), aimDirection * ((Mathf.PingPong(chargeValue, 1.0f) * forceMultiplier) + forceOffset));
    }
    private void FixedUpdate()
    {
        bool grounded = false;
        // if you are touching the ground you are grounded
        // if not, start counting down the grace timer
        if(currentColliders.Count > 0)
        {
            Vector2 totalNormal = Vector2.zero;
            foreach ((Collider2D collider, Vector2 normal) cn in currentColliders)
            {
                float currentAngle = Vector2.Angle(cn.normal, Vector2.up);
                if(currentAngle <= groundedAngle)
                {
                    grounded = true;
                    break;
                }
                if (grounded) break;
                totalNormal += cn.normal;
            }
            if (!grounded)
            {
                Debug.DrawRay(rb.position, (totalNormal / currentColliders.Count).normalized * 0.5f, Color.yellow);

                if (Vector2.Angle((totalNormal / currentColliders.Count).normalized, Vector2.up) <= groundedAngle)
                    grounded = true;
            }
        }

        if (grounded)
        {
            groundedGraceTimer = 0.0f;
        }
        else
        {
            if (groundedGraceTimer < groundedGrace)
            {
                groundedGraceTimer += Time.fixedDeltaTime;
                if (groundedGraceTimer >= groundedGrace)
                {
                    grounded = false;
                }
                else
                {
                    grounded = true;
                }
            }
        }

        if (!charging)
        {
            if(chargeValue > 0)
            {
                if (grounded)
                {
                    rb.AddForce(aimDirection *
                        ((Mathf.PingPong(chargeValue, 1.0f) * forceMultiplier) + forceOffset),
                        ForceMode2D.Impulse);
                    groundedGraceTimer = groundedGrace;
                    grounded = false;
                }

                chargeValue = 0;
            }
        }

        if(rotationDirection != 0.0f)
        {
            rb.AddTorque(rotationDirection * rotationForce, ForceMode2D.Force);
        }
        /*
        //gets mouse position and clamps to max dist
        if (release == true && readyToLaunch == true)
        {
            rb.AddForce(direction * (offsetCos * (-Mathf.Cos((newDist * multiplyForce) / 3.3f) + 2f) + offsetCos), ForceMode2D.Impulse);
            readyToLaunch = false;
            release = false;
        }
        */
    }

    public float GetChargeValue()
    {
        return chargeValue;
    }


    // track colliders if their surface vector is pointing upwards
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.name);
        int collisionMatch = currentColliders.FindIndex(x => x.collider == collision.collider);
        if(collisionMatch >= 0)
        {
            currentColliders.RemoveAt(collisionMatch);
        }
        Vector2 closestNormal = Vector2.down;
        float closestAngle = 180.0f;
        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            //Debug.DrawRay(contactPoint.point, contactPoint.normal * 0.5f, Color.red);
            float currentAngle = Vector2.Angle(contactPoint.normal, Vector2.up);
            if (currentAngle <= closestAngle)
            {
                closestAngle = currentAngle;
                closestNormal = contactPoint.normal;
            }
        }
        currentColliders.Add((collision.collider, closestNormal));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        int collisionMatch = currentColliders.FindIndex(x => x.collider == collision.collider);
        if(collisionMatch >= 0)
        {
            currentColliders.RemoveAt(collisionMatch);
        }
    }

}

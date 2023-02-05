using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Transform centerOfMass;
    [SerializeField]
    private SpriteRenderer barHolder;
    [SerializeField]
    private SpriteRenderer bar;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private Vector2 uiOffset;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        barHolder.transform.position = centerOfMass.position + new Vector3(uiOffset.x, uiOffset.y, 0);
        bar.transform.rotation = Quaternion.identity;
        barHolder.transform.rotation = Quaternion.identity;
        bar.transform.localScale = new Vector3(Mathf.PingPong(playerController.GetChargeValue(), 1), 1, 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public int nextLevel;
    GameManager gm;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered");
        gm.GetComponent<GameManager>().LoadScene(nextLevel);
    }
}

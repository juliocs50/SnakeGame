using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetecter : MonoBehaviour
{
    void Awake()
    {       
        transform.parent = FindObjectOfType<GameManager>().transform;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.parent.GetComponent<GameManager>().CollisionEnterDetected(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<GameManager>().CollisionEnterDetected(this);
    }
}

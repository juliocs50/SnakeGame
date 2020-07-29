using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    private int index;   
    void Awake()
    {       
        transform.parent = FindObjectOfType<GameManager>().transform;
    }
    
    void Update()
    {        
        index= this.transform.GetSiblingIndex();        
    }
    private void OnDestroy()
    {           
       if (GameManager.SnakeBodySize == index)
        {            
            GameManager.AvailablePositions.Insert(0, new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }         
    }
}

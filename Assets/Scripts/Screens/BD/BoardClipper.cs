using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardClipper : MonoBehaviour
{

    public int order = 0;
    public Vector3 position;

    public float width;
    public float height;
    // Start is called before the first frame update
    void Awake()
    {
        
        position = transform.position;
        width = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        height = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;

        // Debug.Log(width+ " / "+height );

    }
 
}

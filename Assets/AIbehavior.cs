using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 dir = new Vector2(0f, 0f);

        Vector2 movement = dir.normalized * speed;
        rb.velocity = movement;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

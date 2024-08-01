using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;

    [SerializeField]
    private AIshoot shoot;

    public GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 dir = new Vector2(0f, 0f);

        Vector2 movement = dir.normalized * speed;
        rb.velocity = movement;

        if (Time.time >= shoot.nextTimeToFire)
        {
            //if AI shoots ASAP, it needs to determine what angle it should shoot, so that it intercepts teh player perfectly.
            //A. determine how long object
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0f)
            {
                rb.rotation = Mathf.Atan2(player.transform.position.y, player.transform.position.x)*Mathf.Rad2Deg;
            }

            shoot.Shoot();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 predictFuturePos(Vector2 initPos, Vector2 velocity, float time)
    {
        return initPos + time * velocity;
    }
}

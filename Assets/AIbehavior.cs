using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbehavior : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;

    [SerializeField]
    private AIshoot shoot;

    public GameObject ?player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 dir = new Vector2(0f, 0f);

        Vector2 movement = dir.normalized * speed;
        rb.velocity = movement;

        if (Time.time >= shoot.nextTimeToFire)
        {
            //if AI shoots ASAP, it needs to determine what angle it should shoot, so that it intercepts teh player perfectly.
            //A. determine how long object
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0f)
            {

                rb.rotation = Mathf.Atan2(player.transform.position.y-transform.position.y, player.transform.position.x-transform.position.x) * Mathf.Rad2Deg-90f;
            }
            else
            {
                Vector2 playerVel=player.GetComponent<Rigidbody2D>().velocity;
                float estimatedTime = Vector2.Distance(shoot.firePoint.position, player.transform.position) / shoot.bulletSpeed;
                Vector2 futurePos = predictFuturePos(player.transform.position, playerVel, estimatedTime);
                rb.rotation = Mathf.Atan2(futurePos.y - transform.position.y, futurePos.x - transform.position.x)*Mathf.Rad2Deg-90f;
                //predictFuturePos(player.transform, playerVel,t)
            }
            rb.angularVelocity = 0f;
            shoot.Shoot();
        }

    }

    public Vector2 predictFuturePos(Vector2 initPos, Vector2 velocity, float time)
    {
        return initPos + time * velocity;
    }
}

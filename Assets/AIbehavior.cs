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

                rb.rotation = Mathf.Atan2(player.transform.position.y-transform.position.y, player.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
            }
            else
            {
                Vector2 playerVel=player.GetComponent<Rigidbody2D>().velocity;
                float estimatedTime = Vector2.Distance(shoot.firePoint.position, player.transform.position) / shoot.bulletSpeed;
                Vector2 futurePos = predictDir(transform.position, shoot.bulletSpeed, player.transform.position, player.GetComponent<Rigidbody2D>().velocity);
                rb.rotation = Mathf.Atan2(futurePos.y,futurePos.x) * Mathf.Rad2Deg - 90f;
                //predictFuturePos(player.transform, playerVel,t)
                Debug.DrawLine(transform.position, futurePos);
            }
            rb.angularVelocity = 0f;
            shoot.Shoot();
        }

    }

    public Vector2 predictFuturePos(Vector2 initPos, Vector2 velocity, float time)
    {
        return initPos + time * velocity;
    }

    public Vector2 predictDir(Vector2 bulletPos, float bulletSpeed, Vector2 objectPos, Vector2 objectVel)
    {
        //law of cosine

        Vector2 diff = objectPos - bulletPos;
        float distA;
        float distC = diff.magnitude;
        float objectSpeed = objectVel.magnitude;
        float theta = Vector2.Angle(diff, objectVel) * Mathf.Rad2Deg;

        float r = objectSpeed / bulletSpeed;

        if (Solve(1 - r * r, 2 * r * distC * Mathf.Cos(theta), -(distC * distC), out float x1, out float x2))
        {
            distA = Mathf.Max(x1, x2);
            float time = distA / bulletSpeed;
            Vector2 c = bulletPos + objectVel * time;
            Vector2 result = (c - objectPos).normalized;
            return result;

        }
        return Vector2.zero;

    }

    public bool Solve(float a, float b, float c, out float x1, out float x2)
    {
        x1 = 0;
        x2 = 0;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {

            return false;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);

        x1 = (-b + sqrtDiscriminant) / (2 * a);
        x2 = (-b - sqrtDiscriminant) / (2 * a);

        return true;
    }
}

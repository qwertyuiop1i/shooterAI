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

    public float dodgeDistance = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 dir = Vector2.zero;


        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, dodgeDistance);
        /////bool shouldDodge = false;
        Vector2 dodgeDirection = Vector2.zero;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("bullet"))
            {
                shouldDodge = true;
                dodgeDirection = (transform.position - collider.transform.position).normalized;
                dir += dodgeDirection;
                break;
            }
        }



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
                if(InterceptionDirection(player.transform.position,transform.position,playerVel,shoot.bulletSpeed,out var direction))
                {
                    rb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg-90f;
                }
            }
            rb.angularVelocity = 0f;
            shoot.Shoot();
        }

    }




    public Vector2 predictFuturePos(Vector2 initPos, Vector2 velocity, float time)
    {
        return initPos + time * velocity;
    }

    public bool InterceptionDirection(Vector2 a, Vector2 b, Vector2 vA, float sB, out Vector2 result)
    {
        var aToB= b - a;
        var dC = aToB.magnitude;

        var alpha = Vector2.Angle(aToB, vA) * Mathf.Deg2Rad;
        var sA = vA.magnitude;

        var r = sA / sB;


        if (SolveQuadratic(1-r*r,2*r*dC*Mathf.Cos(alpha),-(dC*dC),out var root1, out var root2) == 0)
        {
            result = Vector2.zero;
            return false;
        }

        var dA = Mathf.Max(root1,root2);

        var t = dA / sB;
        var c = a + vA * t;
        result = (c - b).normalized;
        return true;

        
    }

    public int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
    {
        var discriminant=b*b-4*a*c;

        if (discriminant < 0) 
        {
            root1 = Mathf.Infinity;
            root2 = -root1; 
            return 0;
        }

        root1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        root2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        return discriminant > 0 ? 2 : 1;
    }

}

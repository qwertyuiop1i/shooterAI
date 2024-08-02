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

    public float bulletWeight = 2f;

    public float raycastDistance = 8f;

    public float escapeWeight = 6f;

    public float travelWeight = 15f;

    public bool shouldUseLinerenderer = true;
    public LineRenderer lr;

    public float minDodgeAngle=10f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = Vector2.zero;


        int wallCount = 0;
        
        
        //Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        
        //Vector2 escapeDirection = Vector2.zero;

       // foreach (Vector2 x in directions)
       // {
          //  RaycastHit2D hit = Physics2D.Raycast(transform.position, x, raycastDistance, LayerMask.GetMask("walls"));
            //if (hit.collider != null)
        //    {
      //          wallCount++;
    //            escapeDirection -= x;


      //      }
    //    }

        if (wallCount >-1)
        {
           
        //    dir += escapeDirection.normalized*escapeWeight;
            float angle = 0f;
            for (int i = 0; i < 24; i++) 
            {
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up; 
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, LayerMask.GetMask("walls"));

                if (!hit) 
                {
                    dir += direction * escapeWeight; 
                }


                angle += 15f; 
            }
        }


        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, dodgeDistance);
        /////bool shouldDodge = false;
        Vector2 dodgeDirection = Vector2.zero;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("bullet"))
            {
                Rigidbody2D bulletrb = collider.GetComponent<Rigidbody2D>();

                if (ShouldDodge(collider.transform.position,collider.GetComponent<Rigidbody2D>().velocity)) {
                    dodgeDirection = CalculateDodgeDirection(collider.transform.position, collider.GetComponent<Rigidbody2D>().velocity);
                    dir += dodgeDirection * bulletWeight;
                }
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
                    rb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg-90f+Random.Range(-2f,2f);
                }
            }
            rb.angularVelocity = 0f;
            if (shouldUseLinerenderer)
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, (Vector2)transform.position + new Vector2(Mathf.Cos(rb.rotation*Mathf.Deg2Rad+Mathf.PI/2),Mathf.Sin(rb.rotation*Mathf.Deg2Rad+Mathf.PI/2))*50f);
            }

            shoot.Shoot();
        }

    }


    Vector2 CalculateDodgeDirection(Vector2 bulletPosition, Vector2 bulletVelocity)
    {
        Vector2 toBullet = bulletPosition - (Vector2)transform.position;
        Vector2 perpendicularDirection = Vector2.Perpendicular(bulletVelocity.normalized);

        if (Vector2.Dot(perpendicularDirection, toBullet) < 0)
        {
            perpendicularDirection = -perpendicularDirection;
        }

        return perpendicularDirection;
    }
    

    bool ShouldDodge(Vector2 bulletPosition, Vector2 bulletVelocity)
    {
        Vector2 toBullet = bulletPosition - (Vector2)transform.position;
        float dotProduct = Vector2.Dot(toBullet.normalized, bulletVelocity.normalized);

        return dotProduct < 0 && Vector2.Angle(toBullet, -bulletVelocity) < minDodgeAngle;
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

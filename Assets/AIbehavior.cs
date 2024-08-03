using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public float chaseWeight = 15f;


    public bool shouldUseLinerenderer = true;
    public LineRenderer lr;

    public float distanceToMaintain = 15f;

    public float minDodgeAngle=10f;

    public Tilemap level;

    public int[,] grid;

    

    public class Node
    {
        public int X;
        public int Y;
        public float GCost;
        public float HCost;
        public float FCost;
        public Node parent;

        public Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        
        level.CompressBounds();
        //Debug.Log(level.GetTile(level.WorldToCell(new Vector2(-43.5f, 25.5f))));

        grid = new int[level.size.x, level.size.y];

        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                if (level.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    grid[x, y] = 0;
                }
                else
                {
                    grid[x, y] = 1;
                }
                
            }
        }
    }
    
    public Vector2 NextDir(Vector3 start, Vector3 goal)
    {


        Node startPos =new Node(level.WorldToCell(start).x,level.WorldToCell(start).y);
        Node endPos = new Node(level.WorldToCell(goal).x, level.WorldToCell(goal).y);

        int gridWidth = level.size.x;
        int gridHeight = level.size.y;

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        openList.Add(startPos);

        bool ValidNode(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight && grid[x, y] == 0;
        }

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            for(int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost)
                {
                    currentNode = openList[i];
                }

            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endPos)
            {
                List<Node> path = new List<Node>();
                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }
                path.Reverse();
                var worldCoords =level.CellToWorld(new Vector3Int(path[1].X, path[1].Y,0));
                return new Vector2(worldCoords.x,worldCoords.y);

            }

            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int neighborX = currentNode.X + x;
                    int neighborY = currentNode.Y + y;

                    if (!ValidNode(neighborX, neighborY))
                    {
                        continue;
                    }

                    Node neighbor = new Node(neighborX, neighborY);
                    neighbor.GCost = currentNode.GCost + 1;
                    neighbor.HCost = HeuristicDistance(neighbor, endPos);
                    neighbor.FCost = neighbor.GCost + neighbor.HCost;
                    neighbor.parent = currentNode;

                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }
                    if (!openList.Contains(neighbor) || neighbor.GCost < openList[openList.IndexOf(neighbor)].GCost)
                    {
                        openList.Add(neighbor);
                    }
            

                }
            }
        }


        return transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 chaseVector=chaseWeight*NextDir(transform.position,new Vector3(0,0,0));


        Vector2 dir = Vector2.zero;


        int wallCount = 0;
        
        


        if (wallCount >-1)
        {

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
                    dir += dodgeDirection * bulletWeight*(3f/Vector2.Distance(transform.position,collider.transform.position));
                }
            }

        }
        //dir += chaseVector;


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

        return -perpendicularDirection;
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

    public void toggleDebugMode()
    {
        shouldUseLinerenderer = !shouldUseLinerenderer;
    }

    private float HeuristicDistance(Node a, Node b)
    {
        return Vector2.Distance(new Vector2(a.X,a.Y), new Vector2(b.X,b.Y));//pythag heusrtic.
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIshoot : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject player;

    public float fireRate = 3f;
    public float bulletSpeed = 10f;

    private float nextTimeToFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Rigidbody2D>().velocity = firePoint.up * bulletSpeed;

        ///add prediction. if i shoot right now this instance, where do i need to shoot to guarrantee it hits the player, assumngi the player travels at the ssame velocity.
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIshoot : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;
   

    public float fireRate = 3f;
    public float bulletSpeed = 10f;

    public float nextTimeToFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Rigidbody2D>().velocity = firePoint.up * bulletSpeed;

        nextTimeToFire = Time.time + 1f / fireRate;

        ///add prediction. if i shoot right now this instance, where do i need to shoot to guarrantee it hits the player, assumngi the player travels at the ssame velocity.
    }
}

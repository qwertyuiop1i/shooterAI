using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 3f;
    public float bulletSpeed = 10f;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            Shoot();
            
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Rigidbody2D>().velocity = firePoint.up * bulletSpeed;
        nextTimeToFire = Time.time + 1f / fireRate;
    }
}

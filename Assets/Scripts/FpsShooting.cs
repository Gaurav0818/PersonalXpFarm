using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public PlayerController player; 
    public Transform bulletLocation;
    public ParticleSystem MuzzleFlash;
    public float shootPower = 100f;

    void start()
    {
        if (bulletLocation == null)
            bulletLocation = transform;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && player.canShoot)
        {
            MuzzleFlash.Play();
           //Instantiate(bulletPrefab, bulletLocation.position, bulletLocation.rotation).
           //        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * shootPower);
        }
    }
}

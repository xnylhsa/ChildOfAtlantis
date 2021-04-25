using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooterController : MonoBehaviour
{
    public Transform firingPoint;
    public GameObject projectilePrefab;
    public float shootInterval = 1.0f;
    float shootTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if(shootTimer > shootInterval)
        {
            shoot();
            shootTimer = 0.0f;
        }
    }

    void shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = firingPoint.transform.position;
    }
}

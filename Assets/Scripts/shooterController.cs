using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooterController : MonoBehaviour
{
    public Transform firingPoint;
    public GameObject projectilePrefab;
    public float maxShootInterval = 1.0f;
    public float projectileSpeed = 1.0f;
    public Vector2 projectileDirection = Vector2.right;
    public float projectileDeathTime = 1.0f;
    float shootTimer = 0.0f;
    public float startingOffset = 0.0f;
    public float minShootInterval = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        shootTimer += startingOffset;
        if (minShootInterval > maxShootInterval)
            minShootInterval = maxShootInterval;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;
        if(shootTimer <= 0.0f)
        {
            shoot();
            shootTimer = Random.Range(minShootInterval, maxShootInterval);
        }
    }

    void shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = firingPoint.transform.position;
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.projectileSpeed = projectileSpeed;
        projectileController.projectileDirection = projectileDirection;
        projectileController.deathTime = projectileDeathTime;

    }
}

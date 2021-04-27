using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PufferFishController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float maxShootInterval = 2.0f;
    public float minShootInterval = 2.0f;
    public float startingShootOffset = 0.0f;
    public float projectileSpeed = 4.0f;
    public float firingDelayOnDetection = 0.1f;
    public float projectileAngleOffset = 90.0f;
    public float numberOfProjectiles = 4;
    public float firstProjectileAngleOffset = 0.0f;
    public float projectileDeathTime = 1.0f;

    float shootTimer = 0.0f;
    bool shooting = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == 3)
        {
            if (shootTimer <= 0.0f && !shooting)
            {
                StartCoroutine(delayedShoot(other.transform.position - transform.position));
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;

    }

    IEnumerator delayedShoot(Vector2 firingDirection)
    {
        shooting = true;
        yield return new WaitForSeconds(firingDelayOnDetection);
        shoot(firingDirection);
    }

    void shoot(Vector2 firingDirection)
    {
        Vector2 firstDirection = Quaternion.Euler(0.0f, 0.0f, firstProjectileAngleOffset) * firingDirection;
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.transform.position = transform.position;
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            projectileController.projectileSpeed = projectileSpeed;
            projectileController.projectileDirection = Quaternion.Euler(0.0f, 0.0f, projectileAngleOffset * i) * firstDirection;
            projectileController.deathTime = projectileDeathTime;
        }
        shooting = false;
        shootTimer = Random.Range(minShootInterval, maxShootInterval);
    }
}

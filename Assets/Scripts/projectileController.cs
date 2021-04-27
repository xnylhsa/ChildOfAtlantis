using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChildOfAtlantis.Mechanics;
public class ProjectileController : MonoBehaviour
{
    public float projectileSpeed = 1.0f;
    public Vector2 projectileDirection = Vector2.right;
    public float deathTime = 3.0f;
    float deathTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(projectileDirection * projectileSpeed * Time.deltaTime);
        deathTimer += Time.deltaTime;
        if(deathTimer > deathTime)
        {
            Destroy(gameObject);
        }
    }
}

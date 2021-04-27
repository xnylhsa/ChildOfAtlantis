using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChildOfAtlantis.Mechanics;
public class BombfishController : MonoBehaviour
{
    public DetectionArea detectionArea;
    public float movementSpeed = 4.0f;
    public float timeUntilExplosion = 1.5f;
    public float explosionDistance = 1.5f;
    public float explosionTriggerDistance = 0.25f;

    float bombTimer;
    PlayerController target = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (detectionArea && detectionArea.isPlayerDetected && target == null)
        {
            target = detectionArea.player.GetComponent<PlayerController>();
            bombTimer = timeUntilExplosion;
        }

        if(target)
        {
            bombTimer -= Time.deltaTime;
            if(bombTimer <= 0.0f)
            {
                Explode();
            }
            transform.Translate((target.transform.position - (transform.position)).normalized * movementSpeed * Time.deltaTime);
            if (Mathf.Abs(Vector2.Distance(target.transform.position, transform.position)) < explosionTriggerDistance)
            {
                Explode();
            }
        }

    }


    void Explode()
    {
        if(Mathf.Abs(Vector2.Distance(target.transform.position, transform.position)) < explosionDistance)
        {
            target.takeDamage();
        }
        //Play explosion
        Destroy(gameObject);
    }
}

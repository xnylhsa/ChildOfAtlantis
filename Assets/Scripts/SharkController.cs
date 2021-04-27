using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    public DetectionArea detectionArea;
    Vector2 home;
    public float movementSpeed;
    public float playedDetectedMovementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        home = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(detectionArea && detectionArea.isPlayerDetected)
        {
            GameObject player = detectionArea.player;
            transform.Translate((player.transform.position - (transform.position)).normalized * playedDetectedMovementSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate((home - (Vector2)(transform.position)).normalized * playedDetectedMovementSpeed * Time.deltaTime);

        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChildOfAtlantis.Mechanics;
public class swimmerController : MonoBehaviour
{
    public List<Vector2> wanderLocations;
    [Range(0,25.0f)]
    public float movementSpeed = 2.0f;
    int currentWanderLocation = 0;
    Vector2 origin;
    public bool randomizePattern = false;
    SpriteRenderer spriteRenderer;
    bool hasSpriteRenderer = false;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer)
        {
            hasSpriteRenderer = true;
        }
        origin = transform.position;
        if(randomizePattern && wanderLocations.Count >= 2)
        {
            Vector2 randomizeStartPos;
            randomizeStartPos.x = Random.Range(origin.x + wanderLocations[0].x, origin.x + wanderLocations[1].x);
            randomizeStartPos.y = Random.Range(origin.y + wanderLocations[0].y, origin.y + wanderLocations[1].y);
            transform.position = randomizeStartPos;
            currentWanderLocation = Random.Range(0, wanderLocations.Count - 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        if(!Application.isPlaying)
        {
            origin = transform.position;
        }
        for(int i = 0; i < wanderLocations.Count; i++)
        {
            Vector2 WanderWorldLocation = origin + wanderLocations[i];

            Gizmos.DrawCube(WanderWorldLocation, Vector3.one * 0.25f);
        }
       


    }

    // Update is called once per frame
    void Update()
    {
        if (wanderLocations.Count > 0 && currentWanderLocation < wanderLocations.Count)
        {
            Vector2 WanderWorldLocation = origin + wanderLocations[currentWanderLocation];
            transform.Translate((WanderWorldLocation - (Vector2)(transform.position)).normalized * movementSpeed * Time.deltaTime);
            if(hasSpriteRenderer)
            {
                spriteRenderer.flipX = (WanderWorldLocation - (Vector2)(transform.position)).normalized.x < 0 ? false : true;
            }
            Vector2.Distance(transform.position, WanderWorldLocation);
            if (Mathf.Abs(Vector2.Distance(transform.position, WanderWorldLocation)) < 0.1f)
            {
                currentWanderLocation++;
                if (currentWanderLocation >= wanderLocations.Count)
                    currentWanderLocation = 0;
            }
        }
    }
}

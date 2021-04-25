using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChildOfAtlantis.Mechanics;
public class swimmerController : MonoBehaviour
{

    public float wanderDistance;
    public int startingDirection = -1;
    public float movementSpeed = 2.0f;
    int direction = 0;
    public float startingXpos;
    // Start is called before the first frame update
    void Start()
    {
        direction = startingDirection;
        startingXpos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == -1)
        {
            transform.Translate(Vector2.left * movementSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - (startingXpos - wanderDistance)) < 0.1f)
            {
                direction = 1;
            }
        }
        if (direction == 1)
        {
            transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - (startingXpos + wanderDistance)) < 0.1f)
            {
                direction = -1;
            }
        }
    }
}

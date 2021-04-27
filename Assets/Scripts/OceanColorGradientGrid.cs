using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OceanColorGradientGrid : MonoBehaviour
{
    public List<GameObject> oceanGradients;
    public int numberOfColumns;
    public Vector2 topLeft;
    List<GameObject> oceanTiles;
    // Start is called before the first frame update
    void Start()
    {
        topLeft = gameObject.transform.position;
        oceanTiles = new List<GameObject>(oceanGradients.Count * numberOfColumns);
        if (oceanTiles.Count != oceanGradients.Count * numberOfColumns)
        {
            for (int i = 0; i < oceanGradients.Count; i++)
            {
                if (!oceanGradients[i]) return;

                for (int j = 0; j < numberOfColumns; j++)
                {
                    int index = oceanTiles.Count;
                    oceanTiles.Add(Instantiate(oceanGradients[i]));
                    oceanTiles[index].transform.parent = transform;
                }
            }
        }
        SpriteRenderer spriteRenderer = oceanGradients[0].GetComponent<SpriteRenderer>();
        float xSize = spriteRenderer.sprite.bounds.size.x;
        float ySize = spriteRenderer.sprite.bounds.size.y;
        for (int i = 0; i < oceanGradients.Count; i++)
        {
            if (!oceanGradients[i]) return;
            for (int j = 0; j < numberOfColumns; j++)
            {
                int index = i * numberOfColumns + j;
                float x = topLeft.x + (xSize * j);
                float y = topLeft.y - (ySize * i);
                oceanTiles[index].transform.position = new Vector3(x, y, 1.0f);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {

    }
}

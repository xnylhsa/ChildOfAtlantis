using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct TerrainGridInfo
{
    public int row;
    public int col;
    public GameObject terrainGrid;
}


public class TerrainController : MonoBehaviour
{
    public List<TerrainGridInfo> terrainGridInfos;
    public Vector2 topLeft;
    public float xDistance = 4096.0f / 100.0f;
    public float yDistance = 2304.0f / 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        SpriteRenderer spriteRenderer = terrainGridInfos[0].terrainGrid.GetComponent<SpriteRenderer>();
        xDistance = spriteRenderer.sprite.bounds.size.x;
        yDistance = spriteRenderer.sprite.bounds.size.y;

        for (int i = 0; i < terrainGridInfos.Count; i++)
        {
            if(terrainGridInfos[i].terrainGrid != null)
            {

                float x = topLeft.x + (xDistance * terrainGridInfos[i].col);
                float y = topLeft.y - (yDistance * terrainGridInfos[i].row);
                terrainGridInfos[i].terrainGrid.transform.position = new Vector3(x, y, 1.0f);
            }


        }

    }
}

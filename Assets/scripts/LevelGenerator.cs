using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public GameObject StaticBlockPrefab;
    public GameObject DynamicBlockPrefab;
    public GameObject []WallPrefab;
    public GameObject FloorPrefab;
    public GameObject RefPoint;
    Vector3 floorOffset = new Vector3(2, 0, 2);
    int width = 2;
    int height = 2;
	// Use this for initialization
	void Start () {
        width = Random.Range(25, 25);
        height = Random.Range(25, 25);
        FloorPrefab.transform.localScale = new Vector3(width + 4, 0.2f, height + 4);
        Invoke("GenerateWorld", 0.5f);
    }

    void GenerateWalls()
    {
        Vector3 refPos = RefPoint.transform.position + floorOffset;
        Vector3 scale = WallPrefab[0].transform.localScale;
        int offset = 1;
        WallPrefab[0].transform.localScale = new Vector3(width + offset, scale.y, scale.z);
        WallPrefab[1].transform.localScale = new Vector3(width + offset, scale.y, scale.z);
        WallPrefab[2].transform.localScale = new Vector3(scale.x, scale.y, height + offset);
        WallPrefab[3].transform.localScale = new Vector3(scale.x, scale.y, height + offset);
        WallPrefab[0].transform.position = new Vector3(refPos.x + (width ) / 2, refPos.y, refPos.z - offset);
        WallPrefab[1].transform.position = new Vector3(refPos.x + (width ) / 2, refPos.y , refPos.z + height );
        WallPrefab[2].transform.position = new Vector3(refPos.x - offset, refPos.y, refPos.z + (height ) / 2 );
        WallPrefab[3].transform.position = new Vector3(refPos.x + width , refPos.y, refPos.z + (height ) / 2 );
    }

    void GenerateWorld()
    {
        Vector3 refPos = RefPoint.transform.position + floorOffset;
        FloorPrefab.transform.position = RefPoint.transform.position;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if ((i % 2 == 1) && (j % 2 == 1))
                    Instantiate<GameObject>(StaticBlockPrefab, refPos + new Vector3(i, 0.5f, j), Quaternion.identity);
                else
                {
                    if (i > 3 && ((i - 1) < width))
                        if (j > 3 && ((j - 1) < height))
                            if (Random.Range(0, 100) < 30)
                                Instantiate<GameObject>(DynamicBlockPrefab, refPos + new Vector3(i, 0.5f, j), Quaternion.identity);
                }
            }
        }
        // Generate Walls
        GenerateWalls();
    }
	// Update is called once per frame
	void Update () {
		
	}
}

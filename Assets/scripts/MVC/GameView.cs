using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using thelab.mvc;
using UnityEngine.Events;

public class GameView : View<Game>
{
    public GameObject StaticBlockPrefab;
    public GameObject DynamicBlockPrefab;
    public GameObject[] WallPrefab;
    public GameObject FloorPrefab;
    public GameObject RefPoint;
    Vector3 floorOffset = new Vector3(2, 0, 2);
    public void Init()
    {
    }
    void GenerateWalls(int Row, int Col)
    {
        Vector3 refPos = RefPoint.transform.position + floorOffset;
        Vector3 scale = WallPrefab[0].transform.localScale;
        int offset = 1;
        WallPrefab[0].transform.localScale = new Vector3(Row + offset, scale.y, scale.z);
        WallPrefab[1].transform.localScale = new Vector3(Row + offset, scale.y, scale.z);
        WallPrefab[2].transform.localScale = new Vector3(scale.x, scale.y, Col + offset);
        WallPrefab[3].transform.localScale = new Vector3(scale.x, scale.y, Col + offset);
        WallPrefab[0].transform.position = new Vector3(refPos.x + (Row) / 2, refPos.y, refPos.z - offset);
        WallPrefab[1].transform.position = new Vector3(refPos.x + (Row) / 2, refPos.y, refPos.z + Col);
        WallPrefab[2].transform.position = new Vector3(refPos.x - offset, refPos.y, refPos.z + (Col) / 2);
        WallPrefab[3].transform.position = new Vector3(refPos.x + Row, refPos.y, refPos.z + (Col) / 2);
    }

    public void GenerateWorld()
    {
        Level level = app.model.GetLevel();
        Debug.Log(" Row : " + level.Row + " Col : " + level.Col);
        Vector3 refPos = RefPoint.transform.position + floorOffset;
        FloorPrefab.transform.position = RefPoint.transform.position;
        FloorPrefab.transform.localScale = new Vector3(level.Row + 4, 0.2f, level.Col + 4);
        foreach (Block blk in level.LevelData)
        {
            int r;
            int c;
            level.GetRCFromID(blk.id, out r, out c);
            switch (blk.type)
            {
                case GameModel.BlockType.NonBreakable:
                    {
                        GameObject obj = Instantiate<GameObject>(StaticBlockPrefab, refPos + new Vector3(r, 0.5f, c), Quaternion.identity);
                        obj.name = blk.id.ToString();
                        break;
                    }
                case GameModel.BlockType.Breakable:
                    {
                        switch (blk.subType)
                        {
                            case GameModel.SubType.Empty:
                                {
                                    break;
                                }
                            default:
                                {
                                   // Instantiate<GameObject>(DynamicBlockPrefab, refPos + new Vector3(r, 0.5f, c), Quaternion.identity);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        GenerateWalls(level.Row, level.Col);
    }
    void Update()
    {

    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using thelab.mvc;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Utility;

public class GameView : View<Game>
{
    public GameObject StaticBlockPrefab;
    public GameObject DynamicBlockPrefab;
    public GameObject[] WallPrefab;
    public GameObject FloorPrefab;
    public GameObject RefPoint;
    public GameObject PlayerPrefab;
    public Transform BlocksRoot;
    public FollowTarget GameCamera;

    Vector3 floorOffset = Vector3.zero;// new Vector3(2, 0, 2);

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
        Vector3 refPos = RefPoint.transform.position + floorOffset;
        FloorPrefab.transform.position = Vector3.zero;
        FloorPrefab.transform.localScale = new Vector3(level.Row + 4, 0.2f, level.Col + 4);
        foreach (Block blk in level.LevelData)
        {
            int r;
            int c;
            Utils.GetRCFromID(blk.id, out r, out c);
            switch (blk.type)
            {
                case GameModel.BlockType.NonBreakable:
                    {
                        GameObject obj = Instantiate<GameObject>(StaticBlockPrefab, refPos + new Vector3(r, 0.5f, c), Quaternion.identity);
                        obj.name = blk.id.ToString();
                        obj.transform.parent = BlocksRoot;
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
                                    GameObject obj = Instantiate<GameObject>(DynamicBlockPrefab, refPos + new Vector3(r, 0.5f, c), Quaternion.identity);
                                    obj.name = blk.id.ToString();
                                    obj.transform.parent = BlocksRoot;
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        GenerateWalls(level.Row, level.Col);
    }
    public void SpawnPlayers()
    {
        SetupPlayer(1);
        SetupPlayer(2);
    }

    void SetupPlayer(int player_no)
    {
        Vector3 refPos = RefPoint.transform.position + floorOffset;
        int r;
        int c;
        Player info = app.model.GetPlayer(player_no);
        Utils.GetRCFromID(info.CurrentCell, out r, out c);
        GameObject obj = Instantiate<GameObject>(PlayerPrefab, refPos + new Vector3(r, 0.5f, c), Quaternion.identity);
        obj.name = "P" + player_no;
        obj.transform.parent = BlocksRoot;
        Dictionary<string, string> param_list = new Dictionary<string, string>();
        param_list.Add("Horizontal", "Horizontal" + obj.name);
        param_list.Add("Vertical", "Vertical" + obj.name);
        param_list.Add("Action", ((player_no == 1)?"right ":"left ") + "ctrl" );
        param_list.Add("Id", player_no.ToString());
        ThirdPersonUserControl user_control = obj.GetComponent<ThirdPersonUserControl>();
        user_control.Init(param_list);
        GameCamera.SetPlayer(player_no, obj.transform);
    }

}

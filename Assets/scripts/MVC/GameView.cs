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
    public GameObject AIPrefab;
    public Transform BlocksRoot;
    public FollowTarget GameCamera;

    public Pickup BombCountPrefab;
    public Pickup BombRangetPrefab;
    public Pickup RunSpeedPrefab;
    public Pickup EnableRemotePrefab;

    public Text TimerText;
    public Text P1BombCount;
    public Text P1BombRange;
    public Text P1RunnerSpeed;
    public Text P1Remote;
    public Text P1Score;
    public Text P2BombCount;
    public Text P2BombRange;
    public Text P2RunnerSpeed;
    public Text P2Remote;
    public Text P2Score;

    public Text ResultText;

    public GameObject GameOverScreen;

    Dictionary<string, string> HudData = new Dictionary<string, string>();
    List<GameObject> AIPlayers = new List<GameObject>();

    void GenerateWalls(int Row, int Col)
    {
        Vector3 refPos = RefPoint.transform.position;
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
    public void Init()
    {
        HudData.Add("LevelTimer", Utils.LevelFinishTime.ToString());
        HideGameOverScreen();
    }
    public void GenerateWorld()
    {
        Level level = app.model.GetLevel();
        Vector3 refPos = RefPoint.transform.position;
        FloorPrefab.transform.position = Vector3.zero;
        int padding = 2;
        FloorPrefab.transform.localScale = new Vector3(level.Row + padding, 0.2f, level.Col + padding);
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

    public void SpawnAI()
    {
        Vector3 refPos = RefPoint.transform.position;
        for (int i = 0; i < Utils.AICount; i++)
        {
            Player info = app.model.GetAI(i);
            int r;
            int c;
            Utils.GetRCFromID(info.CurrentCell, out r, out c);
            GameObject obj = Instantiate<GameObject>(AIPrefab, refPos + new Vector3(r, 0.5f, c), Quaternion.identity);
            obj.name = "AI" + i;
            obj.transform.parent = BlocksRoot;
            AINavigation ai = obj.GetComponent<AINavigation>();
            ai.Init(info);
            AIPlayers.Add(obj);
        }

    }

    void SetupPlayer(int player_no)
    {
        Vector3 refPos = RefPoint.transform.position;
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

    public void DestroyBlocks(List<Block> blockList, Player p)
    {
        foreach (Block blk in blockList)
        {
            Transform t = BlocksRoot.Find(blk.id.ToString());
            SpawnPickupAt(blk);
            if (t != null)
                Destroy(t.gameObject);
            for (int i = 0; i < Utils.AICount; i++)
            {
                if (app.model.GetAI(i).CurrentCell == blk.id && app.model.GetAI(i).IsAlive())
                {
                    app.model.GetAI(i).Die();
                    AIPlayers[i].SetActive(false);
                    p.AddScore(Utils.AIKillPoints);
                }
            }
        }
    }

    public void SpawnPickupAt(Block blk)
    {
        int x, z;
        Utils.GetRCFromID(blk.id, out x, out z);
        Vector3 position = new Vector3(x,0.25f, z);
        Pickup obj = null;
        if (blk.subType == GameModel.SubType.BombCountUp)
            obj = Instantiate<Pickup>(BombCountPrefab, position, Quaternion.Euler(90,0,0));
        else if (blk.subType == GameModel.SubType.BombRangeUp)
            obj = Instantiate<Pickup>(BombRangetPrefab, position, Quaternion.Euler(90, 0, 0));
        else if (blk.subType == GameModel.SubType.SpeedUp)
            obj = Instantiate<Pickup>(RunSpeedPrefab, position, Quaternion.Euler(90, 0, 0));
        else if (blk.subType == GameModel.SubType.RemoteDetonator)
            obj = Instantiate<Pickup>(EnableRemotePrefab, position, Quaternion.Euler(90, 0, 0));
    }

    public void UpdateHudData(string key, string data)
    {
        if (HudData.ContainsKey(key))
            HudData[key] = data;
        else
        {
            Debug.Log(" Hud Key " + key + " Not found. Adding the key instead");
            HudData.Add(key, data);
        }
        UpdateHud();
    }

    public void UpdateHud()
    {
        if (HudData.ContainsKey("LevelTimer"))
        {
            TimerText.text = HudData["LevelTimer"];
        }
        Player info = app.model.GetPlayer(1);
        if (info != null)
        {
            P1BombCount.text = info.GetBombCount().ToString();
            P1BombRange.text = info.GetBombRange().ToString();
            P1RunnerSpeed.text = info.GetRunSpeed().ToString();
            P1Remote.text = info.GetRemoteDetonate() ? "ON" : "Off";
            P1Score.text = info.GetScore().ToString();
        }
        info = app.model.GetPlayer(2);
        if (info != null)
        {
            P2BombCount.text = info.GetBombCount().ToString();
            P2BombRange.text = info.GetBombRange().ToString();
            P2RunnerSpeed.text = info.GetRunSpeed().ToString();
            P2Remote.text = info.GetRemoteDetonate() ? "ON" : "Off";
            P2Score.text = info.GetScore().ToString();
        }
    }
    public void ShowGameOverScreen(string result)
    {
        ResultText.text = result;
        GameOverScreen.transform.DOScale(1, 1.0f);
    }
    public void HideGameOverScreen()
    {
        ResultText.text = "";
        GameOverScreen.transform.DOScale(0, 0.1f);
    }
}

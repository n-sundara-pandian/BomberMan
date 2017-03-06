using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;


public class GameModel : Model<Game>
{
    public enum BlockType
    {
        NonBreakable,
        Breakable,
    }

    public enum SubType
    {
        Empty,
        Breakable,
        SpeedUp,
        BombRangeUp,
        BombCountUp,
        RemoteDetonator,
    }
    Level GameLevel = new Level();
    Player Player1 = new Player();
    Player Player2 = new Player();
    List<Player> AIList = new List<Player>();

    bool bGameOver = false;
    public void Init()
    {
        List<int> TempOccupiedBlocks = new List<int>();
        Utils.Row = Random.Range(20, 6);
        Utils.Col = Random.Range(20, 10);
        GameLevel.Init();
        Player1.Init(0, GetEmptyBlock(ref TempOccupiedBlocks));
        Player2.Init(1, GetEmptyBlock(ref TempOccupiedBlocks));
        for (int i = 0; i < Utils.AICount; i++)
        {
            Player p = new Player();
            p.Init(i, GetEmptyBlock(ref TempOccupiedBlocks));
            AIList.Add(p);
        }
        bGameOver = false;
    }
    int GetEmptyBlock(ref List<int> TempOccupiedBlocks)
    {
        int empty = GameLevel.GetEmptyBlock().id;
        while (TempOccupiedBlocks.Contains(empty))
            empty = GameLevel.GetEmptyBlock().id;
        TempOccupiedBlocks.Add(empty);
        return empty;
    }
    public Level GetLevel()
    {
        return GameLevel;
    }
    public Player GetPlayer(int player_no)
    {
        if (player_no == 2) return Player2;
        return Player1;
    }
    public Player GetAI(int id)
    {
        return AIList[id];
    }
    public void DestroyBlocks(List<Block> DestoyList, Player p)
    {
        int score = 0;
        foreach (Block blk in DestoyList)
        {
            if (Player1.CurrentCell == blk.id)
            {
                Player1.Die();
                app.view.PlayOneShot("death");
                SetGameOver(true);
            }
            if (Player2.CurrentCell == blk.id)
            {
                Player2.Die();
                app.view.PlayOneShot("death");
                SetGameOver(true);
            }
            if (!blk.IsNavigable()) score += Utils.BlockBreakPoints;

            GameLevel.Get(blk.id).MakeitEmpty();
        }
        p.AddScore(score);
    }
    public Player OnCharacterTrigger(string playerName)
    {
        if (playerName == "P1") return GetPlayer(1);
        if (playerName == "P2") return GetPlayer(2);
        return null;
    }
    public void SetGameOver(bool flag)
    {
        bGameOver = flag;
    }
    public bool GetGameOver()
    {
        if (!Player1.IsAlive() || !Player2.IsAlive())
            bGameOver = true;
        return bGameOver;
    }
}
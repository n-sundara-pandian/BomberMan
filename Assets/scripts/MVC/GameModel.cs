using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class Level
{
    public List<Block> LevelData = new List<Block>();
    public List<Block> EmptyBlocks = new List<Block>();
    public List<Block> SpecialBlocks = new List<Block>();
    public int Row;
    public int Col;
    public void Init()
    {
        Row = Utils.Row;
        Col = Utils.Col;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                Block block = new Block();
                int id = Utils.GetIDFromRC(i, j); 
                if ((i % 2 == 1) && (j % 2 == 1))
                {
                    block.Init(id, GameModel.BlockType.NonBreakable, GameModel.SubType.Empty);
                }
                else
                {
                    block.Init(id, GameModel.BlockType.Breakable, GetSubBlock(i, j));
                    if (block.subType == GameModel.SubType.Empty)
                        EmptyBlocks.Add(block);
                    else
                        SpecialBlocks.Add(block);
                }
                LevelData.Add(block);
            }
        }
    }
    GameModel.SubType GetSubBlock(int r, int c)
    {
        GameModel.SubType st = GameModel.SubType.Empty;
        if (r > 3 && ((r - 1) < Row))
            if (c > 3 && ((c - 1) < Col))
            {
                int seed = Random.Range(0, 100);
                if (seed > 30) st = GameModel.SubType.Empty;
                else if (seed > 25) st = GameModel.SubType.Breakable;
                else if (seed > 20) st = GameModel.SubType.BombRangeUp;
                else if (seed > 15) st = GameModel.SubType.BombCountUp;
                else if (seed > 10) st = GameModel.SubType.SpeedUp;
                else  st = GameModel.SubType.RemoteDetonator;
            }
        return st;
    }

    public Block Get(int id)
    {
        return LevelData[id];
    }
    public Block GetEmptyBlock()
    {
        return EmptyBlocks[Random.Range(0, EmptyBlocks.Count)];
    }
}
public struct Block
{
    public int id;
    public GameModel.BlockType type;
    public GameModel.SubType subType;

    public void Init(int i, GameModel.BlockType t, GameModel.SubType st)
    {
        id = i;
        type = t;
        subType = st;
    }
}
public class GameModel : Model<Game>
{
    public enum BlockType
    {
        NonBreakable,
        Breakable,
        Gate,
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
    public void Init()
    {
        Utils.Row = Random.Range(10, 15);
        Utils.Col = Random.Range(20, 25);
        GameLevel.Init();
        int p1_spawn_block = GameLevel.GetEmptyBlock().id;
        Player1.Init(0, p1_spawn_block);
        int p2_spawn_block = GameLevel.GetEmptyBlock().id;
        while(p1_spawn_block == p2_spawn_block)
            p2_spawn_block = GameLevel.GetEmptyBlock().id;
        Player2.Init(1, p2_spawn_block);
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
    void Update()
    {
    }
}
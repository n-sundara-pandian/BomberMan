using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public struct Level
{
    public List<Block> LevelData;
    public int Row;
    public int Col;
    public void Init(int r, int c )
    {
        LevelData = new List<Block>();
        Row = r;
        Col = c;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                Block block = new Block();
                int id = GetIDFromRC(i, j); 
                if ((i % 2 == 1) && (j % 2 == 1))
                {
                    block.Init(id, GameModel.BlockType.NonBreakable, GameModel.SubType.Empty);
                }
                else
                {
                    block.Init(id, GameModel.BlockType.Breakable, GetSubBlock(i, j));
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

    public int GetIDFromRC(int r,int c)
    {
        return r* Col +c;
    }
    public void GetRCFromID(int id, out int r, out int c)
    {
        r = id / Col;
        c = id % Col;
    }
    public Block Get(int id)
    {
        return LevelData[id];
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
    public void Init()
    {
        GameLevel.Init(Random.Range(20, 25), Random.Range(20, 25));
    }
    public Level GetLevel()
    {
        return GameLevel;
    }
    void Update()
    {
    }
}
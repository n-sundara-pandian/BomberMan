using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class Level
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
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
        int seed = Random.Range(0, 100);
        if (seed > 30) st = GameModel.SubType.Empty;
        else if (seed > 25) st = GameModel.SubType.Breakable;
        else if (seed > 20) st = GameModel.SubType.BombRangeUp;
        else if (seed > 15) st = GameModel.SubType.BombCountUp;
        else if (seed > 10) st = GameModel.SubType.SpeedUp;
        else  st = GameModel.SubType.RemoteDetonator;
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
    public Dictionary<Direction, int> GetAffectedMap(int cell, int depth)
    {
        Dictionary<Direction, int> affectedMap = new Dictionary<Direction, int>();
        affectedMap.Add(Direction.Left, GetDamageDepth(cell, Direction.Left, depth));
        affectedMap.Add(Direction.Right, GetDamageDepth(cell, Direction.Right, depth));
        affectedMap.Add(Direction.Up, GetDamageDepth(cell, Direction.Up, depth));
        affectedMap.Add(Direction.Down, GetDamageDepth(cell, Direction.Down, depth));
        return affectedMap;
    }
    int GetDamageDepth(int cell,Direction dir, int distance)
    {
        int depth = 0;
        int next_cell = GetNextCell(cell, dir);
        for (int i = 0; i < distance; i++)
        {
            if (next_cell == -1)
                break;
            bool can_count;
            if (!LevelData[next_cell].CanTakeDamage(out can_count))
            {
                if (can_count) depth = i + 1;
                break;
            }
            depth= i + 1;
            next_cell = GetNextCell(next_cell, dir);
        }
        return depth;
    }
    
    int GetNextCell(int cell, Direction dir)
    {
        switch(dir)
        {
            case Direction.Left:
                {
                    if (cell - Col < 0) return -1;
                    return cell - Col;
                }
            case Direction.Right:
                {
                    if (cell + Col >= Row * Col) return -1;
                    return cell + Col;
                }
            case Direction.Up:
                {
                    if (cell + 1 >= Row * Col) return -1;
                    return cell + 1;
                }
            case Direction.Down:
                {
                    if (cell - 1 < 0) return -1;
                    return cell - 1;
                }
        }
        return -1;
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
    public bool CanTakeDamage(out bool count_this)
    {
        count_this = false;
        if (type == GameModel.BlockType.NonBreakable)
            return false;
        else if (type == GameModel.BlockType.Gate)
            return true;
        else if (type == GameModel.BlockType.Breakable)
        {
            if (subType == GameModel.SubType.Empty)
                return true;
            else
            {
                count_this = true;
                return false;
            }
        }
        return false;
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
        Utils.Row = Random.Range(6, 6);
        Utils.Col = Random.Range(6, 6);
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
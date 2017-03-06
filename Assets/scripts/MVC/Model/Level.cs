using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        else st = GameModel.SubType.RemoteDetonator;
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
    public Dictionary<Direction, int> GetAffectedMap(Player p, ref List<Block> blockList)
    {
        int cell = p.CurrentCell;
        int depth = p.GetBombRange();
        Dictionary<Direction, int> affectedMap = new Dictionary<Direction, int>();
        if (!blockList.Contains(LevelData[cell]))
            blockList.Add(LevelData[cell]);
        affectedMap.Add(Direction.Left, GetDamageDepth(cell, Direction.Left, depth, ref blockList));
        affectedMap.Add(Direction.Right, GetDamageDepth(cell, Direction.Right, depth, ref blockList));
        affectedMap.Add(Direction.Up, GetDamageDepth(cell, Direction.Up, depth, ref blockList));
        affectedMap.Add(Direction.Down, GetDamageDepth(cell, Direction.Down, depth, ref blockList));
        return affectedMap;
    }
    int GetDamageDepth(int cell, Direction dir, int distance, ref List<Block> blockList)
    {
        int depth = 0;
        int next_cell = GetNextCell(cell, dir);
        for (int i = 0; i < distance; i++)
        {
            if (next_cell == -1)
                break;
            bool can_count;
            if (!LevelData[next_cell].CanTakeDamage(out can_count, ref blockList))
            {
                if (can_count) depth = i + 1;
                break;
            }
            depth = i + 1;
            next_cell = GetNextCell(next_cell, dir);
        }
        return depth;
    }

    int GetNextCell(int cell, Direction dir)
    {
        switch (dir)
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
    public int GetNextCellForAIToNavigate(int cell, ref Direction dir)
    {
        int next_cell = GetNextCell(cell, dir);
        if (next_cell != -1 && LevelData[next_cell].IsNavigable())
            return next_cell;
        int original_dir = (int)dir;
        for (int i = 0; i < 4; i++)
        {
            dir = (Direction)((i + original_dir) % 4);
            next_cell = GetNextCell(cell, dir);
            if (next_cell != -1 && LevelData[next_cell].IsNavigable())
                return next_cell;
        }
        return next_cell;
    }
}
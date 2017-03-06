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
    public Dictionary<Direction, int> GetAffectedMap(Player p, ref List<Block> blockList)
    {
        int cell = p.CurrentCell;
        int depth = p.GetBombRange(); 
        Dictionary<Direction, int> affectedMap = new Dictionary<Direction, int>();
        blockList.Add(LevelData[cell]);
        affectedMap.Add(Direction.Left, GetDamageDepth(cell, Direction.Left, depth, ref blockList));
        affectedMap.Add(Direction.Right, GetDamageDepth(cell, Direction.Right, depth, ref blockList));
        affectedMap.Add(Direction.Up, GetDamageDepth(cell, Direction.Up, depth, ref blockList));
        affectedMap.Add(Direction.Down, GetDamageDepth(cell, Direction.Down, depth, ref blockList));
        return affectedMap;
    }
    int GetDamageDepth(int cell,Direction dir, int distance, ref List<Block> blockList)
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
    public int GetNextCellForAIToNavigate(int cell, ref Direction dir)
    {
        int next_cell = GetNextCell(cell, dir);
        if (next_cell != -1 && LevelData[next_cell].IsNavigable() )
            return next_cell;
        int original_dir = (int)dir;
        for(int i = 0; i < 4; i++)
        {
            dir = (Direction) ((i + original_dir) % 4);
            next_cell = GetNextCell(cell, dir);
            if (next_cell != -1 && LevelData[next_cell].IsNavigable())
                return next_cell;
        }
        return next_cell;
    }
}
public class Block
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
    public void MakeitEmpty()
    {
        subType = GameModel.SubType.Empty;
    }
    public bool CanTakeDamage(out bool count_this, ref List<Block> blockList)
    {
        count_this = false;
        if (type == GameModel.BlockType.NonBreakable)
            return false;
        else if (type == GameModel.BlockType.Gate)
            return true;
        else if (type == GameModel.BlockType.Breakable)
        {
            blockList.Add(this);
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
    public bool IsNavigable()
    {
        if (type == GameModel.BlockType.Breakable)
        {
            if (subType == GameModel.SubType.Empty)
                return true;
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
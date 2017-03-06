using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        else if (type == GameModel.BlockType.Breakable)
        {
            if (!blockList.Contains(this))
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

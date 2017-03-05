using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class Player {
    // Use this for initialization
    bool CanRemoteDetonate = false;
    bool RemotePower = false;
    int BombCount = 2;
    int MaxBombCount = 3;
    int BombRange = 1;
    float RunSpeed = 1.0f;
    public int CurrentCell = 0;
    int Id;
    public void Init(int id, int cell)
    {
        Id = id;
        CurrentCell = cell;
        Debug.Log("Player " + Id + " Spawned at " + cell);
    }

    public void EnableRemoteDetonate()
    {
        RemotePower = true;
    }
    public bool RemoteDetonate ()
    {
        CanRemoteDetonate = !CanRemoteDetonate;
        return !CanRemoteDetonate;
    }
    public bool GetRemoteDetonate()
    {
        return RemotePower;
    }
    public void IncBombRange()
    {
        BombRange++;
    }
    public int GetBombRange()
    {
        return BombRange;
    }
    public void ResetRunSpeed()
    {
        RunSpeed = 1.0f;
    }
    public void IncRunSpeed()
    {
        RunSpeed += 1.0f;
    }
    public float GetRunSpeed()
    {
        return RunSpeed;
    }
    public int GetBomb()
    {
        BombCount--;
        return BombCount;
    } 
    public void ReplenishBombCount()
    {
        if (BombCount < 0)
            BombCount = 0;
        BombCount++;
        if (BombCount > MaxBombCount)
            BombCount = MaxBombCount;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class Player {
    // Use this for initialization
    bool CanRemoteDetonate = false;
    bool RemotePower = false;
    int BombCount = 1;
    int MaxBombCount = 3;
    int BombRange = 1;
    float RunSpeed = 1.0f;
    public int CurrentCell = 0;
    int Id;
    bool bAlive = true;
    int Score;
    public void Init(int id, int cell)
    {
        Id = id;
        CurrentCell = cell;
        bAlive = true;
        Score = 0;
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
    public int GetBombCount()
    {
        return BombCount;
    }
    public int AcquireBomb()
    {
        BombCount--;
        return BombCount;
    } 
    public void IncBombCount()
    {
        BombCount++;
        if (BombCount > MaxBombCount)
            BombCount = MaxBombCount;
    }
    public void ReplenishBombCount()
    {
        if (BombCount < 0)
            BombCount = 0;
        BombCount++;
        if (BombCount > MaxBombCount)
            BombCount = MaxBombCount;
    }

    public void Die()
    {
        bAlive = false;
    }

    public bool IsAlive() { return bAlive; }
    public void AddScore(int score) { Score += score; }
    public int GetScore() { return Score; }
}

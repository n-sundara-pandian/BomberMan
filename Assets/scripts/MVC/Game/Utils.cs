using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {
    public static int Row = -1;
    public static int Col = -1;
    public static int LevelFinishTime = 100;
    public static int AICount = 5;
    public static int AIKillPoints = 200;
    public static int PickupPoints = 50;
    public static int BlockBreakPoints = 100;
    public static int GetIDFromRC(int r, int c)
    {
        return r * Col + c;
    }
    public static void GetRCFromID(int id, out int r, out int c)
    {
        r = id / Col;
        c = id % Col;
    }

    public static int ceilOrFloor(float v)
    {
        if (v < 0) return 0;
        int intv = (int)v;
        float diff = v - intv;
        if (diff > 0.5f) return Mathf.CeilToInt(v);
        else return Mathf.FloorToInt(v);
    }
}

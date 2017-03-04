using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {
    public static int Row = -1;
    public static int Col = -1;
    public static int GetIDFromRC(int r, int c)
    {
        return r * Col + c;
    }
    public static void GetRCFromID(int id, out int r, out int c)
    {
        r = id / Col;
        c = id % Col;
    }


}

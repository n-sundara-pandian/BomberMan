﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class Player {
    // Use this for initialization
    bool CanDetonateRemotely = false;
    float RemoteTimer = 0.0f;
    int BombAmount = 1;
    int BombRange = 1;
    float RunSpeed = 1.0f; 
    public Player()
    {

    }
}
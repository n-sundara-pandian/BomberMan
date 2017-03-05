using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCountPickup : Pickup
{
    public override void Action(Player p)
    {
        p.IncBombCount();
        p.AddScore(Utils.PickupPoints);
    }
}


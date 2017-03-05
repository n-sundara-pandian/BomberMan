using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombRangePickup : Pickup
{
    public override void Action(Player p)
    {
        p.IncBombRange();
        p.AddScore(Utils.PickupPoints);
    }

}

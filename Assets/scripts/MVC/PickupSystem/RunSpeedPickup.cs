using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSpeedPickup : Pickup
{
    public override void Action(Player p)
    {
        p.IncRunSpeed();
        p.AddScore(Utils.PickupPoints);
    }
}
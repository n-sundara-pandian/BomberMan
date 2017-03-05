using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnableRemotePickup : Pickup
{
    public override void Action(Player p)
    {
        p.EnableRemoteDetonate();
        p.AddScore(Utils.PickupPoints);
    }
}
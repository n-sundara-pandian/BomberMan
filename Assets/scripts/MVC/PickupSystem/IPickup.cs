using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using thelab.mvc;

public interface IPickup {
    void Action(Player p);
}

public class Pickup : View<Game>, IPickup
{
    public virtual void Action(Player p){}
    protected void OnTriggerEnter(Collider collider)
    {
        Player p = app.model.OnCharacterTrigger(collider.name);
        if (p != null) // pickup successfully picked up
        {
            Action(p);
            Destroy(gameObject, 0.1f);
        }
    }
}




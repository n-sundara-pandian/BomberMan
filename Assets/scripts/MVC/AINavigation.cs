using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using thelab.mvc;

public class AINavigation : Controller<Game>
{
    private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Vector3 m_Move;
    private Player Owner;
    Level.Direction CurrentDirection = Level.Direction.Right;

    public void Init(Player p)
    {
        Owner = p;
        int id = p.CurrentCell;
        int r, c;
        Utils.GetRCFromID(id, out r, out c);
        m_Character = GetComponent<ThirdPersonCharacter>();
        m_Character.transform.position = new Vector3(r, 0.25f, c);
        m_Character.SetMoveSpeedMultiplier(0.25f);
    }
    private void FixedUpdate()
    {
        if (!Owner.IsAlive())
            return;
        int next_cell = app.model.GetLevel().GetNextCellForAIToNavigate(Owner.CurrentCell, ref CurrentDirection);
        if (next_cell == -1)
            return;
        // read inputs
        float h = 0; //CrossPlatformInputManager.GetAxis(Horizontal);
        float v = 0;// CrossPlatformInputManager.GetAxis(Vertical);
        // we use world-relative directions in the case of no main camera
        switch(CurrentDirection)
        {
            case Level.Direction.Left:
                {
                    h = -1;
                    break;
                }
            case Level.Direction.Right:
                {
                    h = 1;
                    break;
                }
            case Level.Direction.Up:
                {
                    v = 1;
                    break;
                }
            case Level.Direction.Down:
                {
                    v = -1;
                    break;
                }
        }
        m_Move = v * Vector3.forward + h * Vector3.right;
        // pass all parameters to the character control script
        m_Character.Move(m_Move);
        int x = Utils.ceilOrFloor(m_Character.transform.position.x);
        int z = Utils.ceilOrFloor(m_Character.transform.position.z);
        Owner.CurrentCell = Utils.GetIDFromRC(x, z);
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using thelab.mvc;
public class ThirdPersonUserControl : Controller<Game>
{
    private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Vector3 m_Move;
    string Horizontal;
    string Vertical;
    string Action;
    bool Inited = false;
    public GameObject BombPrefab;
    int Id;
    List<Bomb> ActiveBombList = new List<Bomb>();
    private void Start()
    {
        m_Character = GetComponent<ThirdPersonCharacter>();
    }

    public void Init(Dictionary<string, string> param_list)
    {
        Horizontal = param_list["Horizontal"];
        Vertical = param_list["Vertical"];
        Action = param_list["Action"];
        Id = int.Parse(param_list["Id"]);
        Inited = true;
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (!Inited)
            return;
        // read inputs
        float h = CrossPlatformInputManager.GetAxis(Horizontal);
        float v = CrossPlatformInputManager.GetAxis(Vertical);
        // we use world-relative directions in the case of no main camera
        m_Move = v*Vector3.forward + h*Vector3.right;           
        // pass all parameters to the character control script
        m_Character.Move(m_Move);
    }
    int ceilOrFloor(float v)
    {
        if (v < 0) return 0;
        int intv = (int)v;
        float diff = v - intv;
        if (diff > 0.5f) return Mathf.CeilToInt(v);
        else return Mathf.FloorToInt(v);
    }
    void Update()
    {
        if(Input.GetKeyUp(Action))
        {
            Player p = app.model.GetPlayer(Id);
            int x = ceilOrFloor(m_Character.transform.position.x);
            int z = ceilOrFloor(m_Character.transform.position.z);
            int cell = Utils.GetIDFromRC(x, z);
            int r, c;
            Utils.GetRCFromID(cell, out r, out c);
            List<Block> affectedBlockList = new List<Block>();
          //  p.EnableRemoteDetonate();
            if (p.GetRemoteDetonate())
            {
                if (p.RemoteDetonate())
                {
                    foreach (Bomb b in ActiveBombList)
                        b.Explode();
                    ActiveBombList.Clear();
                }
                else
                {
                    Bomb b = MakeBomb(r, c);
                    b.Init(3.0f, app.model.GetLevel().GetAffectedMap(cell, 3, ref affectedBlockList), p.GetRemoteDetonate(), affectedBlockList);
                    ActiveBombList.Add(b);
                }

            }
            else
            {
                int bomb_count = p.GetBomb();
                if (bomb_count >= 0)
                {
                    MakeBomb(r, c).Init(3.0f, app.model.GetLevel().GetAffectedMap(cell, 3, ref affectedBlockList), p.GetRemoteDetonate(), affectedBlockList);
                }
            }
        }
    }

    Bomb MakeBomb(int r, int c)
    {
        Vector3 bombPos = new Vector3(r, 0.5f, c);
        GameObject obj = Instantiate<GameObject>(BombPrefab, bombPos, Quaternion.identity);
        Bomb bomb = obj.GetComponent<Bomb>();
        bomb.Replenish += ReplenishPlayer;
        bomb.DestroyBlocks += DestroyBlocks;
        return bomb;
    }
    void ReplenishPlayer()
    {
        Player p = app.model.GetPlayer(Id);
        p.ReplenishBombCount();
    }
    void DestroyBlocks(List<Block> DestroyList)
    {
        app.model.DestroyBlocks(DestroyList);
        app.view.DestroyBlocks(DestroyList);
    }

}

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
    Player Owner;
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
        Owner = app.model.GetPlayer(Id);

        Renderer[] renderer_list = gameObject.GetComponentsInChildren<Renderer>();
        for(int i = 0; i < renderer_list.Length; i++)
            renderer_list[i].material.SetColor("_Color", (Id == 1)?Color.red:Color.green);
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (!Inited)
            return;
        if (app.model.GetGameOver())
            return;
        // read inputs
        float h = CrossPlatformInputManager.GetAxis(Horizontal);
        float v = CrossPlatformInputManager.GetAxis(Vertical);
        // we use world-relative directions in the case of no main camera
        m_Move = v*Vector3.forward + h*Vector3.right;           
        // pass all parameters to the character control script
        m_Character.Move(m_Move);
        int x = Utils.ceilOrFloor(m_Character.transform.position.x);
        int z = Utils.ceilOrFloor(m_Character.transform.position.z);
        Owner.CurrentCell = Utils.GetIDFromRC(x, z);
    }

    void Update()
    {
        if (app.model.GetGameOver())
            return;
        if (String.IsNullOrEmpty(Action))
            return;
        if(Input.GetKeyUp(Action))
        {
            int r, c;
            Utils.GetRCFromID(Owner.CurrentCell, out r, out c);
            List<Block> affectedBlockList = new List<Block>();
            if (Owner.GetRemoteDetonate())
            {
                if (Owner.RemoteDetonate())
                {
                    foreach (Bomb b in ActiveBombList)
                        b.Explode();
                    ActiveBombList.Clear();
                }
                else
                {
                    Bomb b = MakeBomb(r, c);
                    b.Init(3.0f, app.model.GetLevel().GetAffectedMap(Owner, ref affectedBlockList), Owner.GetRemoteDetonate(), affectedBlockList);
                    ActiveBombList.Add(b);
                }

            }
            else
            {
                int bomb_count = Owner.AcquireBomb();
                if (bomb_count >= 0)
                {
                    MakeBomb(r, c).Init(3.0f, app.model.GetLevel().GetAffectedMap(Owner, ref affectedBlockList), Owner.GetRemoteDetonate(), affectedBlockList);
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
        Owner.ReplenishBombCount();
        app.view.PlayOneShot("bomb");
    }
    void DestroyBlocks(List<Block> DestroyList)
    {
        app.view.DestroyBlocks(DestroyList, Owner);
        app.model.DestroyBlocks(DestroyList, Owner);
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("AiBot"))
            app.model.GetPlayer(Id).Die();
    }
}

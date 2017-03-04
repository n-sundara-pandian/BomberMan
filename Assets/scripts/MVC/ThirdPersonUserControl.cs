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
            //int x = (int)Mathf.Clamp(m_Character.transform.position.x, Mathf.FloorToInt(m_Character.transform.position.x), Mathf.CeilToInt(m_Character.transform.position.x));
            //int z = (int)Mathf.Clamp(m_Character.transform.position.z, Mathf.FloorToInt(m_Character.transform.position.z), Mathf.CeilToInt(m_Character.transform.position.z));
            int x = ceilOrFloor(m_Character.transform.position.x);
            int z = ceilOrFloor(m_Character.transform.position.z);
            int cell = Utils.GetIDFromRC(x, z);
            int r, c;
            Utils.GetRCFromID(cell, out r, out c);
            Debug.Log("Cell " + cell + " r " + r + " c " + c);
            Vector3 bombPos = new Vector3(r, 0.5f, c); //m_Character.transform.position
            GameObject obj = Instantiate<GameObject>(BombPrefab, bombPos, Quaternion.identity);
            Bomb bomb = obj.GetComponent<Bomb>();
            bomb.Init(3.0f);
        }
    }
}

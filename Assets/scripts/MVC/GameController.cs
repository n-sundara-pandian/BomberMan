using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameController : Controller<Game>
{
    public delegate void ReplenishBomb(Player p);
    public static event ReplenishBomb Replenish;
    void Start()
    {
        app.model.Init();
        Invoke("StartLater", 0.25f);
    }

    void StartLater()
    {
        app.view.GenerateWorld();
        app.view.SpawnPlayers();
    }

    public void Replay()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    public void OnReplenishBomb(Player p)
    {
        Debug.Log(" Bomb Replenished for plare " + p.CurrentCell);
 
    }
}
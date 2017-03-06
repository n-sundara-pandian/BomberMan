using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameController : Controller<Game>
{
    int ElapsedTime;
    void Start()
    {
        app.model.Init();
        app.view.Init();
        Invoke("StartLater", 0.25f);
        InvokeRepeating("UpdateTimer", 0.01f, 1.0f);
    }

    void StartLater()
    {
        app.view.GenerateWorld();
        app.view.SpawnPlayers();
        app.view.SpawnAI();
    }
    void UpdateTimer()
    {
        ElapsedTime++;
        int time_remaining = Utils.LevelFinishTime - ElapsedTime;
        if (time_remaining < 0)
        {
            OnGameOver();
            return;
        }
        else if (app.model.GetGameOver())
        {
            OnGameOver();
            return;
        }
        app.view.UpdateHudData("LevelTimer", time_remaining.ToString());
    }
    public void OnGameOver()
    {
        string result = "Match Draw";
        bool player1_alive = app.model.GetPlayer(1).IsAlive();
        bool player2_alive = app.model.GetPlayer(2).IsAlive();
        if (!player1_alive && !player2_alive) result = " Match Tie";
        else if (player1_alive && !player2_alive) result = " Player 1 Wins";
        else if (player2_alive && !player1_alive) result = " Player 2 Wins";
        app.model.SetGameOver(true);
        app.view.ShowGameOverScreen(result);
        PlayerPrefs.SetInt("P1", app.model.GetPlayer(1).GetScore());
        PlayerPrefs.SetInt("P2", app.model.GetPlayer(2).GetScore());
    }
    public void Replay()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
 
}
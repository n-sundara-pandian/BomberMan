﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameController : Controller<Game>
{
    void Start()
    {
        app.model.Init();
        app.view.Init();
        Invoke("StartLater", 0.25f);
    }

    void StartLater()
    {
        app.view.GenerateWorld();
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
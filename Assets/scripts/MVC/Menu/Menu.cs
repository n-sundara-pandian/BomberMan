using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Text Player1PrevScore;
    public Text Player2PrevScore;
    // Use this for initialization
    void Start () {
        Player1PrevScore.text = PlayerPrefs.GetInt("P1", 0).ToString();
        Player2PrevScore.text = PlayerPrefs.GetInt("P2", 0).ToString();
    }
	
	// Update is called once per frame
	public void OnPlayClick () {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void OnExitClick()
    {
        Application.Quit();
    }
}

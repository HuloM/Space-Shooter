using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && _isGameOver)
            SceneManager.LoadScene(0);
        
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


    public void GameOver()
    {
        _isGameOver = true;
    }
    
}

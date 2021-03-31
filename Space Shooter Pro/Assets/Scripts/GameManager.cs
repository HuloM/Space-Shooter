using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOverLoss;
    private bool _isGameOverWon;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && _isGameOverLoss)
            SceneManager.LoadScene(0);
        
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


    public void GameOver(int gameWon)
    {
        if(gameWon == 0)
            _isGameOverLoss = true;
        else if (gameWon == 1)
            _isGameOverWon = true;
    }

}

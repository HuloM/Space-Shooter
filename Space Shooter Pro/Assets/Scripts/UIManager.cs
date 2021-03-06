using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //handle for text
    [SerializeField] private Text _scoreText;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _ammoCountText;
    [SerializeField] private Slider _thrusterFuelSlider;

    private GameManager _gameManager;
    private void Start()
    {
        if (_scoreText == null)
            Debug.LogError("no score text assigned");
        else
            _scoreText.text = "Score: " + 0;

        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        
        if(_gameManager == null)
            Debug.LogError("game manager not found");
        
        if(_ammoCountText == null)
            Debug.LogError("no ammo count text assigned");
        
        if(_thrusterFuelSlider == null)
            Debug.LogError("no fuel slider assigned");
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives)
    {
        if (_livesImage != null && currentLives < 3)
            _livesImage.sprite = _liveSprites[currentLives];
    }

    public void UpdateGameOverLoss()
    {
        GameOverSequence(0, "YOU LOSE");
    } 
    public void UpdateGameOverWon()
    {
        GameOverSequence(1, "YOU WIN!!!");
    }
    private void GameOverSequence(int gameWonNum, string gameWonString)
    {
        _gameOverText.gameObject.SetActive(true);
        _gameManager.GameOver(gameWonNum);
        StartCoroutine(GameOverFlickerRoutine(gameWonString));
    }

    IEnumerator GameOverFlickerRoutine(String gameWonString)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(1f);
            _gameOverText.text = "GAME OVER: " + gameWonString;
        }
    }

    public void UpdatePlayerAmmo(int ammo, int maxAmmo)
    {
        _ammoCountText.text = "Ammo : " + ammo + "/" + maxAmmo;
    }

    public void updateThrusterFuel(float fuel)
    {
        _thrusterFuelSlider.value = fuel;
    }
}

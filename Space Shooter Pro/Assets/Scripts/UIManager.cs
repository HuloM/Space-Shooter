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

    private GameManager _gameManager;
    private void Start()
    {
        if (_scoreText == null)
            Debug.Log("no text component assigned");
        else
            _scoreText.text = "Score: " + 0;

        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        
        if(_gameManager == null)
            Debug.LogError("game manager not found");
    }

    public void updateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void updateLives(int currentLives)
    {
        if (_livesImage != null)
            _livesImage.sprite = _liveSprites[currentLives];
    }

    public void updateGameOver()
    {
        GameOverSequence();
    }

    private void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(1f);
            _gameOverText.text = "GAME OVER";
        }
    }
}

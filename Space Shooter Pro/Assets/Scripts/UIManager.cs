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
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives)
    {
        if (_livesImage != null)
            _livesImage.sprite = _liveSprites[currentLives];
    }

    public void UpdateGameOver()
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

    public void UpdatePlayerAmmo(int ammo)
    {
        _ammoCountText.text = "Ammo : " + ammo;
    }
}

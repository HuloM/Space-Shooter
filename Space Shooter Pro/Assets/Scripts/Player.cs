using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private GameObject _shieldVisual;
    [SerializeField] private int _score = 0;
    [SerializeField] private GameObject[] _EngineDamage;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _laserShotClip;

    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private float _canFire = -1.0f;
    private float _speedMultiplier = 2f;
    private bool _isTripleShotEnabled = false;
    private bool _isShieldEnabled = false;
    private UIManager _uiManager;

    public int Score => _score;

    private void Start()
    {
        transform.position = Vector3.zero;
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = _laserShotClip;
        
        if(_spawnManager == null)
            Debug.LogError("Spawn Manager not found");
        if(_uiManager == null)
            Debug.LogError("UI Manager not found");
    }

    private void Update()
    {
        CalculateMovement();
        
        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
    }

    private void EnableShieldVisual()
    {
        _shieldVisual.SetActive(_isShieldEnabled);
    }

    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput);
        transform.Translate(movement * (_speed * Time.deltaTime));

        transform.position = new Vector3(transform.position.x, 
            Mathf.Clamp(transform.position.y, -3.5f, 0.5f));

        if (transform.position.x <= -12 || transform.position.x >= 12)
            transform.position = new Vector3(transform.position.x * -1, transform.position.y, 0);
    }
    private void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        
        if (_isTripleShotEnabled)
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        else
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        
        _audioSource.Play();
    }

    public void Damage()
    {
        //if shield is active do nothing
        //return
        if (_isShieldEnabled)
        {
            _isShieldEnabled = false;
            EnableShieldVisual();
            return;
        }
        
        _lives -= 1;
        _uiManager.updateLives(_lives);

        switch (_lives)
        {
            case 0:
                Instantiate(_explosionPrefab, gameObject.transform.position, Quaternion.identity);
                _spawnManager.OnPlayerDeath();
                _uiManager.updateGameOver();
                Destroy(gameObject, 1f);
                break;
            case 1:
                _EngineDamage[1].SetActive(true);
                break;
            case 2: 
                _EngineDamage[0].SetActive(true);
                break;
            default: 
                Debug.LogError("invalid value reached for player lives");
                break;
        }
    }

    public void OnTripleShotPickup()
    {
        _isTripleShotEnabled = true;
        StartCoroutine(PowerupPowerDownRoutine(PowerupID.TripleShot));
    }

    public void OnSpeedPickup()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(PowerupPowerDownRoutine(PowerupID.Speed));
    }

    public void OnShieldPickup()
    {
        _isShieldEnabled = true;
        EnableShieldVisual();
        StartCoroutine(PowerupPowerDownRoutine(PowerupID.Shield));
    }

    IEnumerator PowerupPowerDownRoutine(PowerupID id)
    {
        yield return new WaitForSeconds(4f);
        switch (id)
        {
            case PowerupID.TripleShot:
                _isTripleShotEnabled = false;
                break;
            case PowerupID.Speed:
                _speed /= _speedMultiplier;
                break;
            case PowerupID.Shield:
                _isShieldEnabled = false;
                EnableShieldVisual();
                break;
            default:
                Debug.Log("invalid ID");
                break;
        }
    }

    public void EnemyHit(int points)
    {
        _score += points;
        _uiManager.updateScore(_score);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);

        if (other.CompareTag("EnemyLaser"))
        {
            Damage();
            Destroy(other.gameObject);
        }
    }
    
}

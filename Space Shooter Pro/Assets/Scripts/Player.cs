using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    //inspector variables
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _multiShotPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private GameObject _shieldVisual;
    [SerializeField] private int _score = 0;
    [SerializeField] private GameObject[] _EngineDamage;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _laserShotClip;
    
    //private variables
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private SpriteRenderer _shieldSpriteRenderer;
    private float _canFire = -1.0f;
    private float _speedMultiplier = 2f;
    private float _thrusterFuel;
    private float _maxThrusterFuel;
    private bool _isTripleShotEnabled = false;
    private bool _isMultiShotEnabled = false;
    private int _shieldStrength;
    private int _ammoCount;
    private int _maxAmmoCount;
    private Vector3 _cameraInitialPosition;
    private float _shakeMagnitude = 0.05f, _shakeTime = 0.5f;
    private Camera _mainCamera;

    

    private void Start()
    {
        transform.position = Vector3.zero;
        
        _shieldSpriteRenderer = _shieldVisual.GetComponent<SpriteRenderer>();
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _mainCamera = Camera.main;

        _audioSource.clip = _laserShotClip;
        _maxAmmoCount = _ammoCount = 15;
        _maxThrusterFuel = _thrusterFuel = 15;

        if(_mainCamera == null)
            Debug.Log("no main camera found");
        else
            _cameraInitialPosition = _mainCamera.transform.position;

        if(_spawnManager == null)
            Debug.LogError("Spawn Manager not found");
        if(_uiManager == null)
            Debug.LogError("UI Manager not found");
        else
            _uiManager.UpdatePlayerAmmo(_ammoCount, _maxAmmoCount);
    }

    private void Update()
    {
        CalculateMovement(Input.GetKey(KeyCode.LeftShift) && _thrusterFuel > 0? 2 : 1);

        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
            FireLaser();

        Refuel();

    }

    //private methods
    private void CalculateMovement(int speedMultiplier)
    {
        if (speedMultiplier > 1)
            _thrusterFuel -= 2f * Time.deltaTime;

        _uiManager.updateThrusterFuel(_thrusterFuel);
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput);
        transform.Translate(movement * (_speed * Time.deltaTime * speedMultiplier));

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
        else if (_isMultiShotEnabled)
            Instantiate(_multiShotPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        else
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        
        _uiManager.UpdatePlayerAmmo(--_ammoCount, _maxAmmoCount);
        _audioSource.Play();
    }
    private void UpdatePlayerHealthVisual()
    {
        switch (_lives)
        {
            case 0:
                Instantiate(_explosionPrefab, gameObject.transform.position, Quaternion.identity);
                _spawnManager.OnPlayerDeath();
                _uiManager.UpdateGameOver();
                Destroy(gameObject);
                break;
            case 1:
                _EngineDamage[1].SetActive(true);
                break;
            case 2:
                _EngineDamage[1].SetActive(false);
                _EngineDamage[0].SetActive(true);
                break;
            case 3:
                _EngineDamage[0].SetActive(false);
                break;
            default:
                Debug.LogError("invalid value reached for player lives");
                break;
        }
    }
    private void UpdateShieldVisual(int shieldStrength)
    {
        switch (shieldStrength)
        {
            case 0:
                _shieldVisual.SetActive(false);
                break;
            case 1: 
                _shieldSpriteRenderer.color = Color.red;
                break;
            case 2:
                _shieldSpriteRenderer.color = Color.magenta;
                break;
            case 3:
                _shieldSpriteRenderer.color = Color.white;
                break;
            default:
                Debug.Log("Invalid Shield Value");
                break;
        }
        
    }
    private void PowerupToPowerDown(PowerupID id)
    {
        switch (id)
        {
            case PowerupID.TripleShot:
                _isTripleShotEnabled = false;
                break;
            case PowerupID.MultiShot:
                _isMultiShotEnabled = false;
                break;
            case PowerupID.Speed:
                _speed /= _speedMultiplier;
                break;
            case PowerupID.Shield:
                _shieldStrength = 0;
                UpdateShieldVisual(_shieldStrength);
                break;
            default:
                Debug.Log("invalid ID");
                break;
        }
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
    private IEnumerator PowerupPowerDownRoutine(PowerupID id, float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        PowerupToPowerDown(id);
    }
    private void Refuel()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && _thrusterFuel < _maxThrusterFuel)
            _thrusterFuel += 1f * Time.deltaTime;
    }
    private void ShakeCamera()
    {
        InvokeRepeating(nameof(StartShakeCamera), 0f, 0.005f);
        Invoke(nameof(StopShakeCamera), _shakeTime);
    }
    private void StartShakeCamera()
    {
        var transformPosition = _mainCamera.transform.position;
        transformPosition.x += Random.value * _shakeMagnitude * 2 - _shakeMagnitude;
        transformPosition.y += Random.value * _shakeMagnitude * 2 - _shakeMagnitude;
        _mainCamera.transform.position = transformPosition;
    }
    private void StopShakeCamera()
    {
        CancelInvoke("StartShakeCamera");
        _mainCamera.transform.position = _cameraInitialPosition;
    }

    //public methods
    public void EnemyHit(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
    public void Damage()
    {
        if (_shieldStrength > 0)
        {
            UpdateShieldVisual(--_shieldStrength);
            return;
        }
        
        ShakeCamera();
        _lives -= 1;
        _uiManager.UpdateLives(_lives);

        UpdatePlayerHealthVisual();
    }
    public void OnTripleShotPickup()
    {
        _isTripleShotEnabled = true;
        StartCoroutine(PowerupPowerDownRoutine(PowerupID.TripleShot, 4f));
    }
    public void OnSpeedPickup()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(PowerupPowerDownRoutine(PowerupID.Speed, 4f));
    }
    public void OnShieldPickup()
    {
        _shieldStrength = 3;
        _shieldVisual.SetActive(true);
        UpdateShieldVisual(_shieldStrength);
        //StartCoroutine(PowerupPowerDownRoutine(PowerupID.Shield));
    }
    public void OnAmmoRefillPickup()
    {
        _ammoCount = 15;
        _uiManager.UpdatePlayerAmmo(_ammoCount, _maxAmmoCount);
    }
    public void OnHealPickup()
    {
        if (_lives < 3)
        {
            ++_lives;
            _uiManager.UpdateLives(_lives);
            UpdatePlayerHealthVisual();
        }
        else
            Debug.Log("max lives reached");
    }
    public void OnMultiShotPickup()
    {
        _isMultiShotEnabled = true;
        StartCoroutine(PowerupPowerDownRoutine(PowerupID.MultiShot, 5f));
    }
    
    
    

}

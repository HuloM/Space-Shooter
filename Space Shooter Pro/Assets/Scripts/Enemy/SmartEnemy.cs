using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmartEnemy : MonoBehaviour
{

    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private GameObject _enemySmartLaserPrefab;
    [SerializeField] private GameObject _enemyShield;

    private const int SpawnShieldChance = 33;
    
    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private float _canFire = -1;
    private bool _isHit;
    private bool _movingRight;
    private bool _hasShield;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        if(_player == null)
            Debug.LogError("player not found");

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        RandomSpawnShield();
    }

    private void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && !_isHit)
        {
            if(_player.gameObject.transform.position.y > transform.position.y)
                FireLaser(_enemySmartLaserPrefab);
            else
                FireLaser(_enemyLaserPrefab);
        }
    }

    private void AvoidLaser()
    {
    }
    
    private void FireLaser(GameObject laserPrefab)
    {
        _canFire = Time.time + Random.Range(3f, 7f);
        Instantiate(laserPrefab, transform.position, Quaternion.identity);
    }
    
    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        KeepInBorder();
    }

    private void KeepInBorder()
    {
        if (transform.position.y < -6.0f)
            transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 7.0f);
        if (transform.position.x < -9.0f)
            transform.position = new Vector3(-9.0f, transform.position.y);
        else if (transform.position.x > 9.0f)
            transform.position = new Vector3(9.0f, transform.position.y);
    }
    private void RandomSpawnShield()
    {
        var randShieldSpawn = Random.Range(0, 100);

        if (randShieldSpawn <= SpawnShieldChance)
        {
            _hasShield = true;
            _enemyShield.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);

        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
                OnEnemyHit();
            }
        }
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            OnEnemyHit();
        }
    }

    private void OnEnemyHit()
    {
        if (_hasShield)
        {
            _hasShield = false;
            _enemyShield.SetActive(false);
            return;
        }
        
        _player.EnemyHit(Random.Range(5, 15));
        _animator.SetTrigger("OnDestroy");
        _speed = 0;
        _audioSource.Play();
        _isHit = true;
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject, 2.5f);
    }
}
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{

    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _enemyLaserPrefab;
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
            FireLaser();
        }
    }

    private void FireLaser()
    {
        _canFire = Time.time + Random.Range(3f, 7f);
        Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
    }
    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        StartCoroutine(MoveSideToSide());
        KeepInBorder();

        if (Math.Abs(transform.position.y - _player.transform.position.y) < 1.5f &&
            Math.Abs(transform.position.x - _player.transform.position.x) < 2f)
            Ram();
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

    private void Ram()
    {
        StopCoroutine(MoveSideToSide());
        var direction = _player.transform.position - transform.position;
        transform.Translate(direction * (_speed * Time.deltaTime));
        
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
            Player player = other.GetComponent<Player>();

            if (player != null)
                player.Damage();

            if (_hasShield)
            {
                _hasShield = false;
                _enemyShield.SetActive(false);
                return;
            }
            
            if(player != null)
                OnEnemyHit();
        }
        
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            if (_hasShield)
            {
                _hasShield = false;
                _enemyShield.SetActive(false);
                return;
            }
            
            OnEnemyHit();
        }
    }

    private void OnEnemyHit()
    {
        _player.EnemyHit(Random.Range(5, 15));
        _animator.SetTrigger("OnDestroy");
        _speed = 0;
        _audioSource.Play();
        _isHit = true;
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject, 2.5f);
    }
    private IEnumerator MoveSideToSide()
    {
        if (_movingRight)
        {
            transform.Translate(Vector3.right * (Time.deltaTime * _speed));
            yield return new WaitForSeconds(0.5f);
            _movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * (Time.deltaTime * _speed));
            yield return new WaitForSeconds(0.5f);
            _movingRight = true;
        }
    }
}
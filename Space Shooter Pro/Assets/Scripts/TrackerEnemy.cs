using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackerEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private GameObject _laserCagePrefab;
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
        StartCoroutine(CageSequence());
        
        RandomSpawnShield();
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < -6.0f)
            transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 7.0f);


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

            if (player != null)
                _player.EnemyHit(Random.Range(5, 15));
            _animator.SetTrigger("OnDestroy");
            _speed = 0;
            _audioSource.Play();
            _isHit = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.5f);
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
            
            if(_player != null)
                _player.EnemyHit(Random.Range(5, 15));

            
            _animator.SetTrigger("OnDestroy");
            _speed = 0;
            _audioSource.Play();
            _isHit = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.5f);
        }
    }

    IEnumerator CageSequence()
    {
        while (_speed > 0)
        {
            transform.position = new Vector3(_player.transform.position.x + Random.Range(-1f,1f), transform.position.y);
            _laserCagePrefab.SetActive(true);
            yield return new WaitForSeconds(1f);
            _laserCagePrefab.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
}

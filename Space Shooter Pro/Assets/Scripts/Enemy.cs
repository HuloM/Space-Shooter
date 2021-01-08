using System;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TreeEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{

    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _enemyLaserPrefab;
    
    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private float _canFire = -1;
    private bool _isHit;
    private bool _movingRight;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        if(_player == null)
            Debug.LogError("player not found");

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && !_isHit)
        {
            _canFire = Time.time + Random.Range(3f,7f);
            Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        StartCoroutine(MoveSideToSide());
        
        
        if (transform.position.y < -6.0f)
            transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 7.0f);
        if(transform.position.x < -9.0f)
            transform.position = new Vector3(-9.0f, transform.position.y);
        else if(transform.position.x  > 9.0f)
            transform.position = new Vector3(9.0f, transform.position.y);
            
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);
        
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            
            if(player != null)
                player.Damage();
            
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
            
            if(_player != null)
                _player.EnemyHit(Random.Range(5, 15));

            _animator.SetTrigger("OnDestroy");
            StopCoroutine(MoveSideToSide());
            _speed = 0;
            _audioSource.Play();
            _isHit = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.5f);
        }
    }
    IEnumerator MoveSideToSide()
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

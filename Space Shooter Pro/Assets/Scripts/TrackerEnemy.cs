using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackerEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private GameObject _laserCagePrefab;
    
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
        StartCoroutine(CageSequence());
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (_speed > 0)
        {
            transform.position = new Vector3(_player.transform.position.x, transform.position.y);
        }

        if (transform.position.y < -6.0f)
            transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 7.0f);


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);
        
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                _player.EnemyHit(Random.Range(5, 15));
            }

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
            _laserCagePrefab.SetActive(true);
            yield return new WaitForSeconds(1f);
            _laserCagePrefab.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
}

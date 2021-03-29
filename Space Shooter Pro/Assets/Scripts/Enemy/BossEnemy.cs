using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private GameObject _enemyShield;

    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private float _canFire = -1;
    private bool _isHit;
    private int _health;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        if(_player == null)
            Debug.LogError("player not found");

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _health = 10;
    }

    private void Update()
    {
        if(transform.position.y > 5.0f)
            CalculateMovement();

        if (Time.time > _canFire && !_isHit)
            FireLaser();
    }
    
    private void FireLaser()
    {
        _canFire = Time.time + Random.Range(3f, 7f);
        Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
    }
    private void CalculateMovement() => transform.Translate(Vector3.down * (_speed * Time.deltaTime));

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);

        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            OnEnemyHit();
        }
    }

    private void OnEnemyHit()
    {
        _health--;
        if (_health <= 0)
        {
            _player.EnemyHit(100);
            _animator.SetTrigger("OnDestroy");
            _speed = 0;
            _audioSource.Play();
            _isHit = true;
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.5f);
        }
    }
}

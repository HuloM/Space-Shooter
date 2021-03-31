using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject[] _weaponPrefabs;

    private UIManager _uiManager;
    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private float _canFire = -1;
    private bool _isHit;
    private int _health;

    private readonly int[] _weaponTable =
    {
       50,  //fire normal     Index 0
       20,  //fire explosion  Index 1
       15,  //fire homing     Index 2
       10,  //fire Max        Index 3
       5    //fire BulletHell Index 4 
    };

    private void Start()
    {
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        if(_player == null)
            Debug.LogError("player not found");
        if(_uiManager == null)
            Debug.LogError("UI Manager not found");
        
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _health = 10;
    }

    private void Update()
    {
        if(transform.position.y > 5.0f)
            CalculateMovement();

        if (Time.time > _canFire && !_isHit && transform.position.y <= 5.0f)
        {
            FireWeapons();
        }
    }
    
    private void FireWeapons()
    {
        _canFire = Time.time + Random.Range(3f, 7f);
        Vector3 posToSpawn = transform.position;
        int entityToSpawn = 0;
        int total = 0;

        foreach (var item in _weaponTable)
        {
            total += item;
        }
        var randEntity = Random.Range(0, total); 
        
        for (int i = 0; i < _weaponTable.Length; i++)
        {
            if (randEntity <= _weaponTable[i])
            {
                entityToSpawn = i;
                Debug.Log("spawning: " + _weaponPrefabs[i].name);
                break;
            }
            randEntity -= _weaponTable[i];
        }
        
        var entity = Instantiate(_weaponPrefabs[entityToSpawn], posToSpawn, Quaternion.identity);
        
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
            _uiManager.UpdateGameOverWon();
        }
    }
}

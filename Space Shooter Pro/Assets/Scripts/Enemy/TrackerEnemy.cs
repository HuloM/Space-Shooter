using System.Collections;
using UnityEngine;
public class TrackerEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private GameObject _laserCagePrefab;
    [SerializeField] private GameObject _enemyShield;
    
    private const int SpawnShieldChance = 33;
    
    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _hasShield;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        if(_player == null)
            Debug.LogError("player not found");

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        
        RandomSpawnShield();
        StartCoroutine(CageSequence());
    }

    private void Update()
    {
        CalculateMovement();
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
        else
            _hasShield = false;
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
        _laserCagePrefab.SetActive(false);
        _animator.SetTrigger("OnDestroy");
        _speed = 0;
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject, 2.5f);
    }

    private IEnumerator CageSequence()
    {
        while (_speed > 0)
        {
            var direction = new Vector3(_player.transform.position.x,transform.position.y);
            transform.position = Vector3.Lerp(transform.position, direction, 0.1f);
            yield return null;
        }
    }
}

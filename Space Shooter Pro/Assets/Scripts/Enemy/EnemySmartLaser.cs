using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmartLaser : MonoBehaviour
{
    [SerializeField] private float _speed = 2.0f;
    private GameObject _player;

    private float _timeToDestroy;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        gameObject.GetComponent<AudioSource>().Play();

        _timeToDestroy = 2.0f;
    }

    private void Update()
    {
        var step = _speed * Time.deltaTime;
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            _player.transform.rotation, step);
        
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);

        _timeToDestroy -= Time.deltaTime;
        
        if(transform.position.y < -6f || _timeToDestroy <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit: " + other.transform.name);

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
                player.Damage();
            
            Destroy(gameObject);
        }
    }
}

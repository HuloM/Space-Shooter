using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Astroid : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
    }

    private void Update()
    {
        //rotate along z-axis 
        transform.Rotate(Vector3.forward * (Time.deltaTime * _speed), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Instantiate(_explosionPrefab, gameObject.transform.position, Quaternion.identity);
            _spawnManager.StartSpawnSequence();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }    
}

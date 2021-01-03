using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    //spawn enemy every 5 seconds
    //create a coroutine of type IEnumerator -- yield events
    //while loop 

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefabs;

    private bool _stopSpawning = false;

    public void StartSpawnSequence()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }


    IEnumerator SpawnEnemyRoutine()
    { 
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
            
            var enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            
            yield return new WaitForSeconds(5.0f);
        }
    }
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
            var randPowerup = Random.Range(0, 3);
            
            var powerup = Instantiate(_powerupPrefabs[randPowerup], posToSpawn, Quaternion.identity);
            powerup.transform.parent = _enemyContainer.transform;
                
            yield return new WaitForSeconds(Random.Range(8.0f, 16.0f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
    
}

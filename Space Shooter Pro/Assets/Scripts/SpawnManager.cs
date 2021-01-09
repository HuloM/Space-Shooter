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

    [SerializeField] private EnemyWaveIndex _waveIndex;
    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerups = false;
    private int enemiesSpawned;
    
    public int enemiesKilled;
    

    public void StartSpawnSequence()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }


    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawningEnemies == false)
        {
            if (_waveIndex > EnemyWaveIndex.wave03)
                _stopSpawningEnemies = true;
            
            while (enemiesSpawned < 5 * ((int) _waveIndex + 1))
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
                var enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                enemy.transform.parent = _enemyContainer.transform;
                
                enemiesSpawned++;
                yield return new WaitForSeconds(1f);
            }

            if (enemiesKilled >= 5 * ((int) _waveIndex + 1))
            {
                enemiesKilled = 0;
                enemiesSpawned = 0;
                _waveIndex++;
            }

            yield return new WaitForSeconds(5.0f);
        }
    }
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawningPowerups == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
            var randPowerup = Random.Range(0, _powerupPrefabs.Length);
            
            var powerup = Instantiate(_powerupPrefabs[randPowerup], posToSpawn, Quaternion.identity);
            powerup.transform.parent = _enemyContainer.transform;
                
            yield return new WaitForSeconds(Random.Range(8.0f, 16.0f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerups = true;
    }
    
}

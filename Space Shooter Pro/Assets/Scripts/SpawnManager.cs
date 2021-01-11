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

    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefabs;

    private EnemyWaveIndex _waveIndex;
    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerups = false;
    private int enemiesSpawned;
    private const int TrackerSpawnChance = 33;
    private readonly int[] _powerupLootTable =
    {
        25, //25 for ammo           index = 0
        20, //20 for speed          index = 1
        13, //13 for triple shot    index = 2
        12, //12 for shield         index = 3
        10, //10 for half speed     index = 4
        8,   //8 for heal           index = 5
        7,   //7 for multi shot     index = 6
        5    //5 for damage         index = 7
    };

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
                SpawnRandomEnemy();
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

    private void SpawnRandomEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
        var randEnemy = Random.Range(0, 100);

        //33% to spawn tracker
        var enemy = Instantiate(randEnemy <= TrackerSpawnChance ? _enemyPrefabs[1] : _enemyPrefabs[0], posToSpawn, Quaternion.identity);
        enemy.transform.parent = _enemyContainer.transform;

        enemiesSpawned++;
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawningPowerups == false)
        {
            SpawnRandomPowerup();

            yield return new WaitForSeconds(Random.Range(8.0f, 16.0f));
        }
    }

    private void SpawnRandomPowerup()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
        int powerupToSpawn = 0;
        int total = 0;

        foreach (var item in _powerupLootTable)
        {
            total += item;
        }
        var randPowerup = Random.Range(0, total);
        
        for (int i = 0; i < _powerupLootTable.Length; i++)
        {
            if (randPowerup <= _powerupLootTable[i])
            {
                powerupToSpawn = i;
                Debug.Log("spawning: " + _powerupPrefabs[i].name);
                break;
            }
            randPowerup -= _powerupLootTable[i];
        }
        
        var powerup = Instantiate(_powerupPrefabs[powerupToSpawn], posToSpawn, Quaternion.identity);
    }

    public void OnPlayerDeath()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerups = true;
    }
    
}

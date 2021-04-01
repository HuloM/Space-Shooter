using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //spawn enemy every 5 seconds
    //create a coroutine of type IEnumerator -- yield events
    //while loop 

    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefabs;
    [SerializeField] private GameObject _bossEnemy;

    private EnemyWaveIndex _waveIndex;
    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerups = false;
    private bool _bossSpawned = false;
    private int enemiesSpawned;
    private readonly int[] _powerupLootTable =
    {
        20, //20 for ammo           index = 0
        20, //20 for speed          index = 1
        13, //13 for triple shot    index = 2
        12, //12 for shield         index = 3
        10, //10 for half speed     index = 4
        8,   //8 for heal           index = 5
        7,   //7 for multi shot     index = 6
        5,   //5 for damage         index = 7
        5    //5 for homing         index = 8
    };

    private readonly int[] _enemyLootTable =
    {
        80, //70 for normal         index = 0
        15, //25 for tracker        index = 1
        5    //5 for smart          index = 2 
    };
    public int enemiesKilled;
    
    public void StartSpawnSequence()
    {
        _waveIndex = EnemyWaveIndex.wave01; 
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }


    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (_stopSpawningEnemies == false)
        {
            while (enemiesSpawned < 5 * ((int) _waveIndex + 1))
            {
                SpawnRandomEntityFromLootTable(_enemyLootTable, _enemyPrefabs);
                enemiesSpawned++;
                yield return new WaitForSeconds(1f);
            }

            if (enemiesKilled >= 5 * ((int) _waveIndex + 1))
            {
                if (_waveIndex >= EnemyWaveIndex.wave03)
                    _stopSpawningEnemies = true;
                
                enemiesKilled = 0;
                enemiesSpawned = 0;
                _waveIndex++;
            }

            yield return new WaitForSeconds(5.0f);
        }

        if (_stopSpawningEnemies && _waveIndex == EnemyWaveIndex.waveBoss && _bossSpawned == false)
        {
            var posToSpawn = new Vector3(0.0f, 11.0f, 0.0f);
            var entity = Instantiate(_bossEnemy, posToSpawn, Quaternion.identity);

            entity.gameObject.transform.parent = _enemyContainer.transform;
            _bossSpawned = true;
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawningPowerups == false)
        {
            SpawnRandomEntityFromLootTable(_powerupLootTable, _powerupPrefabs);

            yield return new WaitForSeconds(Random.Range(8.0f, 12.0f));
        }
    }
    private void SpawnRandomEntityFromLootTable(int[] lootTable, GameObject[] prefabs)
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 7f);
        int entityToSpawn = 0;
        int total = 0;

        foreach (var item in lootTable)
        {
            total += item;
        }
        var randEntity = Random.Range(0, total); 
        
        for (int i = 0; i < lootTable.Length; i++)
        {
            if (randEntity <= lootTable[i])
            {
                entityToSpawn = i;
                Debug.Log("spawning: " + prefabs[i].name);
                break;
            }
            randEntity -= lootTable[i];
        }
        
        var entity = Instantiate(prefabs[entityToSpawn], posToSpawn, Quaternion.identity);

        entity.gameObject.transform.parent = _enemyContainer.transform;
    }

    public void OnPlayerDeath()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerups = true;
    }
    
}

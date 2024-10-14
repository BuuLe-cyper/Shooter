using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    public Transform[] spawnPoints;

    public float[] spawnTimes;

    public int[] enemyCountPerWave;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int wave = 0; wave < spawnTimes.Length; wave++)
        {
            yield return new WaitForSeconds(spawnTimes[wave]);

            int enemyCount = enemyCountPerWave[wave];

            for (int i = 0; i < enemyCount; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}

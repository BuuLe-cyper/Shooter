using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public float[] spawnTimes;
    public int[] totalEnemiesPerWave;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int wave = 0; wave < spawnTimes.Length; wave++)
        {
            yield return new WaitForSeconds(spawnTimes[wave]);

            int totalEnemies = totalEnemiesPerWave[wave];

            int spawnedEnemies = 0;

            for (int enemyIndex = 0; enemyIndex < enemyPrefabs.Length - 1; enemyIndex++)
            {
                float spawnFactor = (enemyPrefabs.Length - 1 - enemyIndex) / (float)enemyPrefabs.Length;

                int enemyCount = Mathf.FloorToInt(totalEnemies * spawnFactor);

                for (int i = 0; i < enemyCount; i++)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    Instantiate(enemyPrefabs[enemyIndex], spawnPoint.position, Quaternion.identity);

                    spawnedEnemies++;

                    yield return new WaitForSeconds(0.5f);
                }
            }

            if (wave == spawnTimes.Length - 1)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefabs[enemyPrefabs.Length - 1], spawnPoint.position, Quaternion.identity);

                yield return new WaitForSeconds(0.5f);
            }

            while (spawnedEnemies < totalEnemies)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length - 1)], spawnPoint.position, Quaternion.identity);

                spawnedEnemies++;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}

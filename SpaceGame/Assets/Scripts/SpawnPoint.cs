using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;
    private HashSet<GameObject> spawnedEnemyReference;
    public int minSpawnAmount;
    public int maxSpawnAmount;
    private Coroutine spawnClock;
    // Start is called before the first frame update
    void Start()
    {
        spawnClock = StartCoroutine(SpawnClockCO());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnEnemy()
    {
        int numbertoSpawn = Random.Range(minSpawnAmount, maxSpawnAmount);
        int numberOfEnemyTypes = enemies.Count;
        for (int x = 0; x < numbertoSpawn; x++)
        {
            int randomEnemy = Random.Range(0, numberOfEnemyTypes - 1);
            int randomX = Random.Range(-10, 10);
            int randomY = Random.Range(-10, 10);
            Vector3 spawnPosition = gameObject.transform.position + new Vector3(randomX, randomY, 0);
            GameObject tempEnemyRef = GameObject.Instantiate(enemies[randomEnemy]);
            tempEnemyRef.transform.position = spawnPosition;
        }
    }

    IEnumerator SpawnClockCO()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(120);
        }

    }
}

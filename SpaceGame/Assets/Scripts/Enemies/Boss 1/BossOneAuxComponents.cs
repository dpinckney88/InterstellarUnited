using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOneAuxComponents : MonoBehaviour
{
    [SerializeField] private List<GameObject> basicProjectiles;
    [SerializeField] private List<GameObject> largeBeams;
    [SerializeField] private List<GameObject> homingMissiles;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject projectileLarge;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private GameObject enemySpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LargeProjectileFireInSequence());
        StartCoroutine(BasicProjectileFireALL());
        StartCoroutine(LargeProjectileFireALL());
        StartCoroutine(SpawnEnemiesCO());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnEnemies()
    {
        int numberOfEnemiesToSpawn = Random.Range(1, spawnPoints.Count);
        Vector3[] bounds = Utilities.GetScreenBounds();
        Vector3 minBounds = bounds[0];
        Vector3 maxBounds = bounds[1];
        for (int x = 0; x < numberOfEnemiesToSpawn; x++)
        {
            GameObject enemy = GameObject.Instantiate(enemySpawn, spawnPoints[x].transform.position, Quaternion.identity);
            Vector3 enemyPos = enemy.transform.position;
            enemyPos.x = Mathf.Clamp(enemy.transform.position.x, minBounds.x, maxBounds.x);
            enemyPos.y = Mathf.Clamp(enemy.transform.position.y, minBounds.y, maxBounds.y);
            enemy.transform.position = enemyPos;
        }
    }

    private IEnumerator SpawnEnemiesCO()
    {
        while (true)
        {
            yield return new WaitForSeconds(20);
            SpawnEnemies();
        }
    }

    private IEnumerator BasicProjectileFireALL()
    {
        int count = 0;
        while (count <= 30)
        {
            foreach (GameObject projectileSlot in basicProjectiles)
            {
                GameObject p = GameObject.Instantiate(projectile, projectileSlot.transform.position, projectileSlot.transform.rotation);
                //Ignore colliding with the enemy layer, player projectile layer, and the enemy projectile layer
                p.GetComponent<Projectile_base>().IgnoreCollision(new List<int>() { 6, 7, 9 });

            }
            yield return new WaitForSeconds(.33f);
        }

    }

    private IEnumerator BasicProjectileFireInSequence()
    {
        int count = 0;
        while (count <= 30)
        {
            foreach (GameObject projectileSlot in basicProjectiles)
            {
                GameObject p = GameObject.Instantiate(projectile, projectileSlot.transform.position, projectileSlot.transform.rotation);
                //Ignore colliding with the enemy layer, player projectile layer, and the enemy projectile layer
                p.GetComponent<Projectile_base>().IgnoreCollision(new List<int>() { 6, 7, 9 });
                yield return new WaitForSeconds(.33f);
            }

        }

    }

    private IEnumerator LargeProjectileFireInSequence()
    {
        int count = 0;
        while (count <= 30)
        {
            foreach (GameObject projectileSlot in largeBeams)
            {
                GameObject p = GameObject.Instantiate(projectileLarge, projectileSlot.transform.position, projectileSlot.transform.rotation);
                //Ignore colliding with the enemy layer, player projectile layer, and the enemy projectile layer
                p.GetComponent<Projectile_base>().IgnoreCollision(new List<int>() { 6, 7, 9 });
                yield return new WaitForSeconds(.5f);
            }

        }
    }

    private IEnumerator LargeProjectileFireALL()
    {
        int count = 0;
        while (count <= 30)
        {
            foreach (GameObject projectileSlot in largeBeams)
            {
                GameObject p = GameObject.Instantiate(projectileLarge, projectileSlot.transform.position, projectileSlot.transform.rotation);
                //Ignore colliding with the enemy layer, player projectile layer, and the enemy projectile layer
                p.GetComponent<Projectile_base>().IgnoreCollision(new List<int>() { 6, 7, 9 });

            }
            yield return new WaitForSeconds(1.5f);
        }

    }
}



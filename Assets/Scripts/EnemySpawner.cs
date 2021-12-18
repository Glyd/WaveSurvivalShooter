using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject meleeEnemyPrefab;

    private Transform _spawnLocation;
    private float _spawnDelaySeconds = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        _spawnLocation = transform;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void spawnMeleeEnemies(int numberToSpawn, int currentLevel) {

        for(int i = 0; i < numberToSpawn; i++) {
            StartCoroutine(SpawnWithDelay(i, currentLevel));
        }
    }

    private IEnumerator SpawnWithDelay(int numberOfSpawns, int currentLevel) {
        yield return new WaitForSeconds(numberOfSpawns * _spawnDelaySeconds);
        int xPositionOffset = Random.Range(-12, 13);
        int yPositionOffset = Random.Range(-8, 9);
        Vector3 spawnPos = _spawnLocation.position;
        spawnPos.x += xPositionOffset;
        spawnPos.y += yPositionOffset;

        GameObject enemy = Instantiate(meleeEnemyPrefab, spawnPos, _spawnLocation.rotation);

        enemy.GetComponent<Enemy>().setCurrentLevel(currentLevel);

    }
}

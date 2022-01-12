using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Singleton

    public static EnemyManager Instance;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion
    [SerializeField] GameObject[] enemyPrefabs;

    public float enemySpawnDistance = 100f;

    [SerializeField] float spawnTime = 3.0f;
    float timer = 3f;
    public bool spawn = false;

    public List<GameObject> aliveEnemy = new List<GameObject>();

    [HideInInspector]
    public float minX, maxX, minY, maxY;
    void Start()
    {
        timer = spawnTime;        
    }

    public void LevelUp()
    {
        spawnTime *= 0.6f;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>=spawnTime)
        {
            //spawn prefabs
            SpawnNewEnemy();
            timer = 0f;
        }
    }

    void SpawnNewEnemy()
    {
        if(!spawn)
        {
            return;
        }
        float newX = Random.Range(minX, maxX);
        float newY = Random.Range(minY, maxY);

        Vector3 spawnPos = new Vector3(newX, newY  , enemySpawnDistance);
        int r = Random.Range(0, enemyPrefabs.Length - 1);
        GameObject go=Instantiate(enemyPrefabs[r],
            spawnPos,enemyPrefabs[r].transform.rotation);
        aliveEnemy.Add(go);
    }

    public void UpdateEnemy(List<GameObject> targetedEnemy)
    {
        foreach(GameObject enemy in aliveEnemy)
        {
            if(targetedEnemy.Contains(enemy))
            {
                enemy.GetComponent<EnemyLogic>().SetTargetMaterial();
            }
            else
            {
                enemy.GetComponent<EnemyLogic>().ResetRenderer();
            }
        }
    }
}

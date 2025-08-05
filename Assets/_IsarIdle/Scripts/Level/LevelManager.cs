using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private ParticleSystemPoolRef m_spawnFxPoolRef;
    
    [Header("Player Settings")]
    [SerializeField] private Entity playerPrefab;
    [SerializeField] private Transform m_playerSpawnTfm;

    [Header("Enemy Settings")]
    [SerializeField] private Vector2 m_minMaxSpawnDistanceFromPlayer;
    [SerializeField] private List<EnemyWave> m_enemyWaves;
    [SerializeField] private Transform m_enemiesParent;
    
    [SerializeField, ReadOnly]private int m_nbEnemyDead = 0;
    [SerializeField, ReadOnly]private int m_currentWaveIndex;
    [SerializeField, ReadOnly]private EnemyWave m_currentWave;

    private void Start()
    {
        SpawnPlayer();
        if (m_enemyWaves.Count <= 0)
            return;
        
        SpawnInitialEnemies(50, m_enemyWaves[0]);
        StartWave(0);
    }

    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, m_playerSpawnTfm.position, Quaternion.identity);
        
        if (EntityManager.Instance.Player != null)
            CameraManager.Instance.SetTarget(EntityManager.Instance.Player.transform);
    }

    private IEnumerator SpawnEnemyWave(EnemyWave wave)
    {
        while (m_nbEnemyDead < m_currentWave.nbEnemies)
        {
            EnemyWave.EnemyEntry enemyEntry = wave.GetRandomEnemyEntry();
            
            Vector3 spawnPosition = AstarPath.active.GetNearest(GetSpawnPositionPlayer()).position;
            
            m_spawnFxPoolRef.pool.Spawn(spawnPosition, Quaternion.identity, m_spawnFxPoolRef.pool.transform);
            Entity _spawnedEnemy = enemyEntry.enemyPool.pool.Spawn(spawnPosition, Quaternion.identity, m_enemiesParent);
            _spawnedEnemy.transform.localScale = Vector3.zero;
            _spawnedEnemy.transform.DOScale(1, 1f).SetEase(Ease.OutElastic, 0.5f);
            if (_spawnedEnemy.TryGetModule(out HealthEM _healthEM))
                _healthEM.OnDeath += OnEnemyDeath;
            
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }
    
    private void SpawnInitialEnemies(int count, EnemyWave sourceWave)
    {
        GridGraph grid = AstarPath.active.data.gridGraph;
        List<GraphNode> walkableNodes = new List<GraphNode>();

        // Step 1: Collect all walkable nodes
        grid.GetNodes(node =>
        {
            if (node.Walkable)
                walkableNodes.Add(node);
        });

        if (walkableNodes.Count == 0)
        {
            Debug.LogWarning("No walkable nodes found on the graph.");
            return;
        }

        // Step 2: Spawn enemies at random walkable nodes
        for (int i = 0; i < count; i++)
        {
            GraphNode chosenNode = walkableNodes[Random.Range(0, walkableNodes.Count)];
            Vector3 spawnPos = (Vector3)chosenNode.position;

            var enemyData = sourceWave.enemies[Random.Range(0, sourceWave.enemies.Count)];
            Entity enemy = enemyData.enemyPool.pool.Spawn(spawnPos, Quaternion.identity, m_enemiesParent);

            if (enemy.TryGetModule(out HealthEM _healthEM))
                _healthEM.OnDeath += OnEnemyDeath;
        }
    }


    private void OnEnemyDeath(Entity _deadEnemy)
    {
        if (_deadEnemy.TryGetModule(out HealthEM _healthEM))
            _healthEM.OnDeath -= OnEnemyDeath;
        
        m_nbEnemyDead++;
        
        if (m_nbEnemyDead >= m_currentWave.nbEnemies)
        {
            StartWave(m_currentWaveIndex + 1);
        }
    }

    private void StartWave(int waveIndex)
    {
        if (waveIndex > m_enemyWaves.Count - 1)
        {
            waveIndex = m_enemyWaves.Count - 1;
            Debug.LogWarning("Reached max wave. Clamped wave index");
        }
        
        m_currentWave = m_enemyWaves[waveIndex];
        m_currentWaveIndex = waveIndex;
        
        StartCoroutine(SpawnEnemyWave(m_currentWave));
    }

    private Vector3 GetSpawnPositionPlayer()
    {
        if (EntityManager.Instance.Player != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 offset = new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(m_minMaxSpawnDistanceFromPlayer.x, m_minMaxSpawnDistanceFromPlayer.y);
            return EntityManager.Instance.Player.transform.position + offset;
        }
        
        return m_playerSpawnTfm.position;
    }
}

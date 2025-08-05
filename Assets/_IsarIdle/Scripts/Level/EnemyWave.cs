using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class EnemyWave
{
    [System.Serializable]
    public class EnemyEntry
    {
        public EntityPoolRef enemyPool;
        public int level;
        public float weight;
    }

    public List<EnemyEntry> enemies = new List<EnemyEntry>();
    public float spawnInterval = 1f;
    public int nbEnemies;
    
    public EnemyWave.EnemyEntry GetRandomEnemyEntry()
    {
        float totalWeight = 0f;

        // Step 1: Calculate total weight
        foreach (var entry in enemies)
            totalWeight += entry.weight;

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("Total enemy weight is zero or negative.");
            return null;
        }

        // Step 2: Roll a random value
        float randomValue = Random.value * totalWeight;
        float cumulative = 0f;

        // Step 3: Select based on weight
        foreach (var entry in enemies)
        {
            cumulative += entry.weight;
            if (randomValue <= cumulative)
                return entry;
        }

        // Fallback (shouldn't happen)
        return enemies[enemies.Count - 1];
    }

}
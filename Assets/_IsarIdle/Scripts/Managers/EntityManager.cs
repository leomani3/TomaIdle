using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    public static Action<Entity> onPlayerRegistered;

    [SerializeField] private readonly Dictionary<GameObject, Entity> m_entitiesByGameobject = new Dictionary<GameObject, Entity>();
    [SerializeField] private readonly Dictionary<Collider, Entity> m_entitiesByCollider = new Dictionary<Collider, Entity>();
    
    public Dictionary<GameObject, Entity> EntitiesByGameobject => m_entitiesByGameobject;
    public Dictionary<Collider, Entity> EntitiesByCollider => m_entitiesByCollider;
    public Entity Player => m_player;

	[SerializeField, ReadOnly] private List<Entity> m_enemies;
    [SerializeField, ReadOnly] private Entity m_player;

    public void RegisterEntity(Entity entity)
    {
        if (entity.TryGetModule(out TeamEM _teamEM))
        {
            if (_teamEM.team.HasFlag(Team.Enemy))
                m_enemies.Add(entity);
            
            if (_teamEM.team.HasFlag(Team.Player))
            {
                if (m_player != null)
                {
                    Debug.LogWarning("Player entity already registered, replacing it with the new one.");
                }
                m_player = entity;
                onPlayerRegistered?.Invoke(m_player);
            }
        }

		if (!m_entitiesByGameobject.ContainsKey(entity.gameObject))
        {
            m_entitiesByGameobject.Add(entity.gameObject, entity);
            
            entity.OnRegistered();
        }
        
        if (!m_entitiesByCollider.ContainsKey(entity.EntityCollider))
            m_entitiesByCollider.Add(entity.EntityCollider, entity);
    }

    public void UnregisterEntity(Entity entity)
    {
        if (entity.TryGetModule(out TeamEM _teamEM))
        {
            if (_teamEM.team.HasFlag(Team.Enemy))
                m_enemies.Remove(entity);
        }
        
        
        if (m_entitiesByGameobject.ContainsKey(entity.gameObject))
        {
            m_entitiesByGameobject.Remove(entity.gameObject);
            entity.OnUnregistered();
        }
        
        if (!m_entitiesByCollider.ContainsKey(entity.EntityCollider))
            m_entitiesByCollider.Remove(entity.EntityCollider);
    }

    public Entity GetEntityByCollider(Collider _collider)
    {
        m_entitiesByCollider.TryGetValue(_collider, out var entity);
        return entity;
    }

    public void ClearAll()
    {
        m_entitiesByGameobject.Clear();
    }
    
    public Entity GetClosestEnemy(Vector3 _position)
    {
        Entity _closestEntity = null;
        foreach (Entity entity in m_enemies)
        {
            if (_closestEntity == null || Vector3.Distance(_position, entity.transform.position) < Vector3.Distance(_position, _closestEntity.transform.position))
                _closestEntity = entity;
        }
        
        return _closestEntity;
    }
}
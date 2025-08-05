using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private ProjectileData projectileData;
    
    [Title("Reference")]
    [SerializeField, Required] private Rigidbody m_rb;
    
    private Entity m_targetEntity;
    private Vector3 m_targetPosition;
    private Entity m_owner;
    private bool m_hasLaunched;
    private bool m_hasHitTarget;
    private bool m_isHoming;
    private List<AbilityEffect> m_effects;

    private void Reset()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    [Button]
    public void TestLaunch()
    {
        m_rb.linearVelocity = transform.forward * projectileData.projectileSpeed;
    }

    // For homing projectiles
    public void LaunchAtEntity(Entity ownerEntity, Entity target, List<AbilityEffect> _effects)
    {
        m_effects = _effects;
        m_owner = ownerEntity;
        m_targetEntity = target;
        m_isHoming = true;
        m_hasLaunched = true;
    }

    // For straight projectiles
    public void LaunchAtPosition(Entity ownerEntity, Vector3 targetPos, List<AbilityEffect> _effects)
    {
        m_effects = _effects;
        m_owner = ownerEntity;
        m_targetPosition = targetPos;
        m_isHoming = false;
        m_hasLaunched = true;

        Vector3 direction = (m_targetPosition - transform.position).normalized;
        m_rb.linearVelocity = (transform.forward + direction) * projectileData.projectileSpeed;
        transform.LookAt(targetPos);
    }

    private void FixedUpdate()
    {
        if (!m_hasLaunched || m_hasHitTarget) return;

        if (m_isHoming && m_targetEntity != null)
        {
            Vector3 direction = (m_targetEntity.transform.position - transform.position).normalized;
            m_rb.linearVelocity = direction * projectileData.projectileSpeed;
            transform.forward = direction;

            if (Vector3.Distance(transform.position, m_targetEntity.transform.position) < 0.2f)
            {
                HitTarget(m_targetEntity);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_hasLaunched || m_hasHitTarget || m_isHoming)
            return;

        Entity hitEntity = other.GetComponent<Entity>();
        if (hitEntity != null && hitEntity != m_owner)
        {
            HitTarget(hitEntity);
            Destroy(gameObject);
        }
    }

    private void HitTarget(Entity hitEntity)
    {
        m_hasHitTarget = true;

        if (projectileData.hitVFXPoolRef != null)
        {
            projectileData.hitVFXPoolRef.pool.Spawn(transform.position, Quaternion.identity);
        }
        
        foreach (AbilityEffect abilityEffect in m_effects)
            abilityEffect.ApplyEffect(m_owner, hitEntity);

        if (m_isHoming)
        {
            Destroy(gameObject);
        }
    }
}

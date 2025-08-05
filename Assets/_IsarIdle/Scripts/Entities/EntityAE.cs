using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityAE : MonoBehaviour
{
    [SerializeField] private List<Renderer> m_renderers;
    [SerializeField] private Transform m_hips;
    [SerializeField] private Entity m_entity;

    private Tween m_flashTween;
    
    private void Reset()
    {
		m_entity = GetComponentInParent<Entity>();
        m_renderers = GetComponentsInChildren<Renderer>().ToList();
    }

    private void OnDeathAnimationComplete()
    {
        PoolManager.Instance.smokeExplosionFXPool.pool.Spawn(m_hips.transform.position,  Quaternion.identity,  PoolManager.Instance.smokeExplosionFXPool.pool.transform);
        if (m_entity.TryGetModule(out HealthEM health))
        {
            health.OnDeathAnimationComplete();
        }
    }

    public void OnAttackTrigger()
    {
        if (m_entity.TryGetModule(out AbilityEM abilityEM))
            abilityEM.OnMainAbilityTrigger();
    }

    public void OnAttackFinished()
    {
        if (m_entity.TryGetModule(out AbilityEM abilityEM))
            abilityEM.OnMainAbilityFinished();
    }

    public void Flash()
    {
        m_flashTween.Kill();
        foreach (Renderer renderer in m_renderers)
        {
            renderer.material.SetFloat("_Value", 10);
            //skinnedMeshRenderer.material.SetColor("_EmissionColor", Color.white);
            m_flashTween = renderer.material.DOFloat(0, "_Value", 0.25f);
           //m_flashTween = skinnedMeshRenderer.material.DOColor(Color.black, "_EmissionColor", 0.25f);
        }
    }
}
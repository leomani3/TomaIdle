using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(StatsEM))]
public class AbilityEM : AEntityModule
{
    [Title("Debug")]
    [SerializeField] private bool m_infiniteMana;
    
    [Title("Abilities")]
    [SerializeField] protected AbilityData m_mainAbility;
    [SerializeField] protected List<AbilityData> m_secondaryAbilities = new List<AbilityData>();
    
    [Title("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_mainAbilitySpawnPos;
    [SerializeField] private AbilityPoolRef m_abilityPoolRef;
    
    [Title("Settings")]
    [SerializeField] private LayerMask m_targetLayerMask;
    
    private AnimatorOverrideController m_runtimeOverrideController;
    
    public AbilityData MainAbility => m_mainAbility;
    public List<AbilityData> SecondaryAbilities => m_secondaryAbilities;
    [SerializeField, ReadOnly] protected float m_currentMana;
    public SerializedDictionary<AbilityData, float> m_abilityCooldowns = new SerializedDictionary<AbilityData, float>();
    protected StatsEM StatsEM => m_linkedEntity.GetModule<StatsEM>();
    private bool m_active = true;

    protected virtual void Start()
    {
        m_currentMana = StatsEM.MaxMana.Value;

        if (m_linkedEntity.TryGetModule(out HealthEM _healthEM))
        {
            _healthEM.OnDeath += OnLinkedEntityDeath;
        }
        
        m_active = true;
    }

    private void OnEnable()
    {
        m_currentMana = StatsEM.MaxMana.Value;
        m_active = true;
    }

    protected virtual void Update()
    {
        if (!m_active)
            return;
        
        //Ability cooldown tick
        foreach (AbilityData abilityData in m_abilityCooldowns.Keys.ToArray())
        {
            m_abilityCooldowns[abilityData] -= Time.deltaTime;

            if (m_abilityCooldowns[abilityData] <= 0)
                m_abilityCooldowns.Remove(abilityData);
        }
        
        //mana regen
        if (m_currentMana < StatsEM.MaxMana.Value)
        {
            m_currentMana += StatsEM.ManaRegen.Value * Time.deltaTime;
            m_currentMana = Mathf.Clamp(m_currentMana, 0, StatsEM.MaxMana.Value);
        }
    }

    private void OnLinkedEntityDeath(Entity _deadEntity)
    {
        if (m_linkedEntity != null && _deadEntity == m_linkedEntity)
        {
            m_active = false;
            SetMainAbilityUsable(false);
            m_linkedEntity.GetModule<HealthEM>().OnDeath -= OnLinkedEntityDeath;
        }
    }

    public virtual bool UseAbility(AbilityData _abilityData, AbilityContext _context)
    {
        if (!m_active)
            return false;
        
        if (_abilityData == m_mainAbility || (CheckMana(_abilityData) && CheckCooldown(_abilityData)))
        {
            m_abilityPoolRef.pool.Spawn(_context.targetPosition, quaternion.identity, m_abilityPoolRef.pool.transform).UseAbility(_abilityData, _context, m_targetLayerMask);
            
            if (_abilityData != m_mainAbility)
            {
                m_currentMana -= _abilityData.manaCost;
                m_currentMana = m_currentMana < 0 ? 0 : m_currentMana;
                
                m_abilityCooldowns[_abilityData] = _abilityData.cooldown;
            }
       
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckMana(AbilityData _abilityData)
    {
        return m_infiniteMana || m_currentMana >= _abilityData.manaCost;
    }

    private bool CheckCooldown(AbilityData _abilityData)
    {
        return !m_abilityCooldowns.ContainsKey(_abilityData) || m_abilityCooldowns[_abilityData] <= 0;
    }

    #region MainAbility
    public void SetMainAbilityUsable(bool _inRange)
    {
        m_animator.SetBool("Attacking", _inRange);
        
        if (_inRange)
            UpdateAnimationSpeed();
        else
            m_animator.speed = 1f;
    }
    
    private void UpdateAnimationSpeed()
    {
        var clips = m_animator.runtimeAnimatorController.animationClips;
        var attackClip = clips.FirstOrDefault(c => c.name.Contains("Attack"));
        if (attackClip != null)
        {
            float baseLength = attackClip.length;
            float m_attackSpeed = StatsEM.AttackSpeed.Value;
            float desiredDuration = 1f / m_attackSpeed;
            float adjustedSpeed = baseLength > 0f ? baseLength / desiredDuration : 1f;

            m_animator.speed = adjustedSpeed;
        }
    }
    
    public void OnMainAbilityFinished()
    {

    }

    public void OnMainAbilityTrigger()
    {
        UseAbility(m_mainAbility, new AbilityContextBuilder()
            .SetTargetPosition(m_mainAbilitySpawnPos.position)
            .SetOwnerEntity(m_linkedEntity)
            .SetTargetEntity(null)
            .Build());
    }
    #endregion

}
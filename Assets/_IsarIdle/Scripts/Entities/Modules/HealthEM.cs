using UnityEngine;
using System;
using DamageNumbersPro;
using MyBox;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;
using Isar.UI;
using BreakInfinity; // Ensure this is the correct namespace for BigDouble in your project

public class HealthEM : AEntityModule
{
    public Action<BigDouble, BigDouble> OnHealthChanged;
    public Action<bool> OnDamaged;
    public Action OnHealed;
    public Action<Entity> OnDeath;
    public Action OnRevive;

    [Header("References")] 
    [SerializeField] private Animator m_animator;
    [SerializeField] private EntityAE m_entityAE;
    
    [Header("Health Settings")]
    [SerializeField] private DamageNumber m_damageNumber;

    [Header("UI")]
    [SerializeField] private bool m_showUI;
    [SerializeField, ShowIf(nameof(m_showUI))] private GameObjectPoolRef m_entityUIPoolRef;

    [Header("Debug")]
    [SerializeField, ReadOnly] protected BigDouble m_currentHealth;
    
    private EntityUI m_spawnedUI;
    private bool m_isDead = false;

    public BigDouble CurrentHealth => m_currentHealth;
    public BigDouble MaxHealth => m_linkedEntity.GetModule<StatsEM>().MaxHealth.Value;
    public bool IsDead => m_isDead;

    protected override void Reset()
    {
        base.Reset();
        m_linkedEntity = GetComponent<Entity>();
        m_animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        m_linkedEntity.GetModule<StatsEM>().MaxHealth.OnValueChanged += OnMaxHealthChanged;
        m_currentHealth = MaxHealth;

        if (m_showUI)
        {
            m_spawnedUI = m_entityUIPoolRef.pool.Spawn(UIManager.Instance.GetPanel<GamePanel>().EntityUIParent).GetComponent<EntityUI>();
            m_spawnedUI.Bind(this);
        }
    }

    [Button]
    public void TestTakeDamage()
    {
        TakeDamage((BigDouble)Random.Range(1f, 20f));
    }

    public virtual void TakeDamage(BigDouble amount)
    {
        if (IsDead || amount <= BigDouble.Zero) return;
        
        m_entityAE.Flash();
        
        RectTransform parent = UIManager.Instance.GetPanel<GamePanel>().DamageTextParent;
        Vector3 screenPoint = CameraManager.Instance.Camera.WorldToScreenPoint(transform.position);

        m_damageNumber.SpawnGUI(parent, screenPoint, Mathf.RoundToInt((float)amount.ToDouble()));

        m_currentHealth -= amount;

        bool damageWillKill = m_currentHealth <= BigDouble.Zero;

        OnDamaged?.Invoke(damageWillKill);
        OnHealthChanged?.Invoke(damageWillKill ? BigDouble.Zero : m_currentHealth, MaxHealth);

        if (damageWillKill)
        {
            Die();
        }
    }

    public virtual void Heal(BigDouble amount)
    {
        if (amount.IsZero()) return;
        if (IsDead) return;

        m_currentHealth += amount;
        m_currentHealth = BigDouble.Min(m_currentHealth, MaxHealth);

        OnHealed?.Invoke();
        OnHealthChanged?.Invoke(m_currentHealth, MaxHealth);
    }

    private void Die()
    {
        if (m_isDead) return;

        m_linkedEntity.UnregisterEntity();
        m_currentHealth = BigDouble.Zero;
        m_isDead = true;
        m_animator.SetBool("Dead", IsDead);
        OnDeath?.Invoke(m_linkedEntity);

        if (m_spawnedUI != null)
            m_entityUIPoolRef.pool.Despawn(m_spawnedUI.gameObject);
    }

    public void Revive()
    {
        m_isDead = false;
        m_currentHealth = MaxHealth;
        m_animator.SetBool("Dead", IsDead);
        OnRevive?.Invoke();
        OnHealthChanged?.Invoke(m_currentHealth, MaxHealth);
    }

    public void OnDeathAnimationComplete()
    {
        Destroy(gameObject);
        // Todo: despawn via pool
    }

    protected virtual void OnMaxHealthChanged(BigDouble prevMaxHealth, BigDouble newMaxHealth)
    {
        if (newMaxHealth > prevMaxHealth && !IsDead)
            Heal(newMaxHealth - prevMaxHealth);
        else if (newMaxHealth < prevMaxHealth)
            m_currentHealth = BigDouble.Max(m_currentHealth, newMaxHealth);
        
        OnHealthChanged?.Invoke(m_currentHealth, newMaxHealth);
    }
}

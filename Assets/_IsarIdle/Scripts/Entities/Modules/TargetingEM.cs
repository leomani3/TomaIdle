using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetingEM : AEntityModule
{
    [SerializeField] private float m_aggroRange;
    
    protected Entity m_mainTarget;
    protected AbilityEM m_abilityEM;
    protected Camera m_mainCamera;
    protected bool m_active;

    protected virtual void Awake()
    {
        m_abilityEM = m_linkedEntity.GetModule<AbilityEM>();
        m_mainCamera = CameraManager.Instance.Camera;

        if (m_linkedEntity != null && m_linkedEntity.TryGetModule(out HealthEM _healthEM))
        {
            _healthEM.OnDeath += OnLinkedEntityDeath;
        }
        
        m_mainTarget = null;
        m_active = true;
    }

    private void OnLinkedEntityDeath(Entity _deadEntity)
    {
        if (m_linkedEntity != null && _deadEntity == m_linkedEntity)
        {
            if (m_linkedEntity.TryGetModule(out HealthEM _healthEM))
                _healthEM.OnDeath -= OnLinkedEntityDeath;
            
            m_active = false;
        }
    }

    protected virtual void Update()
    {
        if (!m_active)
            return;

        GetMainTarget();

        CheckMainAbility();
        CheckSecondaryAbilities();
        HandleMovement();
    }

    private void GetMainTarget()
    {
        Entity _chosenTarget = null;
        if (m_linkedEntity.TryGetModule(out TeamEM _teamEM))
        {
            switch (_teamEM.TargetTeam)
            {
                case Team.None:
                    break;
                case Team.Player:
                    if (EntityManager.Instance.Player == null)
                        return;
                    
                    if (Vector3.Distance(transform.position, EntityManager.Instance.Player.transform.position) < m_aggroRange)
                        _chosenTarget = EntityManager.Instance.Player;
                    break;
                case Team.Enemy:
                    _chosenTarget = EntityManager.Instance.GetClosestEnemy(transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (_chosenTarget != null && _chosenTarget != m_mainTarget)
        {
            m_mainTarget = _chosenTarget;
            OnNewMainTargetAcquired();
        }
    }
    
    private void OnNewMainTargetAcquired()
    {
        m_linkedEntity.GetModule<MovementEM>().SetTarget(m_mainTarget.transform);

        if (m_mainTarget.TryGetModule(out HealthEM healthEM))
            healthEM.OnDeath += OnMainTargetDied;
    }

    private void OnMainTargetDied(Entity _deadEntity)
    {
        if (m_mainTarget == null)
            return;
        
        if (m_mainTarget.TryGetModule(out HealthEM healthEM))
            healthEM.OnDeath -= OnMainTargetDied;
        
        m_mainTarget = null;
    }

    private void CheckMainAbility()
    {
        if (m_abilityEM.MainAbility == null)
            return;
        
        if (m_mainTarget == null)
        {
            m_abilityEM.SetMainAbilityUsable(false);
            return;
        }
        
        m_abilityEM.SetMainAbilityUsable(IsInRange(m_abilityEM.MainAbility, m_mainTarget.transform.position));
    }

    private void HandleMovement()
    {
        if (m_mainTarget != null)
        {
            if (IsInRange(m_abilityEM.MainAbility, m_mainTarget.transform.position))
            {
                m_linkedEntity.transform.LookAt(m_mainTarget.transform);
                m_linkedEntity.GetModule<MovementEM>().Stop();
            }
            else
            {
                m_linkedEntity.GetModule<MovementEM>().SetTarget(m_mainTarget.transform);
            }
        }
        else
        {
            if (m_linkedEntity.TryGetModule(out MovementEM _movementEM))
            {
                if (!_movementEM.AIAgent.pathPending && (_movementEM.AIAgent.reachedEndOfPath || !_movementEM.AIAgent.hasPath)) {
                    _movementEM.SetDestination(PickRandomPoint());
                } 
            }
        } 
    }
    
    private void CheckSecondaryAbilities()
    {
        foreach (AbilityData abilityData in m_abilityEM.SecondaryAbilities)
        {
            Entity _target = GetTarget(abilityData.targetAcuisitionType);
            
            if (_target == null)
                return;
            
            if (IsInRange(abilityData, _target.transform.position))
            {
                m_abilityEM.UseAbility(abilityData, new AbilityContextBuilder()
                    .SetTargetPosition(_target.transform.position)
                    .SetTargetEntity(_target)
                    .SetOwnerEntity(m_linkedEntity)
                    .Build());
            }
        }
    }

    private bool IsInRange(AbilityData abilityData, Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < abilityData.range;
    }

    private Entity GetTarget(TargetAcuisitionType _targetAcuisitionType)
    {
        switch (_targetAcuisitionType)
        {
            case TargetAcuisitionType.Closest:
                return EntityManager.Instance.GetClosestEnemy(transform.position);
            case TargetAcuisitionType.Furthest:
                break;
            case TargetAcuisitionType.Random:
                break;
        }

        return null;
    }
    
    private Vector3 PickRandomPoint () {
        Vector3 point = Random.insideUnitSphere * 10;

        point.y = 0;
        point += transform.position;
        return point;
    }
}
using Pathfinding;
using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class MovementEM : AEntityModule
{

	private IAstarAI m_AIAgent;
	private TargetType targetType = TargetType.None;
	private Transform transformTarget;
	private Vector3 positionTarget;
	private float speedCache;
	private int lastSpeedFrame = -1;
	private bool wasMoving = false;
	private bool m_active;

	public bool MovementChangedThisFrame { get; protected set; } = false;
	public IAstarAI AIAgent => m_AIAgent;

	enum TargetType
	{
		None,
		Transform,
		Vector3
	}

	private void Awake()
	{
		m_active = true;

		if (m_linkedEntity != null && m_linkedEntity.TryGetModule(out HealthEM _healthEM))
		{
			_healthEM.OnDeath += OnLinkedEntityDeath;
		}
	}

	private void OnLinkedEntityDeath(Entity _deadEntity)
	{
		if (m_linkedEntity != null && _deadEntity == m_linkedEntity)
		{
			if (m_linkedEntity.TryGetModule(out HealthEM _healthEM))
				_healthEM.OnDeath -= OnLinkedEntityDeath;

			Stop();
			m_active = false;
		}
	}

	private void OnEnable()
	{
		if (m_AIAgent == null)
			m_AIAgent = GetComponent<IAstarAI>();
		if (m_AIAgent != null) m_AIAgent.onSearchPath += UpdateDestination;
	}

	private void OnDisable()
	{
		if (m_AIAgent != null) m_AIAgent.onSearchPath -= UpdateDestination;
	}

	private void Update()
	{
		if (!m_active)
			return;
		
		UpdateDestination();
		if (wasMoving != IsMoving())
		{
			wasMoving = !wasMoving;
			MovementChangedThisFrame = true;
		}
		else
		{
			MovementChangedThisFrame = false;
		}
	}

	private void UpdateDestination()
	{
		if (m_AIAgent == null) return;
		switch (targetType)
		{
			case TargetType.Transform:
				if (transformTarget != null)
				{
					m_AIAgent.destination = transformTarget.position;
				}
				else
				{
					Stop();
				}
				break;
			case TargetType.Vector3:
				m_AIAgent.destination = positionTarget;
				break;
			case TargetType.None:
			default:
				if (!m_AIAgent.isStopped)
				{
					Stop();
				}
				break;
		}
	}

	public void SetTarget(Transform target)
	{
		if (target == null || m_AIAgent == null)
		{
			Stop();
			return;
		}
		targetType = TargetType.Transform;
		transformTarget = target;
		m_AIAgent.isStopped = false;
		UpdateDestination();
	}

	public void SetDestination(Vector3 position)
	{
		if (m_AIAgent == null)
		{
			Stop();
			return;
		}
		targetType = TargetType.Vector3;
		positionTarget = position;
		m_AIAgent.isStopped = false;
		UpdateDestination();
	}

	public Vector3 GetVelocity()
	{
		if (m_AIAgent == null) return Vector3.zero;
		return m_AIAgent.velocity;
	}

	public float GetSpeed()
	{
		if (m_AIAgent == null) return 0f;

		if (Time.frameCount != lastSpeedFrame)
		{
			speedCache = m_AIAgent.velocity.magnitude;
			lastSpeedFrame = Time.frameCount;
		}

		return speedCache;
	}

	public float GetMaxSpeed()
	{
		if (m_AIAgent == null) return 0f;
		return m_AIAgent.maxSpeed;
	}

	public bool IsMoving()
	{
		return m_AIAgent != null && m_AIAgent.isStopped == false && GetSpeed() > 0.05f;
	}

	public void Stop()
	{
		targetType = TargetType.None;
		if (m_AIAgent != null)
		{
			m_AIAgent.isStopped = true;
		}
	}
}

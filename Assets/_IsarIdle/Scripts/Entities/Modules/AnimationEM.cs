using UnityEngine;

[RequireComponent(typeof(Entity))]
public class AnimationEM : AEntityModule
{
	[Header("References")]
	[SerializeField] private Animator animator;

	protected MovementEM entityMovement;

	public Animator Animator => animator;

	protected override void Reset()
	{
		base.Reset();
		animator = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		if (m_linkedEntity.TryGetModule<MovementEM>(out MovementEM movement))
		{
			entityMovement = movement;
		}
	}

	private void LateUpdate()
	{
		UpdateMovementAnimation();
	}

	protected virtual void UpdateMovementAnimation()
	{
		if (entityMovement != null)
		{
			animator.SetFloat("Speed", entityMovement.GetSpeed() / entityMovement.GetMaxSpeed());
		}
	}
}

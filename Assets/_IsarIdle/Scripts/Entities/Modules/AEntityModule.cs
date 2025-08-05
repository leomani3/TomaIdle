using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class AEntityModule : MonoBehaviour
{
	[SerializeField, ReadOnly]
	protected Entity m_linkedEntity;

	protected virtual void Reset()
	{
		UpdateEntityEditor();
	}

#if UNITY_EDITOR
	[ShowIf("@m_linkedEntity == null"), Button("Update Entity")]
	private void UpdateEntityEditor()
	{
		Entity entity = GetComponent<Entity>();
		if (entity != m_linkedEntity)
		{
			m_linkedEntity = entity;
			m_linkedEntity.UpdateModulesEditor();
			m_linkedEntity.LinkModuleEditor(this);
		}
	}
#endif
}

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IEntity
{
	[SerializeField] private Collider m_entityCollider;
    protected Dictionary<Type, AEntityModule> m_entityModules = new Dictionary<Type, AEntityModule>();
	protected HashSet<Type> m_pureTypes = new HashSet<Type>();

	private static Dictionary<Type, Type> m_moduleBaseTypeLookup = new Dictionary<Type, Type>();
	private static Dictionary<Type, HashSet<Type>> m_moduleInheritenceLookup = new Dictionary<Type, HashSet<Type>>();

	[SerializeField, ReadOnly]
    private List<AEntityModule> m_entityModulesList = new List<AEntityModule>();
    
    public Collider EntityCollider => m_entityCollider;

#if UNITY_EDITOR
	private void Reset()
	{
		m_entityModulesList = new List<AEntityModule>(GetComponents<AEntityModule>());
	}

	[Button("Update Modules List")]
	public void UpdateModulesEditor()
    {
		m_entityModulesList = new List<AEntityModule>(GetComponents<AEntityModule>());
	}

    public void LinkModuleEditor(AEntityModule module)
    {
		if (module == null || m_entityModulesList.Contains(module)) return;
		m_entityModulesList.Add(module);
	}
#endif

	protected virtual void Awake()
    {
		foreach (AEntityModule module in m_entityModulesList)
        {
			RegisterModule(module);
        }

		//Register after adding modules so that they are available immediately
		EntityManager.Instance.RegisterEntity(this);
	}

	private void RegisterModule(AEntityModule module)
	{
		Type lookupType = GetLookupType(module.GetType());
		if (!m_moduleInheritenceLookup.TryGetValue(lookupType, out HashSet<Type> types))
		{
			types = new HashSet<Type>();

			Type entityModuleType = typeof(AEntityModule);
			Type type = lookupType;
			types.Add(type);
			if (type.IsSubclassOf(entityModuleType))
			{
				while (type.BaseType != entityModuleType)
				{
					type = type.BaseType;
					if (m_moduleInheritenceLookup.ContainsKey(type))
					{
						types.UnionWith(m_moduleInheritenceLookup[type]);
						break;
					}
					types.Add(type);
				}
			}

			m_moduleInheritenceLookup[lookupType] = types;
		}
		m_pureTypes.UnionWith(types);
		m_entityModules.Add(lookupType, module);
	}


	public void UnregisterEntity()
	{
		if (EntityManager.Instance != null)
			EntityManager.Instance.UnregisterEntity(this);
	} 

    protected virtual void OnDestroy()
    {
	    UnregisterEntity();
    }

    public virtual void OnRegistered()
    {
        // Optional: Override for custom logic
    }

    public virtual void OnUnregistered()
    {
        // Optional: Override for custom logic
    }
	private static bool FillLookupForType(Type type)
	{
		if (type == null || m_moduleBaseTypeLookup.ContainsKey(type))
			return false;
		HashSet<Type> typesToAdd = new HashSet<Type>(){
			type
		};
		Type entityModuleType = typeof(AEntityModule);
		Type targetType = null;

		if (type.IsSubclassOf(entityModuleType))
		{
			while (type.BaseType != entityModuleType)
			{
				type = type.BaseType;
				if (m_moduleBaseTypeLookup.ContainsKey(type))
				{
					targetType = m_moduleBaseTypeLookup[type];
					break;
				}
				typesToAdd.Add(type);
			}
			if (targetType == null)
			{
				targetType = type;
			}
		}
		else
		{
			return false;
		}
		foreach (Type t in typesToAdd)
		{
			m_moduleBaseTypeLookup[t] = targetType;
		}
		return true;
	}

	private Type GetLookupType(Type type)
	{
		if (type == null)
			return null;
		if (!m_moduleBaseTypeLookup.TryGetValue(type, out Type result))
		{
			if (!FillLookupForType(type))
			{
				Debug.LogError($"Type {type} is not a valid Entity Module type or does not inherit from AEntityModule.");
			}
			return m_moduleBaseTypeLookup[type];
		}
		return result;
	}

	public bool TryGetModule<T>(out T module) where T : AEntityModule
	{
		Type originalType = typeof(T);
		if (!m_pureTypes.Contains(originalType))
		{
			module = null;
			return false;
		}
		Type lookupType = GetLookupType(originalType);
		if (m_entityModules.TryGetValue(lookupType, out var foundModule))
		{
			module = foundModule as T;
			return true;
		}
		module = null;
		return false;
	}

	public T GetModule<T>() where T : AEntityModule
	{
		Type originalType = typeof(T);
		if (!m_pureTypes.Contains(originalType))
		{
			return null;
		}
		Type lookupType = GetLookupType(originalType);
		if (m_entityModules.TryGetValue(lookupType, out var module))
		{
			return module as T;
		}
		return null;
	}

	public bool HasModule<T>() where T : AEntityModule
	{
		return m_pureTypes.Contains(typeof(T));
	}

}
using UnityEngine;

namespace Isar.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
					return;
#endif
			if (Instance == null)
			{
				Instance = (T)this;
				return;
			}
			if (Instance != this)
				Destroy(gameObject);
		}
	}
}

using UnityEngine;

namespace _ClashRoyal.Scripts.Extensions
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        private static T _instance;

        private static readonly object Lock = new();

        private bool _persistent = false;

        public static T Instance
        {
            get
            {
                if (Quitting)
                    return null;

                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;

                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    switch (count)
                    {
                        case <= 0:
                            return _instance = new GameObject().AddComponent<T>();
                        case 1:
                            return _instance = instances[0];
                    }

                    for (var i = 1; i < instances.Length; i++)
                        Destroy(instances[i]);

                    return _instance = instances[0];

                }
            }
        }

        private void Awake()
        {
            if (_persistent)
                DontDestroyOnLoad(gameObject);

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }

    public abstract class Singleton : MonoBehaviour
    {
        protected static bool Quitting { get; private set; }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
    }
}
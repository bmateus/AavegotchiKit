using UnityEngine;

namespace PortalDefender.AavegotchiKit.Utils
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        bool isPersistent;

        private static T instance_;
        public static T Instance
        {
            get
            {
                if (instance_ == null)
                {
                    T[] instances = FindObjectsOfType<T>();

                    if (instances != null && instances.Length > 0)
                    {
                        if (instances.Length > 1)
                        {
                            Debug.LogError("Multiple instances of " + typeof(T).Name + " found in scene. Returning first instance found.");
                            return instances[0];
                        }
                        instance_ = instances[0];
                    }
                }

                if (instance_ == null)
                {
                    Debug.LogError("No instance of " + typeof(T).Name + " found in scene.");
                }
                    
                return instance_;
            }
        }

        public static bool IsInitialized => instance_ != null;
        
        private void Awake()
        {
            if (instance_ == null)
            {
                instance_ = this as T;

                if (isPersistent && Application.isPlaying)
                    DontDestroyOnLoad(gameObject);

            }
            else if (instance_ != this)
                Destroy(gameObject);
        }

    }
}

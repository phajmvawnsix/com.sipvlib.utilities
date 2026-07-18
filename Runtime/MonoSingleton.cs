using SiPVLib.Debugging;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace SiPVLib.Utilities
{
    /// <summary>
    /// A generic singleton class for MonoBehaviours.
    /// Ensures that only one instance of the specified component exists in the scene.
    /// </summary>
    /// <typeparam name="T">The type of the singleton component.</typeparam>
#if ODIN_INSPECTOR
    // With Odin installed, private/non-serialized fields on subclasses are also serialized via Odin's serializer.
    public abstract class MonoSingleton<T> : SerializedMonoBehaviour where T : MonoSingleton<T>
#else
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
#endif
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static T _instance;

        [Header("Singleton Settings")]
#if ODIN_INSPECTOR
        [FoldoutGroup("Singleton Settings", Expanded = false)]
#endif
        [SerializeField]
        [Tooltip("If true, the singleton instance will not be destroyed when loading a new scene.")]
        protected bool _dontDestroyOnLoad = true;

        /// <summary>
        /// Gets a value indicating whether to automatically find an existing instance in the scene.
        /// </summary>
        protected virtual bool AutoFind { get; } = true;

        /// <summary>
        /// Gets a value indicating whether to create a new instance if none is found.
        /// </summary>
        protected virtual bool CreateIfNotFound { get; } = false;

        /// <summary>
        /// Gets the singleton instance.
        /// If the instance does not exist, it may try to find it or create it based on settings.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (AppVariables.AppQuitting)
                {
                    CustomLog.LogWarning($"[MonoSingleton] Instance of {typeof(T)} already destroyed. Returning null.");
                    return null;
                }

                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    var tempInstance = CreateTempInstanceForSettings();
                    var shouldCreate = tempInstance != null && tempInstance.CreateIfNotFound;

                    if (tempInstance != null)
                    {
                        DestroyImmediate(tempInstance.gameObject);
                    }

                    if (shouldCreate)
                    {
                        CreateNewInstance();
                    }
                }
                else
                {
                    _instance.InitializeSingleton();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an instance currently exists.
        /// </summary>
        public static bool HasInstance => _instance != null && !AppVariables.AppQuitting;

        /// <summary>
        /// Gets the existing instance if available, otherwise returns null.
        /// Does not attempt to create or find the instance.
        /// </summary>
        /// <returns>The existing instance or null.</returns>
        public static T GetExistingInstance()
        {
            return AppVariables.AppQuitting ? null : _instance;
        }

        /// <summary>
        /// Unity Awake method.
        /// Initializes the singleton.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        /// <summary>
        /// Unity OnDestroy method.
        /// Clears the singleton instance if this component is the instance.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// Called when the singleton instance is initialized.
        /// Override this method to perform initialization logic.
        /// </summary>
        protected virtual void OnSingletonInitialized()
        {
        }

        /// <summary>
        /// Unity OnApplicationQuit method.
        /// Sets the ApplicationIsQuitting flag to prevent recreation of the singleton during shutdown.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            if (!AppVariables.AppQuitting)
            {
                AppVariables.AppQuitting = true;
            }
        }

        /// <summary>
        /// Initializes the singleton instance.
        /// Handles duplicate instances and DontDestroyOnLoad.
        /// </summary>
        private void InitializeSingleton()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }

                OnSingletonInitialized();
            }
            else if (_instance != this)
            {
                CustomLog.LogWarning($"[MonoSingleton] Multiple instances of {typeof(T)} detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Creates a new GameObject with the component T attached and assigns it as the instance.
        /// </summary>
        private static void CreateNewInstance()
        {
            var singletonObject = new GameObject(typeof(T).Name);
            _instance = singletonObject.AddComponent<T>();

            CustomLog.Log($"[MonoSingleton] Created new instance of {typeof(T)}");
        }

        /// <summary>
        /// Creates a temporary hidden instance to read the default settings (like CreateIfNotFound).
        /// This is necessary because the settings are virtual properties on the instance.
        /// </summary>
        /// <returns>The temporary instance, or null if creation failed.</returns>
        private static T CreateTempInstanceForSettings()
        {
            try
            {
                var tempObject = new GameObject("TempSingleton")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                return tempObject.AddComponent<T>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Destroys the singleton instance if it exists.
        /// </summary>
        public static void DestroyInstance()
        {
            if (_instance == null) return;

            if (Application.isPlaying)
            {
                Destroy(_instance.gameObject);
            }
            else
            {
                DestroyImmediate(_instance.gameObject);
            }

            _instance = null;
        }
    }
}
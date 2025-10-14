using UnityEngine;

/// <summary>
/// Provides a simple reusable singleton pattern for MonoBehaviours that should persist
/// between scenes. Handles duplicate instances and exposes a strongly typed instance
/// reference for derived classes.
/// </summary>
/// <typeparam name="T">Concrete MonoBehaviour implementing the singleton.</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Strongly typed accessor for the singleton instance.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// Indicates whether this component represents the active singleton instance.
    /// Useful for aborting additional initialisation when duplicates are destroyed.
    /// </summary>
    protected bool IsPrimaryInstance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            IsPrimaryInstance = false;
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        IsPrimaryInstance = true;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (IsPrimaryInstance && Instance == this)
        {
            Instance = null;
        }
    }
}

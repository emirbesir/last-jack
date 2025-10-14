using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

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
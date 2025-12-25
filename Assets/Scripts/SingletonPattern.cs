using UnityEngine;

public class SingletonPattern<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}

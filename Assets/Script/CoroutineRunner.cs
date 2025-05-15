using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        if (Instance == null)
        {
            var obj = new GameObject("CoroutineRunner");
            Instance = obj.AddComponent<CoroutineRunner>();
        }
    }
}

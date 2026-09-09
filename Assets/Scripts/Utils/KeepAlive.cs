using UnityEngine;
using VContainer;


public class KeepAlive : MonoBehaviour
{

    public static KeepAlive Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        else { Instance = this; }

        DontDestroyOnLoad(gameObject);

    }

}

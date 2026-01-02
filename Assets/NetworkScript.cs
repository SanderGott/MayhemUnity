using UnityEngine;
using Unity.Netcode;

public class NetworkScript : MonoBehaviour
{
    private void Awake()
    {
        // If another NetworkManager already exists, destroy this one
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.gameObject != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}

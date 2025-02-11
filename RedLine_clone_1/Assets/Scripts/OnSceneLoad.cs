using UnityEngine;
using UnityEngine.Events;

public class OnSceneLoad : MonoBehaviour
{
    public UnityEvent onSceneLoaded;

    private void OnLevelWasLoaded(int level)
    {
        onSceneLoaded.Invoke();
    }
}

using UnityEngine;

public class AppStateManager : MonoBehaviour
{
    public static bool AppIsClosing { get; private set; }

    void Awake()
    {
        AppIsClosing = false;
    }

    void OnApplicationQuit()
    {
        AppIsClosing = true;
    }
}

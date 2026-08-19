using UnityEngine;

public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        BetterStreamingAssets.Initialize();

        Debug.Log(
            "[Launcher] " +
            "BetterStreamingAssets initialized."
        );
    }
}
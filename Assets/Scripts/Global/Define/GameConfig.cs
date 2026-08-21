using UnityEngine;

public static class GameConfig
{
    // =========================
    // Debug
    // =========================
    public const bool SHOW_FPS = true;
    public const bool SHOW_GIZMOS = true;

    public const float FPS_UPDATE_INTERVAL = 0.5f;

    public const int FPS_GOOD = 120;
    public const int FPS_WARNING = 60;

    // =========================
    // Monster
    // =========================
    public const int MAX_MONSTER = 100;

    // =========================
    // World
    // =========================
    public const float WORLD_SIZE = 500f;
    public const float CHUNK_SIZE = 50f;

}
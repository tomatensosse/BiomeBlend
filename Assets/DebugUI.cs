using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public bool showMenu = false;
    public Player player;

    private float fps;
    private float deltaTime;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!showMenu) { return; }

        GUI.color = Color.white;

        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;

        Rect backgroundRect = new Rect(10, 10, 250, 120);
        GUI.Box(backgroundRect, GUIContent.none);

        float msec = deltaTime * 1000.0f;
        fps = 1f / deltaTime;
        string text = $"FPS: {fps:0.0} ({msec:0.0} ms)\n";

        if (!World.DataLoaded)
        {
            text += "Waiting for world data to be loaded...\n";
        }

        if (World.DataLoaded && player == null)
        {
            text += "Assign a player to debug.";
        }

        if (World.DataLoaded && player != null)
        {
            text += $"R-ChunkPosition: {player.GetRelativeChunkPosition()}\n" +
            $"MegaChunkPosition: {player.GetMegaChunkPosition()}\n";
        }

        GUI.Label(new Rect(20, 20, 220, 100), text, style);
    }
}

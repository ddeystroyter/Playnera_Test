using UnityEngine;
using System.Collections;

// An FPS counter.
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
public class FPSCounter : MonoBehaviour
{
    public static FPSCounter Instance;

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames;
    private float fps;

    private void Awake()
    {
        if (Instance != null) {Destroy(Instance); Instance = null; }
        Instance = this;
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 24; // Увеличиваем размер шрифта
        labelStyle.normal.textColor = Color.red; // Устанавливаем цвет текста на красный

        // Отображаем FPS на экране с использованием стиля
        GUILayout.Label("FPS: " + fps.ToString("f2"), labelStyle);
    }

    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
using UnityEngine;

public class PathRecorder : MonoBehaviour
{
    [Tooltip("Ассет, в который будет записываться путь")]
    [SerializeField] private PathData pathData;

    public bool IsRecording { get; private set; } = false;

    private void FixedUpdate()
    {
        if (IsRecording)
        {
            pathData.positions.Add(transform.position);
            pathData.rotations.Add(transform.rotation);
        }
    }

    /// <summary>
    /// Начинает запись пути, предварительно очистив старые данные.
    /// </summary>
    public void StartRecording()
    {
        if (pathData == null)
        {
            Debug.LogError("PathData не назначен в инспекторе!");
            return;
        }
        pathData.Clear();
        IsRecording = true;
        Debug.Log("Запись пути началась.");
    }

    /// <summary>
    /// Останавливает запись пути.
    /// </summary>
    public void StopRecording()
    {
        IsRecording = false;
        Debug.Log("Запись пути остановлена. Всего точек: " + pathData.positions.Count);
    }
}

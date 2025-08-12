using UnityEngine;

public class GhostController : MonoBehaviour
{
    private PathData pathData;
    private int currentPointIndex;
    private float timeBetweenPoints;
    private float timer;
    private bool isRunning = false;

    private const float PlaybackSpeedMultiplier = 1.0f;

    private void Update()
    {
        if (!isRunning || pathData == null || pathData.positions.Count == 0)
            return;

        timer += Time.deltaTime;

        // Процент прохождения отрезка между точками
        float percentage = timer / timeBetweenPoints;

        if (currentPointIndex + 1 < pathData.positions.Count)
        {
            // Линейная интерполяция позиции и поворота для плавности
            transform.position = Vector3.Lerp(pathData.positions[currentPointIndex], pathData.positions[currentPointIndex + 1], percentage);
            transform.rotation = Quaternion.Slerp(pathData.rotations[currentPointIndex], pathData.rotations[currentPointIndex + 1], percentage);
        }

        if (timer >= timeBetweenPoints)
        {
            timer = 0;
            currentPointIndex++;

            if (currentPointIndex >= pathData.positions.Count - 1)
            {
                isRunning = false;

                transform.position = pathData.positions[pathData.positions.Count - 1];
                transform.rotation = pathData.rotations[pathData.rotations.Count - 1];
                Debug.Log("Призрак закончил маршрут.");
            }
        }
    }

    /// <summary>
    /// Запускает движение призрака по заданному пути.
    /// </summary>
    public void StartGhost(PathData pathToFollow)
    {
        this.pathData = pathToFollow;
        this.currentPointIndex = 0;
        this.timer = 0;
        // время на прохождение одного сегмента
        // равным времени записи (Time.fixedDeltaTime)
        this.timeBetweenPoints = Time.fixedDeltaTime / PlaybackSpeedMultiplier;
        this.isRunning = true;

        transform.position = pathData.positions[0];
        transform.rotation = pathData.rotations[0];
    }

    public void StopGhost()
    {
        isRunning = false;
    }
}

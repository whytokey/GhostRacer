using UnityEngine;
using TMPro;
using System.Collections;
using Ashsvp;

public class GameManager : MonoBehaviour
{
    public enum GameState { WaitingToStart, Countdown, FirstRun, BetweenRuns, SecondRun, RaceFinished }

    [Header("Ссылки на объекты сцены")]
    [SerializeField] private GameObject playerCar;
    [SerializeField] private GameObject ghostCarPrefab;
    [SerializeField] private PathData pathData;

    [Header("Ссылки на элементы UI")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private TextMeshProUGUI startButtonText;

    private SimcadeVehicleController playerController;
    private Rigidbody playerRigidbody;
    private PathRecorder playerRecorder;

    private GameObject ghostInstance;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private GameState stateAfterCountdown;
    public GameState CurrentState { get; private set; }

    private void OnEnable() { FinishLine.OnPlayerCrossedFinish += HandleFinishCrossed; }
    private void OnDisable() { FinishLine.OnPlayerCrossedFinish -= HandleFinishCrossed; }

    private void Start()
    {
        playerRecorder = playerCar.GetComponent<PathRecorder>();
        playerController = playerCar.GetComponent<SimcadeVehicleController>();
        playerRigidbody = playerCar.GetComponent<Rigidbody>();

        startPosition = playerCar.transform.position;
        startRotation = playerCar.transform.rotation;

        ChangeState(GameState.WaitingToStart);
    }

    private void ResetPlayerCar()
    {
        playerCar.transform.position = startPosition;
        playerCar.transform.rotation = startRotation;

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        playerCar.SetActive(true);
    }

    private void HandleFinishCrossed()
    {
        if (playerController != null) playerController.CanDrive = false;

        if (CurrentState == GameState.FirstRun)
        {
            playerRecorder.StopRecording();
            ChangeState(GameState.BetweenRuns);
        }
        else if (CurrentState == GameState.SecondRun)
        {
            ChangeState(GameState.RaceFinished);
        }
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.WaitingToStart:
                statusText.text = "Готов к заезду?";
                startButtonText.text = "СТАРТ";
                startButton.SetActive(true);
                ResetPlayerCar();
                if (playerController != null) playerController.CanDrive = false;
                if (ghostInstance != null) Destroy(ghostInstance);
                break;

            case GameState.Countdown:
                startButton.SetActive(false);
                if (playerController != null) playerController.CanDrive = false;
                StartCoroutine(CountdownCoroutine());
                break;

            case GameState.FirstRun:
                statusText.text = "Заезд 1: Запись маршрута";
                playerRecorder.StartRecording();
                break;
            case GameState.BetweenRuns:
                statusText.text = "Маршрут записан!";
                startButtonText.text = "ГОНКА С ПРИЗРАКОМ";
                startButton.SetActive(true);
                playerCar.SetActive(false);
                break;
            case GameState.SecondRun:
                statusText.text = "Заезд 2: Обгони себя!";
                if (ghostInstance == null) SpawnAndRunGhost();
                break;
            case GameState.RaceFinished:
                statusText.text = "Гонка окончена!";
                startButtonText.text = "РЕСТАРТ";
                startButton.SetActive(true);
                playerCar.SetActive(false);
                if (playerController != null) playerController.CanDrive = false;
                if (ghostInstance?.GetComponent<GhostController>() != null)
                {
                    ghostInstance.GetComponent<GhostController>().StopGhost();
                }
                break;
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        if (stateAfterCountdown == GameState.SecondRun)
        {
            SpawnAndRunGhost();
            if (ghostInstance != null) ghostInstance.GetComponent<GhostController>().StopGhost();
        }

        statusText.text = "3"; yield return new WaitForSeconds(1f);
        statusText.text = "2"; yield return new WaitForSeconds(1f);
        statusText.text = "1"; yield return new WaitForSeconds(1f);
        statusText.text = "GO!";

        if (playerController != null) playerController.CanDrive = true;

        if (ghostInstance != null && stateAfterCountdown == GameState.SecondRun)
        {
            ghostInstance.GetComponent<GhostController>().StartGhost(pathData);
        }

        ChangeState(stateAfterCountdown);
    }

    public void OnStartButtonPressed()
    {
        ResetPlayerCar();

        if (CurrentState == GameState.WaitingToStart || CurrentState == GameState.RaceFinished)
        {
            stateAfterCountdown = GameState.FirstRun;
            ChangeState(GameState.Countdown);
        }
        else if (CurrentState == GameState.BetweenRuns)
        {
            stateAfterCountdown = GameState.SecondRun;
            ChangeState(GameState.Countdown);
        }
    }

    private void SpawnAndRunGhost()
    {
        if (pathData.positions.Count == 0) return;
        if (ghostInstance != null) Destroy(ghostInstance);
        ghostInstance = Instantiate(ghostCarPrefab, pathData.positions[0], pathData.rotations[0]);
    }
}
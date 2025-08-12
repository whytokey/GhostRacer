using UnityEngine;

public class PathRecorder : MonoBehaviour
{
    [Tooltip("�����, � ������� ����� ������������ ����")]
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
    /// �������� ������ ����, �������������� ������� ������ ������.
    /// </summary>
    public void StartRecording()
    {
        if (pathData == null)
        {
            Debug.LogError("PathData �� �������� � ����������!");
            return;
        }
        pathData.Clear();
        IsRecording = true;
        Debug.Log("������ ���� ��������.");
    }

    /// <summary>
    /// ������������� ������ ����.
    /// </summary>
    public void StopRecording()
    {
        IsRecording = false;
        Debug.Log("������ ���� �����������. ����� �����: " + pathData.positions.Count);
    }
}

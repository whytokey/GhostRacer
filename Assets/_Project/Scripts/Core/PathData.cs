using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewPath", menuName = "Ghost Racer/Path Data")]
public class PathData : ScriptableObject
{
    public List<Vector3> positions;
    public List<Quaternion> rotations;

    /// <summary>
    /// ќчищает все записанные данные дл€ нового заезда.
    /// </summary>
    public void Clear()
    {
        positions.Clear();
        rotations.Clear();
    }
}

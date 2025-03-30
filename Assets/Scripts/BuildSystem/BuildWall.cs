using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallDirection
{
    Forward,
    Backward,
    Left,
    Right
}

public class BuildWall : MonoBehaviour
{
    public WallDirection direction;

    public Transform upCheck;
    public Transform downCheck;
    public Transform leftCheck;
    public Transform rightCheck;

}

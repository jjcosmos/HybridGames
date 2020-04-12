using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    public int x;
    public int y;

    [HideInInspector]
    public Vector3 center;
    static Vector3 offset;

    public Vector3 GetCenter()
    {
        return transform.position + offset;
    }
}

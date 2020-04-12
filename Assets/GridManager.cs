using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    List<GridSpace> myGridSpaces;
    void Awake()
    {
        myGridSpaces = transform.GetComponentsInChildren<GridSpace>().ToList<GridSpace>();
    }

    public GridSpace GetGridSpaceAt(int x, int y)
    {
        foreach (var gridspace in myGridSpaces)
        {
            if(gridspace.x == x && gridspace.y == y)
            {
                return gridspace;
            }
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridMoveTool
{
    Vector3Int maxGrid;
    public void UpdateGrid(Vector3 _mapSize, Vector3 _basis)
    {
        Vector3 targetVec = new Vector3(_mapSize.x / _basis.x, _mapSize.y / _basis.y, _mapSize.z / _basis.z);
        maxGrid = Vector3Int.FloorToInt(targetVec);
    }
    public void CalcGrid(Vector3 _pos)
    {

    }
}

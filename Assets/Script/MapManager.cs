using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Vector3Int mapSize = new Vector3Int(2, 2, 2);
    [SerializeField]
    private Vector3Int basis = new Vector3Int(4, 4, 4);
    [SerializeField]
    private Vector3Int gizmosOffset = new Vector3Int(0, 2, 0);
    [SerializeField]
    private Vector3Int topoOffset = new Vector3Int(-2, 0, -2);
    [SerializeField]
    private Vector3Int selection = Vector3Int.zero;

    [SerializeField]
    private Enums.TopoTags topoSelection;

    [SerializeField]
    private List<GameObject> topoList = new List<GameObject>();
    public void topoGenerate()
    {
        foreach (GameObject target in topoList)
        {
            if (target.CompareTag(topoSelection.ToString()) == true && topoSelection != Enums.TopoTags.Untagged)
            {
                GameObject go = target;
                go.transform.position = selection;
                Instantiate(go);

                topoSelection = Enums.TopoTags.Untagged;
            }
        }
    }
    private void mapSizeGizmos()
    {
        basis.Clamp(Vector3Int.one, Vector3Int.one * 10);
        int basisX = basis.x;
        int basisY = basis.y;
        int basisZ = basis.z;

        int offsetX = (int)gizmosOffset.x;
        int offsetY = (int)gizmosOffset.y;
        int offsetZ = (int)gizmosOffset.z;

        for (int x = offsetX; x < Math.Clamp(mapSize.x, 1, 20) * 4; x += basisX)
        {
            for (int y = offsetY; y < Math.Clamp(mapSize.y, 1, 20) * 4; y += basisY)
            {
                for (int z = offsetZ; z < Math.Clamp(mapSize.z, 1, 20) * 4; z += basisZ)
                {
                    Gizmos.DrawWireCube(new Vector3(x, y, z), new Vector3(basisX, basisY, basisY));
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        mapSizeGizmos();
    }
}
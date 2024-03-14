using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
public class MapManager : MonoBehaviour
{
    static MapManager()
    {
        EditorApplication.update += Update;
    }
    enum tags
    {
        wall,
        floor,
        pillar,
        stair,
    }
    [SerializeField]
    private string tagName = "topo";
    [SerializeField]
    private Vector3 mapSize;
    [SerializeField]
    private Vector3 basis = new Vector3 (4, 4, 4);
    [SerializeField]
    private Vector3 gizmosOffset = new Vector3 (0, 2, 0);
    [SerializeField]
    private Vector3 topoOffset = new Vector3 (-2, 0, -2);

    private void mapSizeGizmos()
    {
        int basisX = (int)basis.x;
        int basisY = (int)basis.y;
        int basisZ = (int)basis.z;

        int offsetX = (int)gizmosOffset.x;
        int offsetY = (int)gizmosOffset.y;
        int offsetZ = (int)gizmosOffset.z;

        for (int x = offsetX; x < mapSize.x * 4; x+=basisX)
        {
            for(int y = offsetY; y < mapSize.y * 4; y+=basisY)
            {
                for (int z = offsetZ; z < mapSize.z * 4; z+=basisZ)
                {
                    Gizmos.DrawWireCube(new Vector3(x,y,z), basis);
                }
            }
        }
    }
    private static void topographyManage(MapManager _map)
    {
        Selection.selectionChanged += () =>
        {
            List<string> tagList = Enum.GetNames(typeof(tags)).ToList();
            GameObject[] objList = Selection.gameObjects;
            foreach (GameObject targetObj in objList)
            {
                string targetTag = targetObj.gameObject.tag;
                if (tagList.Contains(targetTag) == true)
                {
                    Vector3 offset = Vector3.zero;
                    float angle = targetObj.transform.rotation.eulerAngles.y;
                    if (targetTag == tags.wall.ToString())
                    {
                        if((int)(angle/90) % 2 == 0)
                            offset = new Vector3(_map.topoOffset.x, 0, 0);
                        else
                            offset = new Vector3(0, 0, _map.topoOffset.z);
                    }
                    if(targetTag == tags.pillar.ToString())
                    {
                        offset = _map.topoOffset;
                    }
                    else
                    {
                        offset = -_map.topoOffset;
                    }
                    Vector3 targetPos = (targetObj.transform.position + offset) / 4;
                    if (targetPos.x > _map.mapSize.x || targetPos.y > _map.mapSize.y || targetPos.z > _map.mapSize.z
                        || targetPos.x < 0 || targetPos.y < 0 || targetPos.z < 0)
                        targetObj.SetActive(false);
                    else
                        targetObj.SetActive(true);
                }
            }
        };
    }
    private static void topographyMove()
    {
        GameObject[] targetList = Selection.gameObjects;
    }
    private void OnDrawGizmos()
    {
        mapSizeGizmos();
    }
    static void Update()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager == null)
        {
            GameObject go = new GameObject();
            mapManager = go.AddComponent<MapManager>();
            go.name = typeof(MapManager).Name;
        }
        topographyManage(mapManager);
    }
}

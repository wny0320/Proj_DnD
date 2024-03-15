using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Trash : MonoBehaviour
{
    //private void a()
    //{
    //    Selection.selectionChanged += () =>
    //    {
    //        List<string> tagList = Enum.GetNames(typeof(Enums.TopoTags)).ToList();
    //        GameObject[] objList = Selection.gameObjects;
    //        foreach (GameObject targetObj in objList)
    //        {
    //            string targetTag = targetObj.gameObject.tag;
    //            if (tagList.Contains(targetTag) == true)
    //            {
    //                Vector3 offset = Vector3.zero;
    //                float angle = targetObj.transform.rotation.eulerAngles.y;
    //                if (targetTag == Enums.TopoTags.wall.ToString())
    //                {
    //                    if ((int)(angle / 90) % 2 == 0)
    //                        offset = new Vector3(Global.mapManager.topoOffset.x, 0, 0);
    //                    else
    //                        offset = new Vector3(0, 0, Global.mapManager.topoOffset.z);
    //                }
    //                if (targetTag == Enums.TopoTags.pillar.ToString())
    //                {
    //                    offset = Global.mapManager.topoOffset;
    //                }
    //                else
    //                {
    //                    offset = -Global.mapManager.topoOffset;
    //                }
    //                Vector3 targetPos = (targetObj.transform.position + offset) / 4;
    //                if (targetPos.x > Global.mapManager.mapSize.x || targetPos.y > Global.mapManager.mapSize.y || targetPos.z > Global.mapManager.mapSize.z
    //                    || targetPos.x < 0 || targetPos.y < 0 || targetPos.z < 0)
    //                    targetObj.SetActive(false);
    //                else
    //                    targetObj.SetActive(true);
    //            }
    //        }
    //    };
    //}
}

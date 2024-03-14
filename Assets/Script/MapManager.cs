using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Reflection;
using Unity.VisualScripting;

[InitializeOnLoad]
public class MapManager : MonoBehaviour
{
    [SerializeField]
    private string tagName = "topo";
    [SerializeField]
    private Vector3 mapSize;
    [SerializeField]
    private Vector3 basis = new Vector3 (4, 4, 4);
    [SerializeField]
    private Vector3 offset = new Vector3 (0, 2, 0);
    private List<GameObject> topographyList = new List<GameObject> ();

    private void mapSizeGizmos()
    {
        int basisX = (int)basis.x;
        int basisY = (int)basis.y;
        int basisZ = (int)basis.z;

        int offsetX = (int)offset.x;
        int offsetY = (int)offset.y;
        int offsetZ = (int)offset.z;

        for (int x = offsetX; x < mapSize.x; x+=basisX)
        {
            for(int y = offsetY; y < mapSize.y; y+=basisY)
            {
                for (int z = offsetZ; z < mapSize.z; z+=basisZ)
                {
                    Gizmos.DrawWireCube(new Vector3(x,y,z), basis);
                }
            }
        }
    }
    private void topograpyManage()
    {
        EditorApplication.hierarchyChanged += () =>
        {
            GameObject[] selectObj = Selection.gameObjects;
            foreach (GameObject sel in selectObj)
            {
                if (topographyList.Contains(sel))
                    continue;
                if(sel.CompareTag(tagName) == true)
                    topographyList.Add(sel);
            }
            foreach (GameObject topo in topographyList)
            {
                if(topo == null)
                    topographyList.Remove(topo);
            }
        };
    }
    private void indecentTopograpyMange()
    {
        foreach (GameObject topo in topographyList)
        {
            bool outsideFlag = false;
            if(transform.position.x > mapSize.x || transform.position.y > mapSize.y || transform.position.z > mapSize.z)
                outsideFlag = true;
            if(outsideFlag == true)
            {
                topo.SetActive(false);
            }
            else
                topo.SetActive(true);
        }
    }
    private void OnDrawGizmos()
    {
        mapSizeGizmos();
    }
    private void Awake()
    {
        EditorApplication.update += Update;
    }
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    
    // Update is called once per frame
    private void Update()
    {
        Debug.Log("!");
        topograpyManage();
        indecentTopograpyMange();
    }
}

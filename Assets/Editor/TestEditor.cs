using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Codice.Client.BaseCommands;
using System;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEditor.IMGUI.Controls;

[CanEditMultipleObjects]
[CustomEditor(typeof(MapManager))]
public class TestEditor : Editor
{
    SerializedProperty mapSizeProperty;
    SerializedProperty basisProperty;
    SerializedProperty gizmosOffsetProperty;
    SerializedProperty topoOffsetProperty;
    SerializedProperty selectionProperty;
    SerializedProperty topoSelectionProperty;
    SerializedProperty topoListProperty;

    Vector3 snap;
    public override void OnInspectorGUI()
    {
        var component = target as MapManager;
        var transform = component.transform;

        serializedObject.Update();
        EditorGUILayout.PropertyField(mapSizeProperty);
        EditorGUILayout.PropertyField(basisProperty);
        EditorGUILayout.PropertyField(gizmosOffsetProperty);
        EditorGUILayout.PropertyField(topoOffsetProperty);
        EditorGUILayout.PropertyField(selectionProperty);
        EditorGUILayout.PropertyField(topoSelectionProperty);
        EditorGUILayout.PropertyField(topoListProperty);

        Vector3Int intVecPos = Vector3Int.FloorToInt(transform.position);
        intVecPos.Clamp(Vector3Int.zero, mapSizeProperty.vector3IntValue - Vector3Int.one);
        transform.position = intVecPos;
        Vector3Int gridPos = basisProperty.vector3IntValue * Vector3Int.FloorToInt(transform.position);
        selectionProperty.vector3IntValue = gridPos;
        component.topoGenerate();
        //topoGenerate(selectionProperty.vector3IntValue, basisProperty.vector3IntValue, topoSelectionProperty.En, topoListProperty)
        serializedObject.ApplyModifiedProperties();
    }
    //private void topoGenerate(Vector3Int _selection, Vector3Int _basis, Enums.TopoTags _topoTags, List<GameObject> _topoList)
    //{
    //    foreach (GameObject target in _topoList)
    //    {
    //        if (target.CompareTag(_topoTags.ToString()) == true)
    //        {
    //            GameObject go = target;
    //            go.transform.position = _selection;
    //            Instantiate(go);
    //        }
    //    }
    //}
    /// <summary>
    /// Custom Handler
    /// </summary>
    /// <param name="_transform">Handler's Transform</param>
    private Vector3 PositionHandle(Transform _transform)
    {
        var position = _transform.position;
        var handlerPos = _transform.position + new Vector3Int(basisProperty.vector3IntValue.x / 2, 0, basisProperty.vector3IntValue.z / 2);

        //해당 위치에 사각형 그리기
        //색 입히는 코드
        Handles.color = Color.red;
        Handles.DrawWireCube(selectionProperty.vector3IntValue + gizmosOffsetProperty.vector3IntValue, basisProperty.vector3IntValue);

        //Handles.PositionHandle(position, _transform.rotation);

        //축별로 그리기
        Handles.color = Handles.xAxisColor;
        position = Handles.Slider(position, _transform.right, 1, Handles.ArrowHandleCap, snap.x); // X축
        Handles.color = Handles.yAxisColor;
        position = Handles.Slider(position, _transform.up, 1, Handles.ArrowHandleCap, snap.y); // Y축
        Handles.color = Handles.zAxisColor;
        position = Handles.Slider(position, _transform.forward, 1, Handles.ArrowHandleCap, snap.z); // Z축

        position = Vector3Int.FloorToInt(position);
        return position;
    }
    private void OnEnable()
    {
        mapSizeProperty = serializedObject.FindProperty("mapSize");
        basisProperty = serializedObject.FindProperty("basis");
        gizmosOffsetProperty = serializedObject.FindProperty("gizmosOffset");
        topoOffsetProperty = serializedObject.FindProperty("topoOffset");
        selectionProperty = serializedObject.FindProperty("selection");
        topoSelectionProperty = serializedObject.FindProperty("topoSelection");
        topoListProperty = serializedObject.FindProperty("topoList");

        var snapX = EditorPrefs.GetFloat("MoveSnapX", 1f);
        var snapY = EditorPrefs.GetFloat("MoveSnapY", 1f);
        var snapZ = EditorPrefs.GetFloat("MoveSnapZ", 1f);
        snap = new Vector3(snapX, snapY, snapZ);
    }
    private void OnSceneGUI()
    {
        Tools.current = Tool.None;
        var component = target as MapManager;
        var transform = component.transform;

        transform.position = PositionHandle(component.transform);
    }
}

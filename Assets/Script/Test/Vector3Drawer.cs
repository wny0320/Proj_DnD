using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector3Attribute))]
internal sealed class Vector3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Vector3Attribute vec3Attr = (Vector3Attribute)attribute;
        if(property.propertyType == SerializedPropertyType.Vector3)
        {
            Vector3 resultVec = property.vector3Value;
            float resultX = Math.Clamp(resultVec.x, vec3Attr.minVec.x, vec3Attr.maxVec.x);
            float resultY = Math.Clamp(resultVec.y, vec3Attr.minVec.y, vec3Attr.maxVec.y);
            float resultZ = Math.Clamp(resultVec.z, vec3Attr.minVec.z, vec3Attr.maxVec.z);

            property.vector2Value = new Vector3(resultX, resultY, resultZ);

            EditorGUI.Vector3Field(position, label, vec3Attr.minVec);
            EditorGUI.Vector3Field(position, label, vec3Attr.maxVec);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}

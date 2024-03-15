using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class Vector3Attribute : PropertyAttribute
{
    public readonly Vector3 minVec;
    public readonly Vector3 maxVec;


    public Vector3Attribute(Vector3 _min, Vector3 _max)
    {
        minVec = _min;
        maxVec = _max;
    }
}

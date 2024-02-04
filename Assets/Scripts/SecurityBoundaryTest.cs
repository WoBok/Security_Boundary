using System.Collections.Generic;
using UnityEngine;

public class SecurityBoundaryTest : MonoBehaviour
{
    [SerializeField]
    List<Transform> points = new List<Transform>();
    [SerializeField]
    Transform sceneCenter;
    void Awake()
    {
        var polygonPoints = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i].position;
            polygonPoints[i] = new Vector2(p.x, p.z);
        }
        SecurityBoundaryManager.Open(polygonPoints, 0.45f, sceneCenter);
    }
}
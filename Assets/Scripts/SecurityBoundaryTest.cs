using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SecurityBoundaryTest : MonoBehaviour
{
    [SerializeField]
    List<Transform> points = new List<Transform>();
    [SerializeField]
    Transform point;

    /// <summary>
    /// 给定构成多边形的有序点集polygonPoints和点p，计算点p与各条边的垂直距离
    /// </summary>
    /// <param name="polygonPoints">构成多边形的有序点集</param>
    /// <param name="p">需要计算的点</param>
    void CheckDistanceToPolygonEdges(Vector2[] polygonPoints, Vector2 p)
    {
        Vector2 pA;
        Vector2 pB;
        for (int i = 0; i < polygonPoints.Length; i++)
        {
            pA = polygonPoints[i];
            pB = polygonPoints[(i + 1) % polygonPoints.Length];

           SecurityBoundaryAlgorithm.GetDistanceToLineSegment(pA, pB, p);
        }
    }
    void OnDrawGizmos()
    {
        Test();
    }
    void Test()
    {
        var polygonPoints = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            var pTA = points[i].position;
            var pTB = points[(i + 1) % points.Count].position;
            Vector3 pA = new Vector3(pTA.x, 0, pTA.z);
            Vector3 pB = new Vector3(pTB.x, 0, pTB.z);
            polygonPoints[i] = new Vector2(pTA.x, pTA.z);
            Gizmos.DrawLine(pA, pB);
            Handles.Label(pA, points[i].name);
        }

        var p = new Vector2(point.position.x, point.position.z);
        var isInBoundary = SecurityBoundaryAlgorithm.IsPointInPolygon(polygonPoints, p);

        var materialColor = isInBoundary ? new Color(1, 0, 0, 0.25f) : Color.green;
        point.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", materialColor);

        if (isInBoundary)
        {
            CheckDistanceToPolygonEdges(polygonPoints, p);
        }
    }
}
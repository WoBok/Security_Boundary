using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SBChecker : MonoBehaviour
{
    [SerializeField]
    List<Transform> points = new List<Transform>();
    [SerializeField]
    Transform point;
    /// <summary>
    /// 给定构成多边形的有序点集polygonPoints和点p，判断点p是否在多边形内
    /// </summary>
    /// <param name="polygonPoints">构成多边形的有序点集</param>
    /// <param name="p">需要判断的点</param>
    /// <returns>点p是否在多边形内</returns>
    bool IsPointInPolygon(Vector2[] polygonPoints, Vector2 p)
    {
        Vector2 pA;
        Vector2 pB;
        int intersectionCount = 0;
        for (int i = 0; i < polygonPoints.Length; i++)
        {
            pA = polygonPoints[i];
            pB = polygonPoints[(i + 1) % polygonPoints.Length];

            if (pA.y == pB.y) continue;
            if (p.y >= Mathf.Max(pA.y, pB.y)) continue;
            if (p.y < Mathf.Min(pA.y, pB.y)) continue;

            var intersectionPx = pA.x + (p.y - pA.y) * (pB.x - pA.x) / (pB.y - pA.y);
            if (intersectionPx > p.x) intersectionCount++;
        }
        return intersectionCount % 2 != 0;
    }
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

            var isProjected = GetDistanceToLineSegment(pA, pB, p, out float distance);
            Debug.Log($"isProjected: {isProjected}, distance: {distance}");
        }
    }
    /// <summary>
    /// 判断点是否垂直投影在由点pA和点pB构成的线段上，并计算投影到线段上的垂直距离
    /// </summary>
    /// <param name="pA">线段端点A</param>
    /// <param name="pB">线段端点B</param>
    /// <param name="p">需要判断的点</param>
    /// <param name="distance">点p投影到线段上的垂直距离</param>
    /// <returns></returns>
    bool GetDistanceToLineSegment(Vector2 pA, Vector2 pB, Vector2 p, out float distance)
    {
        var vecProjected = p - pA;
        var vecEdge = pB - pA;

        var dirFactor = Vector2.Dot(vecProjected, vecEdge) / Vector2.Dot(vecEdge, vecEdge);
        if (0 < dirFactor && dirFactor <= 1)
        {
            var vecProject = dirFactor * vecEdge;
            var pointProject = pA + vecProject;
            distance = Vector3.Distance(p, pointProject);

            ///////////////////////////////////Visualization///////////////////////////////////
            Debug.DrawLine(new Vector3(p.x, 0, p.y), new Vector3(pointProject.x, 0, pointProject.y));
            var labelPosition = p + (pointProject - p) / 2;
            GUI.color = Color.white;
            Handles.Label(new Vector3(pointProject.x, 0, pointProject.y - 0.05f), pointProject.ToString());
            GUI.color = Color.yellow;
            Handles.Label(new Vector3(labelPosition.x, 0, labelPosition.y + 0.05f), distance.ToString());
            ///////////////////////////////////Visualization///////////////////////////////////

            return true;
        }
        distance = float.PositiveInfinity;
        return false;
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
        var isInBoundary = IsPointInPolygon(polygonPoints, p);

        var materialColor = isInBoundary ? new Color(1, 0, 0, 0.25f) : Color.green;
        point.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", materialColor);

        if (isInBoundary)
        {
            CheckDistanceToPolygonEdges(polygonPoints, p);
        }
    }
}
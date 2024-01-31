using UnityEditor;
using UnityEngine;

public class SecurityBoundaryAlgorithm
{
    /// <summary>
    /// 给定构成多边形的有序点集polygonPoints和点p，判断点p是否在多边形内
    /// </summary>
    /// <param name="polygonPoints">构成多边形的有序点集</param>
    /// <param name="p">需要判断的点</param>
    /// <returns>点p是否在多边形内</returns>
    public static bool IsPointInPolygon(Vector2[] polygonPoints, Vector2 p)
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
    /// 判断点是否垂直投影在由点pA和点pB构成的线段上，并计算投影到线段上的垂直距离
    /// </summary>
    /// <param name="pA">线段端点A</param>
    /// <param name="pB">线段端点B</param>
    /// <param name="p">需要判断的点</param>
    /// <returns>点p投影到线段上的垂直距离</returns>
    public static float GetDistanceToLineSegment(Vector2 pA, Vector2 pB, Vector2 p)
    {
        var vecProjected = p - pA;
        var vecEdge = pB - pA;

        var dirFactor = Vector2.Dot(vecProjected, vecEdge) / Vector2.Dot(vecEdge, vecEdge);
        if (0 < dirFactor && dirFactor <= 1)
        {
            var vecProject = dirFactor * vecEdge;
            var pointProject = pA + vecProject;
            var distance = Vector3.Distance(p, pointProject);

#if UNITY_EDITOR
            ///////////////////////////////////Visualization///////////////////////////////////
            Debug.DrawLine(new Vector3(p.x, 0, p.y), new Vector3(pointProject.x, 0, pointProject.y));
            var labelPosition = p + (pointProject - p) / 2;
            GUI.color = Color.white;
            Handles.Label(new Vector3(pointProject.x, 0, pointProject.y - 0.05f), pointProject.ToString());
            GUI.color = Color.yellow;
            Handles.Label(new Vector3(labelPosition.x, 0, labelPosition.y + 0.05f), distance.ToString());
            ///////////////////////////////////Visualization///////////////////////////////////  
#endif

            return distance;
        }
        return float.PositiveInfinity;
    }
}
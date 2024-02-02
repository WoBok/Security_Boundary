using UnityEngine;
public struct ProjectedPointInfo
{
    public string tag;
    public float projectedDistance;
    public Vector2 projectedPointPosition;
    public Vector2 projectedLineDirection;
}
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
    /// <returns>点p投影到线段上的相关信息</returns>
    public static ProjectedPointInfo GetProjectedPointToLineSegmentInfo(Vector2 pA, Vector2 pB, Vector2 p)
    {
        var projectedVec = p - pA;
        var projectedEdge = pB - pA;
        var dirFactor = Vector2.Dot(projectedVec, projectedEdge) / Vector2.Dot(projectedEdge, projectedEdge);

        var projectedPointInfo = new ProjectedPointInfo();
        if (0 < dirFactor && dirFactor <= 1)
        {
            var projectionVec = dirFactor * projectedEdge;
            var projectedPointPosition = pA + projectionVec;
            var projectedDistance = Vector3.Distance(p, projectedPointPosition);

            projectedPointInfo.tag = projectedEdge.ToString();
            projectedPointInfo.projectedDistance = projectedDistance;
            projectedPointInfo.projectedLineDirection = projectedEdge;
            projectedPointInfo.projectedPointPosition = projectedPointPosition;
            return projectedPointInfo;
        }
        projectedPointInfo.tag = "";
        projectedPointInfo.projectedDistance = float.PositiveInfinity;
        projectedPointInfo.projectedLineDirection = Vector2.zero;
        projectedPointInfo.projectedPointPosition = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        return projectedPointInfo;
    }
}
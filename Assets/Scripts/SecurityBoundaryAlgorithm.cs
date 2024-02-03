using UnityEngine;
public struct MiniDistanceInfo
{
    public string tag;
    public float minDistance;//点到线段的距离
    public Vector2 nearestPoint;//点到直线上最近的点
    public Vector2 projectedLineDirection;//被投影线段的方向
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
    /// 计算点到线段的最短距离
    /// </summary>
    /// <param name="pA">线段端点A</param>
    /// <param name="pB">线段端点B</param>
    /// <param name="p">需要判断的点</param>
    /// <returns>点p到线段的最短距离的相关信息</returns>
    public static MiniDistanceInfo GetMinDistanceInfo(Vector2 pA, Vector2 pB, Vector2 p)
    {
        var projectedVector = p - pA;
        var projectedLine = pB - pA;
        var directionFactor = Vector2.Dot(projectedVector, projectedLine) / Vector2.Dot(projectedLine, projectedLine);

        var miniDistanceInfo = new MiniDistanceInfo();
        miniDistanceInfo.tag = projectedLine.ToString();
        miniDistanceInfo.projectedLineDirection = projectedLine;

        if (directionFactor < 0)
        {
            var minDistance = Vector2.Distance(p, pA);
            miniDistanceInfo.minDistance = minDistance;
            miniDistanceInfo.nearestPoint = pA;
        }

        if (0 < directionFactor && directionFactor <= 1)
        {
            var projectionVector = directionFactor * projectedLine;
            var nearestPoint = pA + projectionVector;
            var minDistance = Vector2.Distance(p, nearestPoint);

            miniDistanceInfo.minDistance = minDistance;
            miniDistanceInfo.nearestPoint = nearestPoint;
        }

        if (directionFactor > 1)
        {
            var minDistance = Vector2.Distance(p, pB);
            miniDistanceInfo.minDistance = minDistance;
            miniDistanceInfo.nearestPoint = pB;
        }

        return miniDistanceInfo;
    }
}
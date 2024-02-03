using UnityEngine;
public struct MiniDistanceInfo
{
    public string tag;
    public float minDistance;//�㵽�߶εľ���
    public Vector2 nearestPoint;//�㵽ֱ��������ĵ�
    public Vector2 projectedLineDirection;//��ͶӰ�߶εķ���
}
public class SecurityBoundaryAlgorithm
{
    /// <summary>
    /// �������ɶ���ε�����㼯polygonPoints�͵�p���жϵ�p�Ƿ��ڶ������
    /// </summary>
    /// <param name="polygonPoints">���ɶ���ε�����㼯</param>
    /// <param name="p">��Ҫ�жϵĵ�</param>
    /// <returns>��p�Ƿ��ڶ������</returns>
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
    /// ����㵽�߶ε���̾���
    /// </summary>
    /// <param name="pA">�߶ζ˵�A</param>
    /// <param name="pB">�߶ζ˵�B</param>
    /// <param name="p">��Ҫ�жϵĵ�</param>
    /// <returns>��p���߶ε���̾���������Ϣ</returns>
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
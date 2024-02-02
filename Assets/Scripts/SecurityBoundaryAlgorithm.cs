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
    /// �жϵ��Ƿ�ֱͶӰ���ɵ�pA�͵�pB���ɵ��߶��ϣ�������ͶӰ���߶��ϵĴ�ֱ����
    /// </summary>
    /// <param name="pA">�߶ζ˵�A</param>
    /// <param name="pB">�߶ζ˵�B</param>
    /// <param name="p">��Ҫ�жϵĵ�</param>
    /// <returns>��pͶӰ���߶��ϵ������Ϣ</returns>
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
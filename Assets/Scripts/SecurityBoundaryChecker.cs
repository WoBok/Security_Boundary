using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SecurityBoundaryChecker : MonoBehaviour
{
    public Action OnEnteredSecurityBoundary;
    public Action OnExitedSecurityBoundary;
    public Action<List<ProjectedPointInfo>> OnNearedSecurityBoundary;
    public Action OnMovedAwayFromSecurityBoundary;

    public Vector2[] boundaryVertices;
    public float nearestDistanceToBoundary = 0.5f;

    Camera m_MainCamera;
    Camera MainCamera { get { if (m_MainCamera == null) m_MainCamera = Camera.main; return m_MainCamera; } }
    Vector2 PlayerPosition { get { var camPos = MainCamera.transform.position; return new Vector2(camPos.x, camPos.z); } }

    bool m_IsPlayerInBoundary;

    bool m_IsEnteredBoundary;
    bool m_IsExitedBoundary;

    bool m_IsNearedBoundary;
    bool m_IsMovedAwayFromBoundary;

    List<ProjectedPointInfo> m_ProjectedPointInfos = new List<ProjectedPointInfo>();
#if  UNITY_EDITOR
    List<ProjectedPointInfo> m_ProjectedPointInfosGizmos = new List<ProjectedPointInfo>();
#endif
    void IsPlayerInBoundary()
    {
        m_IsPlayerInBoundary = SecurityBoundaryAlgorithm.IsPointInPolygon(boundaryVertices, PlayerPosition);
        if (m_IsPlayerInBoundary)
        {
            EnteringSecurityBoundary();
            IsNearBoundary();
        }
        else
        {
            ExitingSecurityBoundary();
        }
    }
    void EnteringSecurityBoundary()
    {
        if (!m_IsEnteredBoundary)
        {
            m_IsEnteredBoundary = true;
            m_IsExitedBoundary = false;
            OnEnteredSecurityBoundary?.Invoke();
        }
    }
    void ExitingSecurityBoundary()
    {
        if (!m_IsExitedBoundary)
        {
            m_IsEnteredBoundary = false;
            m_IsExitedBoundary = true;
            OnExitedSecurityBoundary?.Invoke();
        }
    }
    void IsNearBoundary()
    {
        GetNearestPointInfos();
        if (m_ProjectedPointInfos.Count > 0)
        {
            NearingSecurityBoundary();
        }
        else
        {
            MovingAwayFromSecurityBoundary();
        }
    }
    void GetNearestPointInfos()
    {
        m_ProjectedPointInfos.Clear();
#if UNITY_EDITOR
        m_ProjectedPointInfosGizmos.Clear();
#endif
        for (int i = 0; i < boundaryVertices.Length; i++)
        {
            var pA = boundaryVertices[i];
            var pB = boundaryVertices[(i + 1) % boundaryVertices.Length];
            var projectedPointInfo = SecurityBoundaryAlgorithm.GetProjectedPointToLineSegmentInfo(pA, pB, PlayerPosition);
#if UNITY_EDITOR
            m_ProjectedPointInfosGizmos.Add(projectedPointInfo);
#endif
            if (projectedPointInfo.projectedDistance < nearestDistanceToBoundary)
            {
                m_ProjectedPointInfos.Add(projectedPointInfo);
            }
        }
    }
    void NearingSecurityBoundary()
    {
        if (!m_IsNearedBoundary)
        {
            m_IsMovedAwayFromBoundary = false;
            OnNearedSecurityBoundary?.Invoke(m_ProjectedPointInfos);
        }
    }
    void MovingAwayFromSecurityBoundary()
    {
        if (!m_IsMovedAwayFromBoundary)
        {
            m_IsMovedAwayFromBoundary = true;
            OnMovedAwayFromSecurityBoundary?.Invoke();
        }
    }
    void Update()
    {
        IsPlayerInBoundary();
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        for (int i = 0; i < boundaryVertices.Length; i++)
        {
            var pTA = boundaryVertices[i];
            var pTB = boundaryVertices[(i + 1) % boundaryVertices.Length];
            Vector3 pA = new Vector3(pTA.x, 0, pTA.y);
            Vector3 pB = new Vector3(pTB.x, 0, pTB.y);
            Debug.DrawLine(pA, pB);
            Handles.Label(pA + Vector3.right * 0.05f, $"P{i}");
        }
        if (m_IsPlayerInBoundary)
        {
            for (int i = 0; i < m_ProjectedPointInfosGizmos.Count; i++)
            {
                var pointInfo = m_ProjectedPointInfosGizmos[i];
                var playerPosition = new Vector2(MainCamera.transform.position.x, MainCamera.transform.position.z);
                Gizmos.DrawLine(new Vector3(playerPosition.x, 0, playerPosition.y), new Vector3(pointInfo.projectedPointPosition.x, 0, pointInfo.projectedPointPosition.y));
                GUI.color = Color.white;
                Handles.Label(new Vector3(pointInfo.projectedPointPosition.x, 0, pointInfo.projectedPointPosition.y - 0.05f), pointInfo.projectedPointPosition.ToString());
                var labelPosition = playerPosition + (pointInfo.projectedPointPosition - playerPosition) / 2;
                GUI.color = Color.yellow;
                Handles.Label(new Vector3(labelPosition.x, 0, labelPosition.y + 0.05f), pointInfo.projectedDistance.ToString());
            }
        }
    }
#endif
}
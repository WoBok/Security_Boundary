using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SecurityBoundaryChecker : MonoBehaviour
{
    public Action OnEnteredSecurityBoundary;
    public Action OnExitedSecurityBoundary;
    public Action<List<MiniDistanceInfo>> OnNearedSecurityBoundary;
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

    List<MiniDistanceInfo> m_MiniDistanceInfos = new List<MiniDistanceInfo>();
#if  UNITY_EDITOR
    List<MiniDistanceInfo> m_MiniDistanceInfosGizmos = new List<MiniDistanceInfo>();
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
        if (m_MiniDistanceInfos.Count > 0)
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
        m_MiniDistanceInfos.Clear();
#if UNITY_EDITOR
        m_MiniDistanceInfosGizmos.Clear();
#endif
        for (int i = 0; i < boundaryVertices.Length; i++)
        {
            var pA = boundaryVertices[i];
            var pB = boundaryVertices[(i + 1) % boundaryVertices.Length];
            var projectedPointInfo = SecurityBoundaryAlgorithm.GetMinDistanceInfo(pA, pB, PlayerPosition);
#if UNITY_EDITOR
            m_MiniDistanceInfosGizmos.Add(projectedPointInfo);
#endif
            if (projectedPointInfo.minDistance < nearestDistanceToBoundary)
            {
                m_MiniDistanceInfos.Add(projectedPointInfo);
            }
        }
    }
    void NearingSecurityBoundary()
    {
        if (!m_IsNearedBoundary)
        {
            m_IsMovedAwayFromBoundary = false;
            OnNearedSecurityBoundary?.Invoke(m_MiniDistanceInfos);
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
            var pVA = boundaryVertices[i];
            var pVB = boundaryVertices[(i + 1) % boundaryVertices.Length];
            Vector3 pA = new Vector3(pVA.x, 0, pVA.y);
            Vector3 pB = new Vector3(pVB.x, 0, pVB.y);
            Debug.DrawLine(pA, pB);
            Handles.Label(pA + Vector3.right * 0.05f, $"{pVA}");
        }
        if (m_IsPlayerInBoundary)
        {
            for (int i = 0; i < m_MiniDistanceInfosGizmos.Count; i++)
            {
                var pointInfo = m_MiniDistanceInfosGizmos[i];
                var playerPosition = new Vector2(MainCamera.transform.position.x, MainCamera.transform.position.z);
                Gizmos.DrawLine(new Vector3(playerPosition.x, 0, playerPosition.y), new Vector3(pointInfo.nearestPoint.x, 0, pointInfo.nearestPoint.y));
                GUI.color = Color.white;
                Handles.Label(new Vector3(pointInfo.nearestPoint.x, 0, pointInfo.nearestPoint.y - 0.05f), pointInfo.nearestPoint.ToString());
                var labelPosition = playerPosition + (pointInfo.nearestPoint - playerPosition) / 2;
                GUI.color = Color.yellow;
                Handles.Label(new Vector3(labelPosition.x, 0, labelPosition.y + 0.05f), pointInfo.minDistance.ToString());
            }
        }
        Handles.Label(new Vector3(PlayerPosition.x, 0, PlayerPosition.y), PlayerPosition.ToString());
    }
#endif
}
using UnityEngine;

public class SecurityBoundaryManager
{
    static SecurityBoundaryManager m_Instance;
    static SecurityBoundaryManager Instance { get { if (m_Instance == null) m_Instance = new SecurityBoundaryManager(); return m_Instance; } }
    SecurityBoundaryDisplay m_BoundaryDisplay;
    SecurityBoundaryChecker m_BoundaryChecker;

    public static void Open()
    {
        Instance.CreateBoundaryChecker();
        Instance.CreateBoundaryDisplay();
    }
    void CreateBoundaryChecker()
    {
        if (m_BoundaryChecker == null)
        {
            var boundaryCheckerObj = new GameObject("BoundaryChecker");
            Object.DontDestroyOnLoad(boundaryCheckerObj);
            m_BoundaryChecker = boundaryCheckerObj.AddComponent<SecurityBoundaryChecker>();
        }
    }
    void CreateBoundaryDisplay()
    {
        if (m_BoundaryDisplay == null)
        {
            var boundaryCheckerObj = new GameObject("BoundaryDisplay");
            Object.DontDestroyOnLoad(boundaryCheckerObj);
            m_BoundaryDisplay = boundaryCheckerObj.AddComponent<SecurityBoundaryDisplay>();
        }
        if (m_BoundaryChecker != null)
        {
            m_BoundaryChecker.OnEnteredSecurityBoundary += m_BoundaryDisplay.EnteredBoundary;
            m_BoundaryChecker.OnExitedSecurityBoundary += m_BoundaryDisplay.ExitedBoundary;
            m_BoundaryChecker.OnNearedSecurityBoundary += m_BoundaryDisplay.NearedBoundary;
            m_BoundaryChecker.OnMovedAwayFromSecurityBoundary += m_BoundaryDisplay.MovedAwayFromBoundary;
        }
    }
    public static void Close()
    {
        Instance.DestroyBoundaryChecker();
        Instance.DestroyBoundaryDisplay();
    }
    void DestroyBoundaryChecker()
    {
        if (m_BoundaryChecker != null)
        {
            Object.Destroy(m_BoundaryChecker.gameObject);
            m_BoundaryChecker.OnEnteredSecurityBoundary = null;
            m_BoundaryChecker.OnExitedSecurityBoundary = null;
            m_BoundaryChecker.OnNearedSecurityBoundary = null;
            m_BoundaryChecker.OnMovedAwayFromSecurityBoundary = null;
            m_BoundaryChecker = null;
        }
    }
    void DestroyBoundaryDisplay()
    {
        if (m_BoundaryDisplay != null)
        {
            Object.Destroy(m_BoundaryDisplay.gameObject);
            m_BoundaryDisplay = null;
        }
    }
    public static void Init(Vector2[] boundaryVertices, float minDistanceToBoundary, Transform sceneCenterTrans)
    {
        UpdateBoundaryPoints(boundaryVertices);
        UpdateNearestDistanceToBoundary(minDistanceToBoundary);
        UpdateSceneCenterTrans(sceneCenterTrans);
    }
    public static void UpdateBoundaryPoints(Vector2[] boundaryVertices)
    {
        Instance.m_BoundaryChecker.boundaryVertices = boundaryVertices;
    }
    public static void UpdateNearestDistanceToBoundary(float nearestDistanceToBoundary)
    {
        Instance.m_BoundaryChecker.nearestDistanceToBoundary = nearestDistanceToBoundary;
    }
    public static void UpdateSceneCenterTrans(Transform sceneCenterTrans)
    {
        Instance.m_BoundaryDisplay.sceneCenterTrans = sceneCenterTrans;
    }
}
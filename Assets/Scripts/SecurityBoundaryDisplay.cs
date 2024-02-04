using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.PXR;
using UnityEngine;

public class SecurityBoundaryDisplay : MonoBehaviour
{
    [HideInInspector]
    public Transform sceneCenterTrans;

    GameObject m_SecurityAreaEffect;
    GameObject m_SecurityArrawEffect;
    GameObject m_SecurityBoundaryUI;

    Dictionary<string, GameObject> m_BoundaryEffects = new Dictionary<string, GameObject>();

    Camera m_MainCamera;
    Camera MainCamera { get { if (m_MainCamera == null) m_MainCamera = Camera.main; return m_MainCamera; } }

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    ///EnteredBoundary
    public void EnteredBoundary()
    {
        CloseSecurityAreaEffect();
        CloseExitedBoundaryEffect();
        OpenAllBoundaryEffect();
        SwitchVSTState(false);
    }
    void CloseSecurityAreaEffect()
    {
        if (m_SecurityAreaEffect != null)
        {
            Destroy(m_SecurityAreaEffect);
            m_SecurityAreaEffect = null;
        }
    }
    void CloseExitedBoundaryEffect()
    {
        StopCoroutine(ExitedBoundaryEffect());
        CloseSecurityArrawEffect();
        CloseSecurityBoundaryUI();
    }
    void CloseSecurityArrawEffect()
    {
        if (m_SecurityArrawEffect != null)
        {
            Destroy(m_SecurityArrawEffect);
            m_SecurityArrawEffect = null;
        }
    }
    void CloseSecurityBoundaryUI()
    {
        if (m_SecurityBoundaryUI != null)
        {
            Destroy(m_SecurityBoundaryUI);
            m_SecurityBoundaryUI = null;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    ///ExitedBoundary
    public void ExitedBoundary()
    {
        OpenSecurityAreaEffect();
        StartCoroutine(ExitedBoundaryEffect());
        CloseAllBoundaryEffect();
        SwitchVSTState(true);
    }
    void OpenSecurityAreaEffect()
    {
        if (m_SecurityAreaEffect == null)
        {
            m_SecurityAreaEffect = LoadPrefab("SecurityAreaEffect");
            m_SecurityAreaEffect.transform.position = sceneCenterTrans.position;
            m_SecurityAreaEffect.transform.rotation = sceneCenterTrans.rotation;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    ///EnteredBoundary&ExitedBoundary
    void OpenAllBoundaryEffect()
    {
        foreach (var b in m_BoundaryEffects)
        {
            b.Value.SetActive(true);
        }
    }
    void CloseAllBoundaryEffect()
    {
        foreach (var b in m_BoundaryEffects)
        {
            b.Value.SetActive(false);
        }
    }
    IEnumerator ExitedBoundaryEffect()
    {
        CreateSecurityArrawEffect();
        CreateSecurityBoundaryUI();
        while (true)
        {
            DisplayArrowEffect();
            DisplayUI();
            yield return null;
        }
    }
    void CreateSecurityArrawEffect()
    {
        if (m_SecurityArrawEffect == null)
        {
            m_SecurityArrawEffect = LoadPrefab("SecurityArrowEffect");
        }
    }
    void CreateSecurityBoundaryUI()
    {
        if (m_SecurityBoundaryUI == null)
        {
            m_SecurityBoundaryUI = LoadPrefab("SecurityBoundaryUI");
            m_SecurityBoundaryUI.transform.position = GetUITargetPosition();
            var direction = m_SecurityBoundaryUI.transform.position - MainCamera.transform.position;
            m_SecurityBoundaryUI.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    void DisplayArrowEffect()
    {
        if (m_SecurityArrawEffect != null)
        {
            var direction = sceneCenterTrans.transform.position - MainCamera.transform.position;
            direction.y = 0;
            var playerPosition = new Vector3(MainCamera.transform.position.x, 0, MainCamera.transform.position.z);
            m_SecurityArrawEffect.transform.position = playerPosition + direction.normalized + Vector3.up * 0.1f;
            m_SecurityArrawEffect.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    void DisplayUI()
    {
        if (m_SecurityBoundaryUI != null)
        {
            var uiTargetPosition = GetUITargetPosition();
            var direction = m_SecurityBoundaryUI.transform.position - MainCamera.transform.position;
            var uiTargetRotation = Quaternion.LookRotation(direction);
            m_SecurityBoundaryUI.transform.rotation = Quaternion.Lerp(m_SecurityBoundaryUI.transform.rotation, uiTargetRotation, Time.deltaTime * 2.5f);
            m_SecurityBoundaryUI.transform.position = Vector3.Lerp(m_SecurityBoundaryUI.transform.position, uiTargetPosition, Time.deltaTime * 3);
        }
    }
    Vector3 GetUITargetPosition()
    {
        var position = MainCamera.transform.position;
        position.y = 1f;
        var positionOffset = MainCamera.transform.forward;
        positionOffset.y = 0;
        return position + positionOffset;
    }
    void SwitchVSTState(bool state)
    {
        PXR_MixedReality.EnableVideoSeeThrough(state);
        PXR_MixedReality.EnableVideoSeeThroughEffect(state);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Brightness, state ? -35 : 0, state ? 3 : 0);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    ///NearedBoundary
    public void NearedBoundary(List<MiniDistanceInfo> pointsInfo)
    {
        DestroyUselessEffect(pointsInfo);
        UpdateBoundaryEffect(pointsInfo);
    }

    List<string> uselessBoundaryEffectTags = new List<string>();
    void DestroyUselessEffect(List<MiniDistanceInfo> distanceInfo)
    {
        uselessBoundaryEffectTags.Clear();
        foreach (var b in m_BoundaryEffects)
        {
            if (!distanceInfo.Exists(p => p.tag == b.Key))
            {
                uselessBoundaryEffectTags.Add(b.Key);
            }
        }
        foreach (var tag in uselessBoundaryEffectTags)
        {
            Destroy(m_BoundaryEffects[tag]);
            m_BoundaryEffects.Remove(tag);
        }
    }
    void UpdateBoundaryEffect(List<MiniDistanceInfo> pointsInfo)
    {
        foreach (var p in pointsInfo)
        {
            var targetPosition = new Vector3(p.nearestPoint.x, MainCamera.transform.position.y, p.nearestPoint.y);
            var targetPositionOffset = (targetPosition - MainCamera.transform.position).normalized * 0.1f;
            targetPosition += targetPositionOffset;
            if (!m_BoundaryEffects.ContainsKey(p.tag))
            {
                var boundaryEffect = LoadPrefab("BoundaryEffect");
                m_BoundaryEffects[p.tag] = boundaryEffect;
                m_BoundaryEffects[p.tag].transform.position = targetPosition;
            }
            m_BoundaryEffects[p.tag].transform.position = Vector3.Lerp(m_BoundaryEffects[p.tag].transform.position, targetPosition, Time.deltaTime * 2);
            m_BoundaryEffects[p.tag].transform.rotation = Quaternion.LookRotation(new Vector3(p.projectedLineDirection.x, 0, p.projectedLineDirection.y));
            var eulerAngles = m_BoundaryEffects[p.tag].transform.eulerAngles;
            m_BoundaryEffects[p.tag].transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y - 90, eulerAngles.z);
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    ///MovedAwayFromBoundary
    public void MovedAwayFromBoundary()
    {
        foreach (var b in m_BoundaryEffects)
        {
            Destroy(b.Value);
        }
        m_BoundaryEffects.Clear();
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    GameObject LoadPrefab(string name)
    {
        return Instantiate(Resources.Load<GameObject>("SecurityBoundary/" + name));
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////
}
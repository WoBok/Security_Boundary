using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class SecurityBoundaryDisplay : MonoBehaviour
{
    [HideInInspector]
    public Transform sceneCenterTrans;
    GameObject m_SecurityAreaEffect;
    GameObject m_SecurityArrawEffect;

    Dictionary<string, GameObject> m_BoundaryEffects = new Dictionary<string, GameObject>();

    Camera m_MainCamera;
    Camera MainCamera { get { if (m_MainCamera == null) m_MainCamera = Camera.main; return m_MainCamera; } }
    public void EnteredBoundary()
    {
        CloseExitedBoundaryEffect();
        SwitchVSTState(false);
    }
    void CloseExitedBoundaryEffect()
    {
        if (m_SecurityAreaEffect != null)
        {
            Destroy(m_SecurityAreaEffect);
            m_SecurityAreaEffect = null;
        }
        StopCoroutine(SwitchArrowEffect());
        if (m_SecurityArrawEffect != null)
        {
            Destroy(m_SecurityArrawEffect);
            m_SecurityArrawEffect = null;
        }
        OpenAllBoundaryEffect();
    }
    public void ExitedBoundary()
    {
        OpenExitedBoundaryEffect();
        SwitchVSTState(true);
    }
    void OpenExitedBoundaryEffect()
    {
        if (m_SecurityAreaEffect == null)
        {
            m_SecurityAreaEffect = Instantiate(Resources.Load<GameObject>("SecurityAreaEffect"));
            m_SecurityAreaEffect.transform.position = sceneCenterTrans.position;
            m_SecurityAreaEffect.transform.rotation = sceneCenterTrans.rotation;
        }
        StartCoroutine(SwitchArrowEffect());
        CloseAllBoundaryEffect();
    }
    void SwitchVSTState(bool state)
    {
        PXR_MixedReality.EnableVideoSeeThrough(state);
        PXR_MixedReality.EnableVideoSeeThroughEffect(state);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Brightness, state ? -35 : 0, state ? 3 : 0);
    }
    IEnumerator SwitchArrowEffect()
    {
        if (m_SecurityArrawEffect == null)
        {
            m_SecurityArrawEffect = Instantiate(Resources.Load<GameObject>("SecurityArrowEffect"));
        }
        while (true)
        {
            var direction = sceneCenterTrans.transform.position - MainCamera.transform.position;
            m_SecurityArrawEffect.transform.position = MainCamera.transform.position + MainCamera.transform.forward - Vector3.up * 0.5f;//ÐÞ¸Ä
            m_SecurityArrawEffect.transform.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }
    }
    public void NearedBoundary(List<ProjectedPointInfo> pointsInfo)
    {
        DestroyUselessEffect(pointsInfo);
        UpdateBoundaryEffect(pointsInfo);
    }

    List<string> uselessBoundaryEffectTags = new List<string>();
    void DestroyUselessEffect(List<ProjectedPointInfo> pointsInfo)
    {
        uselessBoundaryEffectTags.Clear();
        foreach (var b in m_BoundaryEffects)
        {
            if (!pointsInfo.Exists(p => p.tag == b.Key))
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
    void UpdateBoundaryEffect(List<ProjectedPointInfo> pointsInfo)
    {
        foreach (var p in pointsInfo)
        {
            var targetPosition = new Vector3(p.projectedPointPosition.x, MainCamera.transform.position.y, p.projectedPointPosition.y);
            if (!m_BoundaryEffects.ContainsKey(p.tag))
            {
                var boundaryEffect = Instantiate(Resources.Load<GameObject>("BoundaryEffect"));
                m_BoundaryEffects[p.tag] = boundaryEffect;
                m_BoundaryEffects[p.tag].transform.position = targetPosition;
            }
            m_BoundaryEffects[p.tag].transform.position = Vector3.Lerp(m_BoundaryEffects[p.tag].transform.position, targetPosition, Time.deltaTime);
            m_BoundaryEffects[p.tag].transform.rotation = Quaternion.LookRotation(new Vector3(p.projectedLineDirection.x, 0, p.projectedLineDirection.y));
            var eulerAngles = m_BoundaryEffects[p.tag].transform.eulerAngles;
            m_BoundaryEffects[p.tag].transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y - 90, eulerAngles.z);
        }
    }
    public void MovedAwayFromBoundary()
    {
        foreach (var b in m_BoundaryEffects)
        {
            Destroy(b.Value);
        }
        m_BoundaryEffects.Clear();
    }
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
}
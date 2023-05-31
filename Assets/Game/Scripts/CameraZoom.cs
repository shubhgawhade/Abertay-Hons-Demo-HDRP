using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;
    public float sensitivity;

    public float minZoom;
    public float maxZoom;

    public bool clampZoom = true;

    private void Awake()
    {
        TextReader.RemoveCinemachineTarget += RemoveCinemachinTarget;
        InspectableInteractables.RemoveCinemachineTarget += RemoveCinemachinTarget;

        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            targetGroup.m_Targets[i].radius -=  Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            
            if (clampZoom)
            {
                targetGroup.m_Targets[i].radius = Mathf.Clamp(targetGroup.m_Targets[1].radius, 2.5f, 5f);
            }
        }
    }

    public void AddCinemachineTarget(Transform transform, float weight, float radius)
    {
        targetGroup.AddMember(transform, weight, radius);
    }

    public void RemoveCinemachinTarget(Transform t)
    {
        targetGroup.RemoveMember(t);
    }

    private void OnDisable()
    {
        TextReader.RemoveCinemachineTarget -= RemoveCinemachinTarget;
        InspectableInteractables.RemoveCinemachineTarget -= RemoveCinemachinTarget;
    }
}
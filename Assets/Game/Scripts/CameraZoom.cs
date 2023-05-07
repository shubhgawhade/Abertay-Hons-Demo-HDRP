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

    private void Awake()
    {
        TextReader.RemoveCinemachineTarget += RemoveCinemachinTarget;
        InspectableInteractables.RemoveCinemachineTarget += RemoveCinemachinTarget;

        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Update()
    {
        targetGroup.m_Targets[0].radius -=  Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        targetGroup.m_Targets[1].radius -=  Input.GetAxis("Mouse ScrollWheel") * sensitivity;
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
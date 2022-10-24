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
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Update()
    {
        targetGroup.m_Targets[0].radius -=  Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        targetGroup.m_Targets[1].radius -=  Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        
        // Mathf.Clamp()
    }
}

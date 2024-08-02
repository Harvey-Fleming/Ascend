using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

public class CameraSwapTrigger : MonoBehaviour
{
    public CustomCameraInspectorObjects cameraInspectorObjects;

    BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (cameraInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.OnPanCamera(cameraInspectorObjects.panDistance, cameraInspectorObjects.panTime, cameraInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - boxCollider.bounds.center).normalized;

            if(cameraInspectorObjects.swapCameras && cameraInspectorObjects.cameraOnLeft != null && cameraInspectorObjects.cameraOnRight != null)
            {
                CameraManager.instance.SwapCamera(cameraInspectorObjects.cameraOnLeft, cameraInspectorObjects.cameraOnRight, exitDirection);
            }

            if (cameraInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.OnPanCamera(cameraInspectorObjects.panDistance, cameraInspectorObjects.panTime, cameraInspectorObjects.panDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CustomCameraInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection
{ 
    Up,
    Down,
    Left,
    Right
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraSwapTrigger))]
public class MyScriptEditor : Editor
{
    CameraSwapTrigger cameraSwapTrigger;

    private void OnEnable()
    {
        cameraSwapTrigger = (CameraSwapTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraSwapTrigger.cameraInspectorObjects.swapCameras)
        {
            cameraSwapTrigger.cameraInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraSwapTrigger.cameraInspectorObjects.cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraSwapTrigger.cameraInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraSwapTrigger.cameraInspectorObjects.cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraSwapTrigger.cameraInspectorObjects.panCameraOnContact)
        {
            cameraSwapTrigger.cameraInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraSwapTrigger.cameraInspectorObjects.panDirection);
            cameraSwapTrigger.cameraInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraSwapTrigger.cameraInspectorObjects.panDistance);
            cameraSwapTrigger.cameraInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraSwapTrigger.cameraInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraSwapTrigger);
        }
    }
} 
#endif




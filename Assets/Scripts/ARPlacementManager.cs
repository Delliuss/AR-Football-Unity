using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARPlacementManager : MonoBehaviour
{
    [Header("AR & Prefabs")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject playerPrefab;


    private enum SelectedType { None, Goal, Ball, Player }
    private SelectedType currentMode = SelectedType.None;

    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        Touch touch = Touch.activeTouches[0];

        // Проверяем, не попал ли палец в UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.finger.index))
        {
            return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            if (currentMode == SelectedType.None)
            {
                return;
            }

            if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SetObject(hitPose);
            }
        }
    }

    private void SetObject(Pose hitPose)
    {
        if (currentMode == SelectedType.Goal && goalPrefab != null)
            Instantiate(goalPrefab, hitPose.position, hitPose.rotation);
        else if (currentMode == SelectedType.Ball && ballPrefab != null)
            Instantiate(ballPrefab, hitPose.position, hitPose.rotation);
        else if (currentMode == SelectedType.Player && playerPrefab != null)
            Instantiate(playerPrefab, hitPose.position, hitPose.rotation);

    }



    public void SelectGoal()
    {
        currentMode = SelectedType.Goal;
    }

    public void SelectBall()
    {
        currentMode = SelectedType.Ball;
    }

    public void SelectPlayer()
    {
        currentMode = SelectedType.Player;
    }
}

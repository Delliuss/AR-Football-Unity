using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
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

    [Header("UI")]
    [SerializeField] private Slider rotationSlider;

    private enum SelectedType { None, Goal, Ball, Player }
    private SelectedType currentMode = SelectedType.None;

    private GameObject spawnedGoal;
    private GameObject spawnedBall;
    private GameObject spawnedPlayer;
    private GameObject activeObject;

    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    private void Start()
    {
        if (rotationSlider != null)
        {
            rotationSlider.minValue = 0f;
            rotationSlider.maxValue = 360f;
            rotationSlider.onValueChanged.AddListener(RotateActiveObject);
        }
    }

    private void Update()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        Touch touch = Touch.activeTouches[0];

        if (IsTouchOverUI(touch.screenPosition))
            return;

        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
        {
            if (currentMode == SelectedType.None)
                return;

            if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SetOrMoveObject(hitPose);
            }
        }
    }

    private void SetOrMoveObject(Pose hitPose)
    {
        if (currentMode == SelectedType.Goal && goalPrefab != null)
        {
            if (spawnedGoal == null)
                spawnedGoal = Instantiate(goalPrefab, hitPose.position, hitPose.rotation);
            else
                spawnedGoal.transform.position = hitPose.position;

            activeObject = spawnedGoal;
        }
        else if (currentMode == SelectedType.Ball && ballPrefab != null)
        {
            if (spawnedBall == null)
                spawnedBall = Instantiate(ballPrefab, hitPose.position, hitPose.rotation);
            else
                spawnedBall.transform.position = hitPose.position;

            activeObject = spawnedBall;
        }
        else if (currentMode == SelectedType.Player && playerPrefab != null)
        {
            if (spawnedPlayer == null)
                spawnedPlayer = Instantiate(playerPrefab, hitPose.position, hitPose.rotation);
            else
                spawnedPlayer.transform.position = hitPose.position;

            activeObject = spawnedPlayer;
        }

        SyncSlider();
    }

    private void RotateActiveObject(float angle)
    {
        if (activeObject != null)
        {
            Vector3 currentEuler = activeObject.transform.eulerAngles;
            activeObject.transform.eulerAngles = new Vector3(currentEuler.x, angle, currentEuler.z);
        }
    }

    private void SyncSlider()
    {
        if (rotationSlider != null && activeObject != null)
        {
            rotationSlider.SetValueWithoutNotify(activeObject.transform.eulerAngles.y);
        }
    }

    public void SelectGoal()
    {
        currentMode = SelectedType.Goal;
        activeObject = spawnedGoal;
        SyncSlider();
    }

    public void SelectBall()
    {
        currentMode = SelectedType.Ball;
        activeObject = spawnedBall;
        SyncSlider();
    }

    public void SelectPlayer()
    {
        currentMode = SelectedType.Player;
        activeObject = spawnedPlayer;
        SyncSlider();
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}

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

    // Память о поставленных объектах (по одному каждого вида)
    private GameObject spawnedGoal;
    private GameObject spawnedBall;
    private GameObject spawnedPlayer;

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
        if (IsTouchOverUI(touch.screenPosition))
            return;

        // Реагируем и на нажатие (Began), и на движение пальца по экрану (Moved)
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
        {
            if (currentMode == SelectedType.None)
            {
                return;
            }

            if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SetOrMoveObject(hitPose);
            }
        }
    }

    // Если объекта ещё нет - создаём. Если уже поставлен — переставляем
    private void SetOrMoveObject(Pose hitPose)
    {
        if (currentMode == SelectedType.Goal && goalPrefab != null)
        {
            if (spawnedGoal == null)
                spawnedGoal = Instantiate(goalPrefab, hitPose.position, hitPose.rotation);
            else
                spawnedGoal.transform.position = hitPose.position;
        }
        else if (currentMode == SelectedType.Ball && ballPrefab != null)
        {
            if (spawnedBall == null)
                spawnedBall = Instantiate(ballPrefab, hitPose.position, hitPose.rotation);
            else
                spawnedBall.transform.position = hitPose.position;
        }
        else if (currentMode == SelectedType.Player && playerPrefab != null)
        {
            if (spawnedPlayer == null)
                spawnedPlayer = Instantiate(playerPrefab, hitPose.position, hitPose.rotation);
            else
                spawnedPlayer.transform.position = hitPose.position;
        }
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

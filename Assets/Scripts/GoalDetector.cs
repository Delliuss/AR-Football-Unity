using System.Collections;
using UnityEngine;
using TMPro;

public class GoalDetector : MonoBehaviour
{
    [Header("Настройки уведомления")]
    [SerializeField] private float popupDuration = 2f; // Сколько секунд висит надпись 

    private TMP_Text counterText;
    private GameObject goalPopupObject;

    private int goalsCount = 0;
    private bool isGoalScored = false;

    private void Start()
    {
        Canvas mainCanvas = FindFirstObjectByType<Canvas>();
        if (mainCanvas != null)
        {
            // 1. Текст счетчика вверху
            Transform counterTransform = mainCanvas.transform.Find("GoalCounterText");
            if (counterTransform != null)
            {
                counterText = counterTransform.GetComponent<TMP_Text>();
                UpdateCounterUI();
            }

            // 2. Надпись по центру
            Transform popupTransform = mainCanvas.transform.Find("GoalPopupText");
            if (popupTransform != null)
            {
                goalPopupObject = popupTransform.gameObject;
                goalPopupObject.SetActive(false); // Скрыта при старте
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isGoalScored && other.CompareTag("Ball"))
        {
            isGoalScored = true;
            goalsCount++;
            UpdateCounterUI();
            ShowGoalPopup();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            isGoalScored = false;
        }
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
        {
            counterText.text = "Голов забито: " + goalsCount;
        }
    }

    private void ShowGoalPopup()
    {
        if (goalPopupObject != null)
        {
            goalPopupObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HidePopupAfterDelay());
        }
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        if (goalPopupObject != null)
        {
            goalPopupObject.SetActive(false);
        }
    }
}

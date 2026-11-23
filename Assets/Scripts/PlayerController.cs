using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 1.2f;     // Скорость бега (м/с)
    [SerializeField] private float rotationSpeed = 10f;  // Скорость разворота

    private Animator animator;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Awake()
    {
        // Ищем компонент Animator на дочерней 3D-модели внутри префаба
        animator = GetComponentInChildren<Animator>();
        targetPosition = transform.position;
    }

    public void SetMoveTarget(Vector3 newTarget)
    {
        targetPosition = new Vector3(newTarget.x, transform.position.y, newTarget.z);
        hasTarget = true;
    }

    // Если игрока переставили вручную в режиме расстановки — сбрасываем старую цель бега
    public void ResetTarget()
    {
        hasTarget = false;
        targetPosition = transform.position;
        if (animator != null)
            animator.SetBool("IsRunning", false);
    }

    private void Update()
    {
        if (!hasTarget)
            return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        // Если до точки назначения больше 5 сантиметров, бежит
        if (direction.magnitude > 0.05f)
        {
            // 1. Плавно поворачиваем персонажа лицом в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 2. Двигаем персонажа к цели
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 3. Включаем анимацию бега
            if (animator != null)
                animator.SetBool("IsRunning", true);
        }
        else
        {
            hasTarget = false;
            if (animator != null)
                animator.SetBool("IsRunning", false);
        }
    }
}

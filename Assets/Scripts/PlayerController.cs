using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 1.2f;    // Скорость бега (м/с)
    [SerializeField] private float rotationSpeed = 10f; // Скорость разворота

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem dustParticles; 

    private Animator animator;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Awake()
    {
        // Ищем компонент Animator на дочерней 3D-модели внутри префаба
        animator = GetComponentInChildren<Animator>();
        targetPosition = transform.position;

        // На всякий случай выключаем пыль при старте
        if (dustParticles != null)
            dustParticles.Stop();
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
        SetRunningState(false);
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
            Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            nextPos.y = targetPosition.y;
            transform.position = nextPos;

            // 3. Включаем анимацию бега
            SetRunningState(true);
        }
        else
        {
            hasTarget = false;
            // Выключаем анимацию бега и частицы пыли
            SetRunningState(false);
        }
    }

    // Единая функция управления состоянием бега (анимация + частицы)
    private void SetRunningState(bool isRunning)
    {
        if (animator != null)
            animator.SetBool("IsRunning", isRunning);

        if (dustParticles != null)
        {
            if (isRunning && !dustParticles.isPlaying)
                dustParticles.Play();
            else if (!isRunning && dustParticles.isPlaying)
                dustParticles.Stop();
        }
    }
}

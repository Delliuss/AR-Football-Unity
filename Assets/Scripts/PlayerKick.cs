using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    [Header("Настройки удара")]
    [SerializeField] private float kickForce = 2.5f; // Сила удара по мячу

    // Срабатывает автоматически, когда коллайдер человека касается другого твёрдого коллайдера
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRigidbody = collision.rigidbody;

        if (ballRigidbody != null)
        {
            // Вычисляем направление от человека к мячу (строго по горизонтали)
            Vector3 kickDirection = collision.transform.position - transform.position;
            kickDirection.y = 0f;
            kickDirection.Normalize(); // Делаем длину вектора равной 1, чтобы сила удара всегда была одинаковой

            ballRigidbody.AddForce(kickDirection * kickForce, ForceMode.Impulse);
        }
    }
}

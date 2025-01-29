using UnityEngine;

public class HuntPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 5f;

    void FixedUpdate()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * (speed * Time.fixedDeltaTime);
        transform.LookAt(transform.position + direction);
    }
}

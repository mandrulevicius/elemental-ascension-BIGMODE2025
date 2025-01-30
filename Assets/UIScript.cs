using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{    

    [SerializeField] TextMeshProUGUI HealthText;
    [SerializeField] GameObject player;
    [SerializeField] EntityStats stats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HealthText.text= stats.Health.ToString();
    }

    void OnEnable()
    {
        stats = player.GetComponent<EntityStats>();
        stats.OnHealthChanged += HealthChangeUI;
    }

    void HealthChangeUI(float health)
    {
        HealthText.text = health.ToString();
    }
}

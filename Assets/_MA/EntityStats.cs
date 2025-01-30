using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityStats : MonoBehaviour
{
    public event Action<float> OnMaxHealthChanged;
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;

            if (Health > maxHealth)
            {
                Health = MaxHealth;
            }

            OnMaxHealthChanged?.Invoke(maxHealth);
        }
    }

    public event Action<float> OnHealthChanged;
    [SerializeField] private float health;
    
    
    [SerializeField] bool dead;

    public float Health
    {
        get => health;
        set
        {
            if (value > MaxHealth) value = MaxHealth;
            health = value;
            OnHealthChanged?.Invoke(health);

            if (health <= 0)
            {
                dead = true;
                OnDestruction?.Invoke();
                // Destroy(selfPrefab, 0f);
            }
        }
    }
    
    public event Action OnDestruction;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityStats : MonoBehaviour
{
    [SerializeField] ParticleSystem deathEffectParticles;
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
                deathEffectParticles.Play();
                Destroy(gameObject, 1f);
                // Destroy(selfPrefab, 0f);
            }
        }
    }
    
    [SerializeField] public bool dead;
    public event Action OnDeath;
    public event Action OnDestruction;
    
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

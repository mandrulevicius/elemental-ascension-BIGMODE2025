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
                if(deathEffectParticles)
                    Instantiate(deathEffectParticles, transform.position, Quaternion.identity);
                Destroy(gameObject, 0f);
                // Destroy(selfPrefab, 0f);
            }
        }
    }
    [SerializeField] public bool dead;
    public event Action OnDeath;
    public event Action OnDestruction;
    
    
    // [SerializeField] public multiplicativeModifier; // one slider to rule them all, affecting all relevant stats
    // additiveModifier
    // rageOnCondition x Friendlies died
    // furyOnCondition y Friendlies died
    // bloodlustOnCondition x Enemies killed
    // frienzyOnCondition y Enemies killed
    // reset counter
    // gaining specific energy grants specific resource.
    // on each entity death, send data to higher level layers of the program. Entity stats is mid-level local.
    // need to have a global static class for each entity's Layers death tallies.
    
    // but first, just drop a pickup that increases the stat, and have it fly at player from anywhere or closest plants from their range.
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

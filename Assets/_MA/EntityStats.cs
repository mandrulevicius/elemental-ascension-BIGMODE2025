using System;
using StarterAssets;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    // >>> The One Modifier
    // there should be multiple defaults for The Slider - thats prefabs
    [SerializeField] float multiplicativeModifier = 1f;
    
    public float MultiplicativeModifier
    {
        get => multiplicativeModifier;
        set
        {
            multiplicativeModifier = value;
            float lastMaxHealth = MaxHealth;
            MaxHealth = baseHealth * multiplicativeModifier;
            if (Mathf.Approximately(Health, lastMaxHealth)) return;
            
            float lastMovespeed = Movespeed;
            Movespeed = baseMovespeed * multiplicativeModifier;
            if (Mathf.Approximately(Movespeed, lastMovespeed)) return;
            
            damage = baseDamage * multiplicativeModifier;
            
            // transform.localScale = _baseScale * multiplicativeModifier;
            plantSpeed = BasePlantSpeed / multiplicativeModifier;
            
            OnMultiplicativeModifierChanged?.Invoke(multiplicativeModifier);
        }
    }
    public event Action<float> OnMultiplicativeModifierChanged;
    // <<< The One Modifier
    
    // entity's power
    
    private Vector3 _baseScale = new (1f, 1f, 1f);
    
    [SerializeField] ParticleSystem deathEffectParticles;
    public event Action<float> OnMaxHealthChanged;
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float BasePlantSpeed = 100f;
    [SerializeField] public float plantSpeed = 100f;
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            float previousMaxHealth = maxHealth;
            maxHealth = value;
            if (Mathf.Approximately(Health, previousMaxHealth))
            {
                Health = maxHealth;
            }
            else if (previousMaxHealth > 0f)
            {
                Health = Health / previousMaxHealth * maxHealth;
            }
            OnMaxHealthChanged?.Invoke(maxHealth);
        }
    }

    public bool bossDead;

    public event Action OnBossDeath;

    public event Action<float> OnHealthChanged;
    [SerializeField] private float health = 100f;
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
                if (gameObject.CompareTag("Boss"))
                {
                    bossDead = true;
                    OnBossDeath?.Invoke();
                    gameObject.GetComponent<StarterAssetsInputs>().ToggleMainMenu();
                    return;
                }
                if (gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    gameObject.GetComponent<StarterAssetsInputs>().ToggleMainMenu();
                    return;
                }

                if (deathEffectParticles)
                {
                    deathEffectParticles.transform.localScale *= multiplicativeModifier;
                    Instantiate(deathEffectParticles, transform.position, Quaternion.identity);
                }
                    
                if (dropPrefab)
                {
                    var drop = Instantiate(dropPrefab, transform.position + Vector3.up, Quaternion.identity);
                    drop.transform.localScale = drop.transform.localScale * (float)Math.Sqrt(multiplicativeModifier);
                    drop.GetComponent<EntityStats>().MultiplicativeModifier = MultiplicativeModifier / 100f;
                }
                Destroy(gameObject, 0.1f);
                // Destroy(selfPrefab, 0f);
            }
        }
    }
    [SerializeField] public bool dead;
    public event Action OnDeath;
    public event Action OnDestruction;

    [SerializeField] float baseMovespeed = 1f;
    [SerializeField] float movespeed = 1f;
    public float Movespeed
    {
        get => movespeed;
        set
        {
            movespeed = value;
            OnMovespeedChanged?.Invoke(movespeed);
        }
    }
    public event Action<float> OnMovespeedChanged;

    public float baseDamage = 1;
    public float damage = 1;
    
    [SerializeField] private GameObject dropPrefab;

    public int pickupsGathered = 0;
    



    void OnTriggerEnter(Collider other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Pickup"))
            {
                var pickupStats = other.gameObject.GetComponent<EntityStats>();
                if (pickupStats)
                {
                    
                    pickupsGathered += Mathf.RoundToInt(pickupStats.MultiplicativeModifier * 100);
                    MultiplicativeModifier += pickupStats.multiplicativeModifier;
                }
                Health += 1;
            }
        }

        if (gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Health = 0;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // Debug.Log(other.gameObject.name);
        if (gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            // Debug.Log($"I am :{gameObject.name}, i hit:{other.gameObject.name});");
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Friends"))
            {
                var playerStats = other.gameObject.GetComponent<EntityStats>();
                if (playerStats)
                {
                    Health -= playerStats.damage;
                    // health loss effect on health loss!
                }
            }
        }

        if (gameObject.layer == LayerMask.NameToLayer("Player") || gameObject.layer == LayerMask.NameToLayer("Friends"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                var enemyStats = other.gameObject.GetComponentInParent<EntityStats>();
                if (enemyStats)
                {
                    Health -= enemyStats.damage;
                    // health loss effect on health loss!
                }
            }
        }
    }


    void Start()
    {
        _baseScale = transform.localScale;
        
        var lastMultiplicativeModifier = MultiplicativeModifier;
        MultiplicativeModifier = 1f;
        MultiplicativeModifier = lastMultiplicativeModifier;
    }
}



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
    
//CAMERA - playerFollowCamera Lens FOV. Vertical should increase when moving fast.
// camera distance and side should change based on The One Slider.
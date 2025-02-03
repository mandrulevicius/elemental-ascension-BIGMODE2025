using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HealthText;
    [SerializeField] TextMeshProUGUI PlantPoolText;
    [SerializeField] TextMeshProUGUI PickupCountText;
    [SerializeField] GameObject player;
    private PlayerActions playerActions;
    [SerializeField] EntityStats stats;
    [SerializeField] private Slider slider;

    [SerializeField] private GameObject audioManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HealthText.text = stats.Health.ToString();
    }

    void OnEnable()
    {
        stats = player.GetComponent<EntityStats>();
        stats.OnHealthChanged += HealthChangeUI;
        playerActions = player.GetComponent<PlayerActions>();
        playerActions.OnPlanting += UiSliderUbdate;
        playerActions.OnPlantsChanged += UiPlantUpdate;
    }

    void UiSliderUbdate(float value)
    {
        slider.value = value;
    }

    void HealthChangeUI(float health)
    {
        
        HealthText.text = "Health: " + Mathf.RoundToInt(health).ToString();
        PickupCountText.text ="Power: " + stats.pickupsGathered.ToString();
    }

     void UiPlantUpdate(List<GameObject> plantPool)
    {
        Dictionary<string, int> plantCounts = new Dictionary<string, int>();

        foreach (var plant in plantPool)
        {
            string plantName = plant.tag; // Get plant name
            if (plantCounts.ContainsKey(plantName))
            {
                plantCounts[plantName]++; // Increase count if exists
            }
            else
            {
                plantCounts[plantName] = 1; // First time seeing this plant
            }
        }

        string plantText = " ";
        // Debug output
        foreach (var pair in plantCounts)
        {
            plantText += pair.Key + ": " + pair.Value.ToString()+"\n";
        }

        PlantPoolText.text = plantText;
        // If needed, update your UI here using plantCounts
    }
}
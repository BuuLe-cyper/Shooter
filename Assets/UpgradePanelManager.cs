using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterStatType
{
    Health,
    Attack,
    Speed
}

public class UpgradePanelManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button applyButton;
    public Button closeButton;
    public Button decreaseHealthButton, increaseHealthButton;
    public Button decreaseAttackButton, increaseAttackButton;
    public Button decreaseSpeedButton, increaseSpeedButton;

    [Header("Text Fields")]
    public TextMeshProUGUI healthValueText, attackValueText, speedValueText;
    public TextMeshProUGUI pointsText;

    [Header("Upgrade Price")]
    public TextMeshProUGUI healthUpgradePrice, attackUpgradePrice, speedUpgradePrice;

    [Header("Discount Note Text")]
    public TextMeshProUGUI discountNoteText;

    private const int upgradeCost = 500;
    private const int healthUpgradeStep = 10;
    private const int attackUpgradeStep = 5;
    private const int speedUpgradeStep = 3;

    private CharacterStats originalStats;
    private int finalUpgradeCost;

    private void Start()
    {
        CharacterStatsManager.InitializeCharacterStats();
        CacheCurrentStats();
        UpdateCurrentStatsUI();
        UpdateUpgradePriceUI();

        applyButton.onClick.AddListener(ApplyStats);
        closeButton.onClick.AddListener(CancelAndClosePanel);
        AddButtonListeners();
    }

    private void AddButtonListeners()
    {
        // Adds button listeners dynamically based on stat type and steps
        AddStatButtonListener(increaseHealthButton, CharacterStatType.Health, healthUpgradeStep);
        AddStatButtonListener(decreaseHealthButton, CharacterStatType.Health, -healthUpgradeStep);
        AddStatButtonListener(increaseAttackButton, CharacterStatType.Attack, attackUpgradeStep);
        AddStatButtonListener(decreaseAttackButton, CharacterStatType.Attack, -attackUpgradeStep);
        AddStatButtonListener(increaseSpeedButton, CharacterStatType.Speed, speedUpgradeStep);
        AddStatButtonListener(decreaseSpeedButton, CharacterStatType.Speed, -speedUpgradeStep);
    }

    private void AddStatButtonListener(Button button, CharacterStatType statType, int step)
    {
        button.onClick.AddListener(() => ModifyStat(statType, step));
    }

    private void CacheCurrentStats()
    {
        originalStats = new CharacterStats
        {
            health = new CharacterStat(CharacterStatsManager.CurrentStats.health.GetValue()),
            attack = new CharacterStat(CharacterStatsManager.CurrentStats.attack.GetValue()),
            speed = new CharacterStat(CharacterStatsManager.CurrentStats.speed.GetValue()),
            point = new CharacterStat(CharacterStatsManager.CurrentStats.point.GetValue())
        };
    }

    private void UpdateCurrentStatsUI()
    {
        healthValueText.text = CharacterStatsManager.CurrentStats.health.GetValue().ToString();
        attackValueText.text = CharacterStatsManager.CurrentStats.attack.GetValue().ToString();
        speedValueText.text = CharacterStatsManager.CurrentStats.speed.GetValue().ToString();
        pointsText.text = CharacterStatsManager.CurrentStats.point.GetValue().ToString();
    }

    private void UpdateUpgradePriceUI()
    {
        finalUpgradeCost = CalculateFinalUpgradeCost();

        healthUpgradePrice.text = finalUpgradeCost.ToString();
        attackUpgradePrice.text = finalUpgradeCost.ToString();
        speedUpgradePrice.text = finalUpgradeCost.ToString();

        if (finalUpgradeCost != upgradeCost)
        {
            discountNoteText.text = "* 50% Discount Applied Today";
        }
    }
    private void ModifyStat(CharacterStatType statType, int amount)
    {
        CharacterStat stat = GetStatByType(statType);
        int originalValue = GetOriginalStatByType(statType).GetValue();
        int newValue = Mathf.Max(originalValue, stat.GetValue() + amount);

        if (amount > 0 && CanUpgrade())
        {
            stat.SetValue(newValue);
            DeductPoints();
        }
        else if (amount < 0 && stat.GetValue() > originalValue)
        {
            stat.SetValue(newValue);
            RefundPoints();
        }

        UpdateCurrentStatsUI();
    }

    private CharacterStat GetStatByType(CharacterStatType statType) =>
        statType switch
        {
            CharacterStatType.Health => CharacterStatsManager.CurrentStats.health,
            CharacterStatType.Attack => CharacterStatsManager.CurrentStats.attack,
            CharacterStatType.Speed => CharacterStatsManager.CurrentStats.speed,
            _ => throw new System.ArgumentOutOfRangeException()
        };

    private CharacterStat GetOriginalStatByType(CharacterStatType statType) =>
        statType switch
        {
            CharacterStatType.Health => originalStats.health,
            CharacterStatType.Attack => originalStats.attack,
            CharacterStatType.Speed => originalStats.speed,
            _ => throw new System.ArgumentOutOfRangeException()
        };

    private bool CanUpgrade() =>
        CharacterStatsManager.CurrentStats.point.GetValue() >= finalUpgradeCost;

    private void DeductPoints() =>
        CharacterStatsManager.CurrentStats.point.SetValue(CharacterStatsManager.CurrentStats.point.GetValue() - finalUpgradeCost);

    private void RefundPoints() =>
        CharacterStatsManager.CurrentStats.point.SetValue(CharacterStatsManager.CurrentStats.point.GetValue() + finalUpgradeCost);

    private void ApplyStats()
    {
        if (HasStatsChanged())
        {
            CharacterStatsManager.SaveCharacterStats(CharacterStatsManager.CurrentStats);
            CacheCurrentStats();
            Debug.Log("Stats applied successfully.");
        }
        else
        {
            Debug.Log("No changes detected, stats not saved.");
        }
    }

    private bool HasStatsChanged() =>
        CharacterStatsManager.CurrentStats.health.GetValue() != originalStats.health.GetValue() ||
        CharacterStatsManager.CurrentStats.attack.GetValue() != originalStats.attack.GetValue() ||
        CharacterStatsManager.CurrentStats.speed.GetValue() != originalStats.speed.GetValue() ||
        CharacterStatsManager.CurrentStats.point.GetValue() != originalStats.point.GetValue();

    private void CancelAndClosePanel()
    {
        if (HasStatsChanged())
        {
            // Rollback to original stats if any changes are detected
            RestoreOriginalStats();
        }

        UpdateCurrentStatsUI();
        Debug.Log("Changes canceled, stats rolled back.");
    }

    private void RestoreOriginalStats()
    {
        CharacterStatsManager.CurrentStats.health.SetValue(originalStats.health.GetValue());
        CharacterStatsManager.CurrentStats.attack.SetValue(originalStats.attack.GetValue());
        CharacterStatsManager.CurrentStats.speed.SetValue(originalStats.speed.GetValue());
        CharacterStatsManager.CurrentStats.point.SetValue(originalStats.point.GetValue());
    }

    // Function to calculate the final upgrade cost with discount if applicable
    private int CalculateFinalUpgradeCost()
    {
        // Check if today's day matches the current month
        if (DateTime.Now.Day == DateTime.Now.Month)
        {
            return upgradeCost / 2; // Apply 50% discount
        }
        return upgradeCost; // Regular cost
    }
}
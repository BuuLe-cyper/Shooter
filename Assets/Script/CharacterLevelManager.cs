using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelManager : MonoBehaviour
{
    public int experiencePoints = 0;
    public int experienceLevelUpStep = 100;
    public int experienceThreshold = 100;
    public int level = 1;
    private bool canGainExperience = true;

    public Slider expSlider;
    public TextMeshProUGUI levelText;

    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;

    private AudioManager audioManager; // Reference to AudioManager

    //private CharacterWeaponDamage attackDamage;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        ////attackDamage = FindObjectOfType<CharacterWeaponDamage>();
        ////if (attackDamage == null)
        ////{
        ////    Debug.LogError("CharacterWeaponDamage component not found on " + gameObject.name);
        ////}
        //GameObject player = GameObject.FindGameObjectWithTag("character");
        //if(player == null )
        //{
        //    player = GameObject.FindGameObjectWithTag("Player");
        //}
        //spriteRenderer = player.GetComponent<SpriteRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        expSlider.value = 0;
        UpdateLevelCharacterText();
    }

    private void Update()
    {
        expSlider.value = (float)experiencePoints / experienceThreshold;
    }

    // Start is called before the first frame update
    public void GainExperiences(int expGained)
    {
        if (canGainExperience)
        {
            canGainExperience = false; // Prevent further gains until this is reset
            experiencePoints += expGained;
            Debug.Log("EXP GAINED " + expGained);
            Debug.Log("EXP POINTS " + experiencePoints);

            while (experiencePoints > experienceThreshold)
            {
                LevelUp();
            }

            StartCoroutine(ResetExperienceGain());
        }
    }

    private void LevelUp()
    {
        level++;
        experiencePoints = 0;
        experienceThreshold += experienceLevelUpStep;
        audioManager.PlaySFX(audioManager.LevelUp);

        playerHealth.AddHealth(10);

        // Start level up animation
        StartCoroutine(LevelUpAnimation());

        //attackDamage.BoostDamage(5);
        UpdateLevelCharacterText();
        UpgradeCharacterByLevel();
    }


    private void UpdateLevelCharacterText()
    {
        levelText.text = "Level " + level;

    }

    private IEnumerator ResetExperienceGain()
    {
        yield return new WaitForSeconds(1f); // Adjust this duration as needed
        canGainExperience = true;
    }

    private IEnumerator LevelUpAnimation()
    {
        // Scale up
        Vector3 originalScale = transform.localScale;
        float scaleDuration = 0.3f; // Time for scale up and down
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            float scaleValue = Mathf.Lerp(1f, 1.2f, elapsedTime / scaleDuration); // Scale up to 120%
            transform.localScale = originalScale * scaleValue;
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Scale back down
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            float scaleValue = Mathf.Lerp(1.2f, 1f, elapsedTime / scaleDuration); // Scale back down to 100%
            transform.localScale = originalScale * scaleValue;
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.localScale = originalScale; // Ensure the scale is reset
    }
    private void UpgradeCharacterByLevel()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player") ?? GameObject.FindGameObjectWithTag("character");
        GameObject weapon = GameObject.FindGameObjectWithTag("PlayerAttack");

        if (weapon != null)
        {
            CharacterWeaponDamage damageWeapon = weapon.GetComponent<CharacterWeaponDamage>();
            if (damageWeapon != null)
            {
                damageWeapon.damage *= 1 + (0.1f * level);
            }
        }

        if (player != null)
        {
            PlayerMove playerMove = player.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.moveSpeed *= 1 + (0.05f * level);
                playerMove.summonCooldownTime = Mathf.Max(0, playerMove.summonCooldownTime - (0.5f * level));
                playerMove.dragonWarriorDuration += 2 * level;
            }

            PlayerController2 playerController2 = player.GetComponent<PlayerController2>();
            if (playerController2 != null)
            {
                playerController2.moveSpeed *= 1 + (0.05f * level);
                playerController2.damage *= 1 + (0.1f * level);
            }
        }

        playerHealth.UpgradeMaxHealthByLevelUp();
    }
}

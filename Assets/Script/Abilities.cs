using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{

    [Header("Ability 1")]
    public Image ability1Image;
    public Text ability1Text;
    public KeyCode ability1Key;
    public float ability1Cooldown = 10;

    private bool isAbility1Cooldown = false;

    private float currentAbility1Cooldown;

    // Start is called before the first frame update
    void Start()
    {
        ability1Image.fillAmount = 0;
        ability1Text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        Ability1Input();

        AbilityCooldown(ref currentAbility1Cooldown, ability1Cooldown, ref isAbility1Cooldown, ability1Image, ability1Text);
    }


    private void Ability1Input()
    {
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            Debug.Log(true);
            isAbility1Cooldown = true;
            currentAbility1Cooldown = ability1Cooldown;
        }
    }

    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, Text skillText)
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0f;

                if (skillImage != null)
                {
                    skillImage.fillAmount = 0f;
                }
                if (skillText != null)
                {
                    skillText.text = "";
                }
            }
            else
            {
                if (skillImage != null)
                {
                    skillImage.fillAmount = currentCooldown / maxCooldown;
                }

                if (skillText != null)
                {
                    skillText.text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
        }
    }

    public void StartFlashCooldown()
    {
        isAbility1Cooldown = true;
        currentAbility1Cooldown = ability1Cooldown;
    }

    public bool IsAbilityOnCooldown()
    {
        return isAbility1Cooldown;
    }
}

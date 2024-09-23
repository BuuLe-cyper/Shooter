using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Transform characterTransform; // To reference the character

    private void Start()
    {
        // Assuming the weapon is a child of the character, we can get the parent transform
        characterTransform = transform.parent;
    }

    private void Update()
    {
        RotateGun();
    }

    void RotateGun()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure it's in 2D space

        // Calculate direction from the weapon to the mouse position
        Vector3 direction = mousePos - transform.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // If the character is facing left, adjust the angle by 180 degrees to rotate correctly
        if (characterTransform.localScale.x < 0)
        {
            angle += 180f;
        }

        // Apply the rotation to the weapon
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Adjust scale based on the weapon's angle to prevent flipping upside down
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(currentScale.x, Mathf.Abs(currentScale.y), currentScale.z);
        // Flip the weapon vertically when aiming left, avoid flipping upside down
        //if (angle > 90 || angle < -90) // Weapon is pointing left
        //{
        //    transform.localScale = new Vector3(currentScale.x, -Mathf.Abs(currentScale.y), currentScale.z);
        //}
        //else // Weapon is pointing right
        //{
        //    transform.localScale = new Vector3(currentScale.x, Mathf.Abs(currentScale.y), currentScale.z);
        //}
    }
}

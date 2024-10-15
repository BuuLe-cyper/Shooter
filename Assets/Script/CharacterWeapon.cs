using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    private Transform characterTransform; // To reference the character
    public GameObject bullet;
    public Transform firePos;
    public float TimeBtwFire = 0.2f;
    public float bulletForce;
    public AudioManager audioManager; // Reference to AudioManager
    public GameObject muzzle;

    private float timeBtwFire;

    
    private void Start()
    {
        // Assuming the weapon is a child of the character, we can get the parent transform
        characterTransform = transform.parent;

        // Assign AudioSource components (optional if already assigned in Inspector)
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        RotateGun();
        timeBtwFire -= Time.deltaTime;
        if(Input.GetMouseButton(0) && timeBtwFire < 0)
        {
            FireBullet();

        }
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
    void FireBullet()
    {
        timeBtwFire = TimeBtwFire;
        audioManager.PlaySFX(audioManager.Shoot);
        // Instantiate the bullet at fire position
        GameObject bulletTmp = Instantiate(bullet, firePos.position, Quaternion.identity);


        Rigidbody2D rb = bulletTmp.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.2f;

        // Get mouse position in world space and calculate direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure it's in 2D space

        Vector3 direction = (mousePos - firePos.position).normalized;

        // Calculate the angle based on direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Determine where to position the muzzle flash (shifted away from the fire position)
        Vector3 muzzlePosition = firePos.position + direction * 0.3f; 

        // Instantiate muzzle flash and rotate it to match the bullet direction
        GameObject muzzleInstance = Instantiate(muzzle, muzzlePosition, Quaternion.Euler(0, 0, angle));

        // If muzzle is a particle system, play it
        ParticleSystem ps = muzzleInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        // Destroy muzzle effect after a short delay
        Destroy(muzzleInstance, 0.1f);

        float bulletForce = 25f;

        // Add force to the bullet
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
    }


}

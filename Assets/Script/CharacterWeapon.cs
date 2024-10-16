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

    private bool isAutoFireMode = false; // Tracks if auto-fire mode is active

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

        // Toggle between auto and manual fire modes when pressing 'V'
        if (Input.GetKeyDown(KeyCode.V))
        {
            isAutoFireMode = !isAutoFireMode;
        }

        // Fire bullets based on the current fire mode
        if (isAutoFireMode)
        {
            AutoFire();
        }
        else if (Input.GetMouseButton(0) && timeBtwFire <= 0)
        {
            FireBullet();
        }
    }

    // Rotate the gun to follow the mouse
    void RotateGun()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure it's in 2D space

        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (characterTransform.localScale.x < 0)
        {
            angle += 180f;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(currentScale.x, Mathf.Abs(currentScale.y), currentScale.z);
    }

    // Handle auto-firing mode
    void AutoFire()
    {
        if (timeBtwFire <= 0)
        {
            FireBullet();
        }
    }

    // Method to fire a bullet
    void FireBullet()
    {
        timeBtwFire = TimeBtwFire;
        audioManager.PlaySFX(audioManager.Shoot);

        GameObject bulletTmp = Instantiate(bullet, firePos.position, Quaternion.identity);
        Rigidbody2D rb = bulletTmp.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.2f;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = (mousePos - firePos.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 muzzlePosition = firePos.position + direction * 0.3f;
        GameObject muzzleInstance = Instantiate(muzzle, muzzlePosition, Quaternion.Euler(0, 0, angle));

        ParticleSystem ps = muzzleInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        Destroy(muzzleInstance, 0.1f);
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;

    public Camera fpsCam;
    public AudioClip gunShotClip;
    public AudioClip bulletDropClip;
    public AudioSource audioSource;
    public VisualEffect muzzleFlashEffect;

    private float nextTimeToFire = 0f;


    public WeaponAnimations weaponAnimationsScript;
    public GameObject muzzleFlash;

    bool isMuzzleFlash = false;

    public void Update() {

        weaponAnimationsScript.Recoil(false);

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire) {

            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            if (!isMuzzleFlash) StartCoroutine(SwitchMuzzleFlash());
        }
      
    }

    IEnumerator PlayGunAudio() {

        audioSource.PlayOneShot(gunShotClip);

        yield return new WaitForSeconds(0.5f);

        int random = Random.Range(0, 3);
        if (random == 0) audioSource.PlayOneShot(bulletDropClip);

    }


    void Shoot() {

        weaponAnimationsScript.Recoil(true);
        muzzleFlashEffect.Play();

        StartCoroutine(PlayGunAudio());

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
            }
        }
    }

    IEnumerator SwitchMuzzleFlash() {

        isMuzzleFlash = true;
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.1f);
        muzzleFlash.SetActive(false);
        isMuzzleFlash = false;

    }
}

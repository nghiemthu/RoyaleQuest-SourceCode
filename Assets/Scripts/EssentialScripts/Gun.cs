using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {


	public Transform[] projectileSpawn;
	public Projectile projectile;
	public Projectile hugeProjectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 20;
	public float reloadTime = .3f;
	float muzzleDamage =1;
	float powerPerShot = 1f;

	Player player;

	[Header("Effects")]
	public AudioClip shootAudio;

	float nextShotTime;

	bool triggerReleasedSinceLastShot;
	bool isReloading;

	void Start() {
		if (GameObject.FindGameObjectWithTag("Player") != null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		}
		muzzleDamage = PlayerPrefsController.instance.GetProjectileDamage ();
		muzzleVelocity = PlayerPrefsController.instance.GetProjectileSpeed ();
	}

	void LateUpdate() {
		if (!isReloading) {
			Reload();
		}
	}

	public void SetMuzzleVelocity(float velocity){
		muzzleVelocity = velocity;
	}

	public void SetMuzzleDamage(float damage){
		muzzleDamage = damage;
	}

	void Update(){
	}

	void Shoot() {
		if (!triggerReleasedSinceLastShot) {
			return;
		}
		player.IncreaseInnerPower ();
		player.DecreaseThePower (powerPerShot);
		for (int i =0; i < projectileSpawn.Length; i ++) {
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newProjectile = Instantiate (projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
			newProjectile.SetSpeed (muzzleVelocity);
			newProjectile.SetDamage (muzzleDamage);
			newProjectile.isPower = false;
		}
			AudioManager.instance.PlaySound (shootAudio, transform.position);
	}

	public void EnemyShoot() {
		
		for (int i =0; i < projectileSpawn.Length; i ++) {
			Projectile newProjectile = Instantiate (projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
			newProjectile.SetSpeed (muzzleVelocity);
			newProjectile.SetDamage (muzzleDamage);
			newProjectile.isPower = false;
		}
		//	AudioManager.instance.PlaySound (shootAudio, transform.position);
	}

	public void ShootPower(){
		
		if (player.innerPower == 0){
			return;
		}
		player.ResetInnerPower ();
		for (int i =0; i < projectileSpawn.Length; i ++) {
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newProjectile = Instantiate (hugeProjectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
			newProjectile.SetSpeed (muzzleVelocity);
			newProjectile.SetDamage (30);
			newProjectile.isPower = true;
		}
		AudioManager.instance.PlaySound (shootAudio, transform.position);
	}

	public void Reload() {
		if (!isReloading) {
			StartCoroutine (AnimateReload ());

		}
	}

	IEnumerator AnimateReload() {
		isReloading = true;
		yield return new WaitForSeconds (reloadTime);

		isReloading = false;
	}

	public void Aim(Vector3 aimPoint) {
		if (!isReloading) {
			transform.LookAt (aimPoint);
		}
	}

	public void OnTriggerHold() {
		Shoot ();
		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease() {
		triggerReleasedSinceLastShot = true;
	}
}

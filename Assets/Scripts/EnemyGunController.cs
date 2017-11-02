using UnityEngine;
using System.Collections;

public class EnemyGunController : MonoBehaviour {

	public Transform weaponHold;
	public Gun gun;
	Gun equippedGun;
	public float muzzleVelocity;
	public float muzzleDamage;

	void Start () {
		
		equippedGun = Instantiate (gun, weaponHold.position,weaponHold.rotation) as Gun;
		equippedGun.transform.parent = weaponHold;
		equippedGun.SetMuzzleDamage (muzzleVelocity);
		equippedGun.SetMuzzleVelocity (muzzleDamage);
	
	}


	public void Shoot(){
		equippedGun.EnemyShoot ();
	}

	IEnumerator	AutoShoot(){
		yield return new WaitForSeconds (2.5f);
		equippedGun.EnemyShoot ();
		StartCoroutine (AutoShoot());
	}
}

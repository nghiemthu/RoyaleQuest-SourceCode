using UnityEngine;
using System.Collections;

public class GunUp : Power {

	GunController gunController;

	protected override void Update(){
		base.Update ();
	}

	protected override void Awake(){
		base.Awake ();
		gunController = FindObjectOfType<GunController> ();
	}

	void OnTriggerEnter(Collider target){
		if (target.tag == "Player"){
			AudioManager.instance.PlaySound ("Pick Up", transform.position);
			gunController.currentGunIndex++;
			GameUIControl.instance.AnimateShards (2);
			if (gunController.currentGunIndex<=4){
				gunController.EquipGun (gunController.currentGunIndex);
			}
			if (!PlayerPrefs.HasKey("FirstYellowShard")){
				PlayerPrefs.SetInt ("FirstYellowShard",0);
				GameUIControl.instance.AnimateTalkPanel ("Collect Yellow Shard to upgrade your gun",2);
			}
			Destroy (gameObject);
		}
	}
}

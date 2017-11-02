using UnityEngine;
using System.Collections;

public class Brust : Power {

	LivingEntity playerEntity;

	protected override void Awake(){
		base.Awake ();
		playerEntity = FindObjectOfType<Player> ();

	}
	protected override void Update(){
		base.Update ();
	}

	void OnTriggerEnter(Collider target){
		if (target.tag == "Player"){
			AudioManager.instance.PlaySound ("Pick Up", transform.position);
			playerEntity.UnbreakablePlayer ();
			GameUIControl.instance.AnimateShards (3);  
			GameUIControl.instance.SetBarColor ();
			if (!PlayerPrefs.HasKey("FirstCrystalShard")){
				PlayerPrefs.SetInt ("FirstCrystalShard",0);
				GameUIControl.instance.AnimateTalkPanel ("Collect Crystal Shard to have unlimited power",2);
			} 
			Destroy (gameObject);
		}
	}


}

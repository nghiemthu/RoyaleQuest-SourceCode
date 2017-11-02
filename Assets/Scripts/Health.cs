using UnityEngine;
using System.Collections;

public class Health : Power {

	Player player;
	protected override void Awake(){
		base.Awake ();
		player = FindObjectOfType<Player> ();
	}
	protected override void Update (){
		base.Update ();
	}

	void OnTriggerEnter(Collider target){
		if (target.tag == "Player"){
			AudioManager.instance.PlaySound ("Pick Up", transform.position);
			GameUIControl.instance.AnimateShards (0);
			player.health = player.startingHealth;
			if (!PlayerPrefs.HasKey("FirstRedShard")){
				PlayerPrefs.SetInt ("FirstRedShard",0);
				GameUIControl.instance.AnimateTalkPanel ("Collect Red Shard to recover your HP",2);
			}
			Destroy (gameObject);
		}
	}
}

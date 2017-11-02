using UnityEngine;
using System.Collections;

public class PowerPower : Power{

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
			player.power = PlayerPrefsController.instance.GetPlayerPower ();
			GameUIControl.instance.AnimateShards (1);
			if (!PlayerPrefs.HasKey("FirstBlueShard")){
				PlayerPrefs.SetInt ("FirstBlueShard",0);
				GameUIControl.instance.AnimateTalkPanel ("Collect Blue Shard to recover your power",2);
			}
			Destroy (gameObject);
		}
	}
}

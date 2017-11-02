using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

	public float startingHealth;
	public float health;
	protected bool dead;
	public bool unbreakable;

	public event System.Action OnDeath;

	protected virtual void Start() {
		health = startingHealth;
		unbreakable = false;
	}

	public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
		// Do some stuff here with hit var
			TakeDamage (damage);

	}

	public virtual void TakeDamage(float damage) {
		if (!unbreakable){
			health -= damage;
		}
	
		
		if (health <= 0 && !dead) {
			Die();
		}
	}

	public void UnbreakablePlayer(){
		StartCoroutine (Unbreakable());
	}

	IEnumerator Unbreakable(){
	
		unbreakable = true;
		yield return new WaitForSeconds (4f);
		unbreakable = false;
	}

	IEnumerator AfterDieEffect(){
		GameUIControl.instance.AnimateTalkPanel ("Mission failed!", 1);
		yield return new WaitForSeconds (2.5f);
		GameUIControl.instance.GoHome ();
	}

	[ContextMenu("Self Destruct")]
	public virtual void Die() {
		dead = true;
		if (OnDeath != null) {
			OnDeath();
		}

		GameObject.Destroy (gameObject);
	}
}

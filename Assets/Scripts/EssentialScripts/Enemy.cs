using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

	public enum State {Idle, Chasing, Attacking};
	State currentState;

	public static event System.Action OnDeathStatic;

	NavMeshAgent pathfinder;
	Transform target;
	LivingEntity targetEntity;

	public float attackDistanceThreshold = .5f;
	public float timeBetweenAttacks = 1;
	public float damage = 1;

	float nextAttackTime;
	float myCollisionRadius;
	float targetCollisionRadius;

	int randomIndex;
	bool hasTarget;
	Material skinMaterial;

	public float moveSpeed =3;

	public bool isShooting;
	public bool isInvinsible;
	EnemyGunController gunController;

	public Power[] power;
	DeathPartical deathPartical;

	void Awake() {
		OnDeath += InstantiateShards;
		pathfinder = GetComponent<NavMeshAgent> ();
		deathPartical = FindObjectOfType<DeathPartical> ();

		if (GetComponent<EnemyGunController>() != null){
			gunController = GetComponent<EnemyGunController> ();
		}

		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			hasTarget = true;

			target = GameObject.FindGameObjectWithTag ("Player").transform;
			targetEntity = target.GetComponent<LivingEntity> ();

			myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;
			SetSpeedAndDamage (moveSpeed);

		}
	}

	void InstantiateShards(){
		if (Random.Range(0, 10f) > 8f){
			int randomIndex = (int)Random.Range (0, power.Length - 0.4f);
			Instantiate (power[randomIndex], transform.position, Quaternion.identity);
		}
	}

	protected override void Start () {
		base.Start ();

		if (hasTarget) {
			currentState = State.Chasing;
			targetEntity.OnDeath += OnTargetDeath;

			StartCoroutine (UpdatePath ());
		}
	}

	void SetSpeedAndDamage(float moveSpeed){
		pathfinder.speed = moveSpeed;
	}

	public void SetCharacteristics(Color skinColour, int randomIndex) {
		
		this.randomIndex = randomIndex;
		deathPartical.deathEffect[randomIndex].startColor = new Color (skinColour.r, skinColour.g, skinColour.b, 1);
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		AudioManager.instance.PlaySound ("Impact", transform.position);
		if (damage >= health && !dead) {
			if (OnDeathStatic != null) {
				OnDeathStatic ();
			}
			AudioManager.instance.PlaySound ("Enemy Death", transform.position);
			Destroy(Instantiate(deathPartical.deathEffect[randomIndex].gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathPartical.deathEffect[randomIndex].startLifetime);
		}
		base.TakeHit (damage, hitPoint, hitDirection);
	}

	void OnTargetDeath() {
		hasTarget = false;
		currentState = State.Idle;
	}

	void Update () {

		if (hasTarget) {
			if (Time.time > nextAttackTime) {
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
				if (sqrDstToTarget < Mathf.Pow (attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) {
					nextAttackTime = Time.time + timeBetweenAttacks;

					if (!isShooting) {
						StartCoroutine (Attack ());

					} 
				}
				if (isShooting){
					if (sqrDstToTarget >= Mathf.Pow (attackDistanceThreshold / 2 + myCollisionRadius + targetCollisionRadius, 2) &&
						sqrDstToTarget <= Mathf.Pow (attackDistanceThreshold / 2 + myCollisionRadius + targetCollisionRadius, 2)*2) {
						nextAttackTime = Time.time + timeBetweenAttacks;
						currentState = State.Attacking;
						pathfinder.enabled = false;
						transform.LookAt (target);
						AudioManager.instance.PlaySound ("Enemy Shoot", transform.position);
						gunController.Shoot ();
					} else {
						currentState = State.Chasing;
						pathfinder.enabled = true;
					}
				}

				if (isInvinsible){
					if (sqrDstToTarget >= (Mathf.Pow (attackDistanceThreshold / 2 + myCollisionRadius + targetCollisionRadius, 2) +20f)) {
						gameObject.layer = 0;
						GetComponent<Renderer> ().material.color = new Color(.9f,.9f,.9f,.5f);
					} else {
						gameObject.layer = 8;
						GetComponent<Renderer> ().material.color = new Color(.9f,.9f,.9f,1f);
					}
				}
			}
		}
	}



	IEnumerator Attack() {

		currentState = State.Attacking;
		pathfinder.enabled = false;

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

		float attackSpeed = 3;
		float percent = 0;

		bool hasAppliedDamage = false;

		while (percent <= 1) {

			if (percent >= .5f && !hasAppliedDamage) {
				hasAppliedDamage = true;
				AudioManager.instance.PlaySound ("Enemy Attack", transform.position);
				targetEntity.TakeDamage(damage);
			}

			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
			transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

			yield return null;
		}
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}

	IEnumerator UpdatePath() {
		float refreshRate = .25f;

		while (hasTarget) {
			if (currentState == State.Chasing) {
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
				if (!dead) {
					pathfinder.SetDestination (targetPosition);
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
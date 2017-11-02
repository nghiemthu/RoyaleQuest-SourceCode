using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity {

	public enum PlayerMode {Home, Gameplay};
	public PlayerMode playerMode;

	public float moveSpeed = 5;

	public Crosshairs crosshairs;

	Camera viewCamera;
	PlayerController controller;
	GunController gunController;
	public float power;
	bool moveable;
	bool stopWalking;
	public float innerPower;
	float holdTime;

	SwipeDetecter swipeDetector;

	protected override void Start () {
		base.Start ();
		OnDeath += OnPlayerDeath;
		if (playerMode == PlayerMode.Gameplay) {
			gunController.EquipGun (0);
		} else {
			crosshairs.gameObject.SetActive (false);
		}
	}

	void Awake() {
		swipeDetector = FindObjectOfType<SwipeDetecter> ();
		controller = GetComponent<PlayerController> ();
		gunController = GetComponent<GunController> ();
		viewCamera = Camera.main;

		health = startingHealth;
		moveable = true;
		innerPower = 0;
		power = PlayerPrefsController.instance.GetPlayerPower ();
	}
		
	void OnPlayerDeath(){
		PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook()-1);
	}

	public void DecreaseThePower(float powerToDecrease){
		if (!unbreakable){
			power -= powerToDecrease;
		}
	}

	public void IncreaseInnerPower(){
		if (innerPower < 15){
			innerPower++;
		}
	}

	public void ResetInnerPower(){
		innerPower = 0;
	}

	public void StopWalking(){
		stopWalking = true;
	}

	void Update () {
		//Power Controll
		if (power < PlayerPrefsController.instance.GetPlayerPower ()){
			power += Time.deltaTime*2;
		}

		if (!PlayerPrefs.HasKey("InnerPowerFirstTime") && innerPower >=15){
			GameUIControl.instance.AnimateTalkPanel ("Press to release inner power", 2);
			PlayerPrefs.SetInt ("InnerPowerFirstTime", 0);
		}


		// Movement input

		if (moveable){
			if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw ("Vertical")==1|| Input.GetAxisRaw ("Vertical")==-1){
				Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
				Vector3 moveVelocity = moveInput.normalized * moveSpeed;
				controller.Move (moveVelocity);
			}
			controller.Move (swipeDetector.moveDir.normalized * moveSpeed);
		}



		if (transform.position.y <= -0.25f){
			controller.Move (Vector3.zero);
			moveable = false;
		}

		if (stopWalking){
			controller.Move (Vector3.zero);
			moveable = false;
		}


		// Look input
		Ray ray = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * gunController.GunHeight);
		float rayDistance;


		if (groundPlane.Raycast(ray,out rayDistance)) {
			Vector3 point = ray.GetPoint(rayDistance);
			//Debug.DrawLine(ray.origin,point,Color.red);
			controller.LookAt(point);
			if (playerMode == PlayerMode.Gameplay) {
				crosshairs.transform.position = point;
				crosshairs.DetectTargets(ray);
			}

			//			if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1) {
			//				gunController.Aim(point);
			//			}
		}



		if (playerMode == PlayerMode.Gameplay){
			if (power <= 0){
				power = 0;
				gunController.Reload ();
			} else {
				// Weapon input
				if (power >=1){
					if (Input.GetMouseButton(0)) {
						gunController.OnTriggerHold();
						holdTime += Time.deltaTime;
						if (holdTime >= 1f && innerPower >= 15){
							gunController.ShootPower();
							holdTime = 0;
						}
					}
					if (Input.GetMouseButtonUp(0)) {
						holdTime = 0;
						gunController.OnTriggerRelease();
					}
				}
			}
		}

		if (transform.position.y < -10) {
			TakeDamage (health);
		}
	}
		
	public override void Die ()
	{
		AudioManager.instance.PlaySound ("Player Death", transform.position);
		base.Die ();
	}


		
}

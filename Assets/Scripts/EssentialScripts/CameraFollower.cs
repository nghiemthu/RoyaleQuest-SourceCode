using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour {

	Transform targetTransform;
	float Zdistance;
	float Xdistance;
	bool hasTarget;
	LivingEntity targetEntity;
	Player player;

	// Use this for initialization
	void Start () {
		if (GameObject.FindGameObjectWithTag("Player") != null){
			targetTransform = GameObject.FindGameObjectWithTag ("Player").transform;
			player = targetTransform.GetComponent<Player> ();
			targetEntity = targetTransform.GetComponent<LivingEntity> ();
			Zdistance = transform.position.z - targetTransform.position.z;
			Xdistance = transform.position.x - targetTransform.position.x;
			hasTarget = true;
			targetEntity.OnDeath += HasTarget;
		}
	}

	// Update is called once per frame
	void Update () {
		if (hasTarget){
			Vector3 temp = transform.position;
			temp.z = Zdistance + targetTransform.position.z;
			temp.x = Xdistance + targetTransform.position.x;
			transform.position = Vector3.Lerp (transform.position, temp, 2f* Time.deltaTime);
		}
	}

	void HasTarget(){
		hasTarget = false;
	}
}

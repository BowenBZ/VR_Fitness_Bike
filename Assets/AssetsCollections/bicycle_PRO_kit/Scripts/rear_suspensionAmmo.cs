// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class rear_suspensionAmmo : MonoBehaviour {

	public Transform target;//invisible gameObject as target for lookAt function(place where rear ammo shlould look at)
	public Transform ammoSpring;//spring of rear suspension to squeeze
	public Transform pendulumAngle;//rear pendulum for proper squeeze of spring
	// Use this for initialization
	//void Start () {
	//}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt (target);//ammo should look at rear pendulum
		ammoSpring.localScale = new Vector3(1, 0.5f-(pendulumAngle.localRotation.x*5), 1);//change those 0.5f, 5 as you need for own ammo


	}
}

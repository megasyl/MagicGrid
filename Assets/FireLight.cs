using UnityEngine;
using System.Collections;

public class FireLight : MonoBehaviour {

	public Vector2 gridPosition; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log (collision.collider.gameObject.name);
	}

}

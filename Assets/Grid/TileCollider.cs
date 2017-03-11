using UnityEngine;
using System.Collections;

public class TileCollider : MonoBehaviour {
	public Vector2 gridPosition; 
	public MeshRenderer mr;
	public bool _display;

	public bool Display { 
		get { return _display; } 
		set { 
			mr.enabled = value;
			_display = value;
		}
	}

	public void Start() {
		mr = GetComponent<MeshRenderer> ();
		Display = false;
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log (other.gameObject.GetComponent<CharacterController>().currentNode = other.gameObject.GetComponent<CharacterController>().grid.grid[(int)gridPosition.x, (int)gridPosition.y]);
	}
}

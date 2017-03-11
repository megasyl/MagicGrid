using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>{

	public Vector3 worldPosition;
	public Connection[] connections;
	public bool open;
	public bool selected;
	public Vector2 gridVector;
	public GameObject Collider;

	public Node parent;

	public int gCost;
	public int hCost;
	int heapIndex;

	public Node(Vector3 _worldPos,bool _open, Vector2 _gridVector)
	{
		worldPosition = _worldPos;
		open = _open;
		connections = new Connection[4];
		selected = false;
		gridVector = _gridVector;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
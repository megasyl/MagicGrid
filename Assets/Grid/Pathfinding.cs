using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;

	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}


	public void StartFindPath(Vector2 startPos, Vector2 targetPos) {
		StartCoroutine(FindPath(startPos,targetPos));
	}

	IEnumerator FindPath(Vector2 startPos, Vector2 targetPos) {

		Node[] waypoints = new Node[0];
		bool pathSuccess = false;

		Node startNode = grid.grid[(int)startPos.x, (int)startPos.y];
		Node targetNode = grid.grid[(int)targetPos.x, (int)targetPos.y];


		if (startNode.open && targetNode.open) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.open || closedSet.Contains(neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);

	}

	Node[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Node[] l = path.ToArray ();
		Array.Reverse (l);
		return l;

	}

	Vector3[] SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i ++) {
			//Vector2 directionNew = new Vector2(path[i-1].gridVector.x - path[i].gridVector.x,path[i-1].gridVector.y - path[i].gridVector.y);
			//if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			//}
			//directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs((int)(nodeA.gridVector.x - nodeB.gridVector.x));
		int dstY = Mathf.Abs((int)(nodeA.gridVector.y - nodeB.gridVector.y));

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


}
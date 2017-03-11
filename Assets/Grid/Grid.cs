using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	//Variables
	public Node[,] grid;
	private bool isDrawn = false;

	private int gridSizeX;
	private int gridSizeZ;
	public bool display;
	public GameObject collider;

	//Parametre
	public Vector3 gridSize = new Vector3(10f,10f,10f);
	public float cellSize = 1f;
	public float stepOffset = 1f;
	public LayerMask layersToIgnore;
	// Use this for initialization
	void Start ()
	{
		gridSizeX = Mathf.RoundToInt(gridSize.x/cellSize);
		gridSizeZ = Mathf.RoundToInt(gridSize.z/cellSize);

		CreateGrid();
		GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().setCurrentNode (grid [19, 35]);
        
    }

	// Update is called once per frame
	void Update ()
	{
		


	}
	public int MaxSize {
		get {
			return gridSizeX * gridSizeZ;
		}
	}

	public void CreateGrid()
	{
		grid = new Node[gridSizeX,gridSizeZ];
		Vector3 gridBottomLeft = transform.position - Vector3.right * gridSize.x/2 - Vector3.forward * gridSize.z/2;

		//Create node
		for(int x = 0; x < gridSizeX; x++)
			for(int z = 0; z < gridSizeZ; z++)
			{

				Vector3 worldPoint = gridBottomLeft + Vector3.right * (x * cellSize + cellSize/2)
					+ Vector3.forward * (z * cellSize + cellSize/2);
				grid[x,z] = new Node(worldPoint,true, new Vector2(x,z));
			}


		grid [2, 2].selected = true;
		//Raycast terrain
		foreach(Node n in grid)
		{
			Ray ray = new Ray(n.worldPosition + new Vector3(0,gridSize.y,0),Vector3.down);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast (ray, out hit, gridSize.y * 2, layersToIgnore)) {
				n.worldPosition = hit.point + new Vector3 (0, 0.1f, 0);


			} else {
				n.open = false;
			}

		}

		//Create connection
		for(int x = 0; x < gridSizeX ; x++)
			for(int z = 0; z < gridSizeZ; z++)
			{
				if (grid [x, z].open) {
					if (x + 1 < gridSizeX)
						grid [x, z].connections [0] = new Connection (grid [x, z].worldPosition, grid [x + 1, z].worldPosition, true);
					if (z + 1 < gridSizeZ)
						grid [x, z].connections [1] = new Connection (grid [x, z].worldPosition, grid [x, z + 1].worldPosition, true);
					//if (x + 1 < gridSizeX && z + 1 < gridSizeZ)
					//	grid [x, z].connections [2] = new Connection (grid [x, z].worldPosition, grid [x + 1, z + 1].worldPosition, true);
					//if (x + 1 < gridSizeX && z - 1 >= 0)
					//	grid [x, z].connections [3] = new Connection (grid [x, z].worldPosition, grid [x + 1, z - 1].worldPosition, true);
				}
			}



		//Remove invalid Connection
		foreach(Node n in grid)
			foreach(Connection c in n.connections)
			{
				if(c != null)
				{
					if(Mathf.Abs(c.start.y - c.end.y) > stepOffset)
					{
						c.valid = false;
					}

				}

			}
		if (!isDrawn) {
			DrawGrid ();
			isDrawn = true;
		}
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		Debug.Log (grid.Length);
		Node tMin = null;
		float minDist = Mathf.Infinity;
		int i=0;

		foreach (Node t in this.grid)
		{
			i++;
			float dist = Vector3.Distance(t.worldPosition, worldPosition);

			if (dist < minDist)
			{
				tMin = t;


				minDist = dist;
			}
		}
		Debug.Log (i);
		return grid[(int)tMin.gridVector.x, (int)tMin.gridVector.y];
	}

	void DrawSquareAround(Node point, Color color) {
		Vector3 squareBottomLeft = point.worldPosition - Vector3.right * cellSize/2 - Vector3.forward * cellSize/2;
		Vector3 squareTopLeft = point.worldPosition - Vector3.right * cellSize/2 + Vector3.forward * cellSize/2;
		Vector3 squareTopRight = point.worldPosition + Vector3.right * cellSize/2 + Vector3.forward * cellSize/2;
		Vector3 squareBottomRight = point.worldPosition + Vector3.right * cellSize/2 - Vector3.forward * cellSize/2;
		DrawLine(squareBottomLeft, squareTopLeft, color);
		DrawLine(squareBottomRight, squareTopRight, color);
		DrawLine(squareBottomRight, squareBottomLeft, color);
		DrawLine(squareTopRight, squareTopLeft, color);

	}		
	public void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(0.05f, 0.05f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		//GameObject.Destroy (myLine, 0.1f);
	}

	void DrawGrid() {
		//Gizmos.color = Color.black;
		//Gizmos.DrawWireCube(transform.position,gridSize);



		if(grid != null)
		{
			foreach(Node n in grid)
			{
				if (n.open) {
					Color color = (!n.selected) ? Color.white : Color.red;
					if (display)
						DrawSquareAround (n, color);
					GameObject theCollider = Instantiate (collider, n.worldPosition, Quaternion.identity) as GameObject;
					n.Collider = theCollider;
					theCollider.GetComponent<TileCollider> ().gridPosition = n.gridVector;

				}

			}
		}
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = (int)node.gridVector.x + x;
				int checkY = (int)node.gridVector.y + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeZ) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

	/*void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(transform.position,gridSize);



		if(grid != null)
		{
			foreach(Node n in grid)
			{
				Gizmos.color = (n.open)?Color.yellow:Color.red;
				float sizeReductor = 8f;
				if (n.selected)
					sizeReductor = 1f;
				Gizmos.DrawCube(n.worldPosition, Vector3.one  * cellSize/sizeReductor);

				foreach(Connection c in n.connections)
				{
					if(c != null && c.valid)
					{
						Gizmos.color = (c.open)?Color.white:Color.red;
						Gizmos.DrawLine(c.start,c.end);
					}
				}

			}
		}
	}*/
}
using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public Transform target;
	public float speed;
	public float rotationSpeed = 3000;
	public float step;
	public Grid grid;
	public Node currentNode;
	Node[] path;
	int targetIndex;
	public GameObject prefab;
	public int i;
	public bool walking = false;
	public Vector2 bas = new Vector2 (1, 0);
	public Vector2 haut = new Vector2 (-1, 0);
	public Vector2 gauche = new Vector2 (0, -1);
	public Vector2 droite = new Vector2 (0, 1);

	private Vector2 pathTarget;
	private Vector2 pathSource;
	protected Animator animator;

	// Use this for initialization
	void Start() {
		i = 0;
		animator = gameObject.GetComponent<Animator> ();
        
    }

	public void setCurrentNode(Node node) {
		currentNode = node;
		this.transform.position = node.worldPosition;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
    }

	public void OnPathFound(Node[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;
			Debug.Log ("path found in "+newPath.Length+"!!");
			foreach (var item in newPath) {
				//item.Collider.GetComponent<TileCollider> ().Display = true;
				//Instantiate (prefab, item.worldPosition, Quaternion.identity);
			}
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}
		

	IEnumerator FollowPath() {
		Debug.Log ("hello " + path.Length);
		walking = true;
		Vector2 currentWaypoint = path[0].gridVector;
        float velocityXel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity).x;
        float velocityZel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity).z;
        //Update animator with movement values
        
        animator.SetBool("Moving", true);
        float movementSpeed = speed;
        animator.SetFloat("Velocity X", 0);
        animator.SetFloat("Velocity Z", 0);
        if (path.Length > 2)
        {
            movementSpeed *= 2;
            animator.SetFloat("Velocity X", 1);
            animator.SetFloat("Velocity Z", 1);
        }

            
        Debug.Log(movementSpeed);
        while (true) {
			Debug.Log ("hekl");
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.RotateTowards(path[targetIndex].worldPosition), 1);


            //Debug.Log ("current node ("+targetIndex+"/"+path.Length+") : " + currentWaypoint);
            //Debug.Log ("target node ("+targetIndex+"/"+path.Length+") : " + currentWaypoint);
            //if (currentNode.gridVector == currentWaypoint) {
            Vector3 targetPostition = new Vector3(path[targetIndex].worldPosition.x,
                                       this.transform.position.y,
                                       path[targetIndex].worldPosition.z);
            this.transform.LookAt(targetPostition);
           // transform.LookAt(path[targetIndex].worldPosition);
            
            if (Vector3.Distance (path[targetIndex].worldPosition + new Vector3(0,0.128f,0), this.transform.position) < 0.3f) {
                
				targetIndex ++;
				if (targetIndex >= path.Length) {
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    animator.SetBool("Moving", false);
                    yield break;
				}
				currentWaypoint = path[targetIndex].parent.gridVector;

			}

			transform.position = Vector3.MoveTowards(transform.position,path[targetIndex].worldPosition,movementSpeed * Time.deltaTime);
			Camera.main.transform.position = new Vector3(transform.position.x + 15, Camera.main.transform.position.y, transform.position.z);

            yield return null;

		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown (0)) { 
			
				
			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
			if (Physics.Raycast (ray, out hit, 100.0f)) {
                if (hit.collider.gameObject.GetComponent<TileCollider>()) {
                    Vector2 vector;//= grid.NodeFromWorldPoint (hit.transform.position).gridVector;
                    vector = hit.collider.gameObject.GetComponent<TileCollider>().gridPosition;
                    Debug.Log("grid : " + vector.x + "/" + vector.y); // ensure you picked right object
                    grid.grid[(int)vector.x, (int)vector.y].selected = true;
                    Debug.Log("isSelected ? " + grid.grid[(int)vector.x, (int)vector.y].selected);
                }
				
				
			}
			pathTarget = hit.collider.gameObject.GetComponent<TileCollider>().gridPosition; 

			Debug.Log ("Path " + pathSource + " -> " + pathTarget);
			PathRequestManager.RequestPath (currentNode.gridVector, pathTarget, OnPathFound);
		}
	}

}

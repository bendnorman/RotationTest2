using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Rhino2Unity : MonoBehaviour {
	private GameObject[,] tiles;
	public int springValue;
	public int damperValue;




	// Use this for initialization
	void Start () {
		tiles = groupComponents();

		for (int i = 0; i < tiles.GetLength(0); i++) {
			for (int j = 0; j < tiles.GetLength(1); j++) {
				GameObject tile = tiles[i,j];

				Rigidbody panelRigidBody = tile.AddComponent<Rigidbody>();
				panelRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				panelRigidBody.useGravity = false;

				BoxCollider panelBoxCollider = tile.AddComponent<BoxCollider>();

				foreach(Transform circle in tile.GetComponentsInChildren<Transform>()) {
					GameObject corner = circle.gameObject;

					BoxCollider circleBoxCollider = corner.AddComponent<BoxCollider>();

				}
			}
		}

		bool down = true;
		bool left = false;
		int gridX = tiles.GetLength(1);
		int gridY = tiles.GetLength(1);

		for (int j = 0; j < gridY; j++) {
			if (j % 2 == 0) {
				down = false;
				left = true;
			}
			else {
				down = true;
				left = false;
			}

			for (int i = 0; i < gridX; i++) {
				GameObject tile = tiles[i,j];

				if (i < gridX - 1) {


					HingeJoint hinge = createJoint(tile,  tiles[i+1,j], false, down);


					down = !down;
				}

				if (j < gridY - 1) {
					HingeJoint hinge = createJoint(tile,  tiles[i,j+1], true, left);
					left = !left;


				}


			}
		}

	}
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < tiles.GetLength(1); i++) {
			for (int j = 0; j < tiles.GetLength(1); j++) {
				GameObject tile = tiles[i,j];
				drawJoints(tile);

				// if (i < tiles.GetLength(1)-1) {
				// 	Debug.DrawLine(tile.GetComponent<Renderer>().bounds.center,  tiles[i+1,j].GetComponent<Renderer>().bounds.center);
				// }
				// if (j < tiles.GetLength(1)-1) {
				// 	Debug.DrawLine(tile.GetComponent<Renderer>().bounds.center,  tiles[i,j+1].GetComponent<Renderer>().bounds.center);
				// }

			}
		}

	}

	HingeJoint createJoint(GameObject cube, GameObject connectedCube, bool verticle, bool down) {
		HingeJoint hinge = cube.AddComponent<HingeJoint>();

		hinge.autoConfigureConnectedAnchor = true;

		hinge.connectedBody = connectedCube.GetComponent( typeof(Rigidbody) ) as Rigidbody;

 		hinge.anchor = cube.GetComponent<Renderer>().bounds.center;
		hinge.connectedAnchor = connectedCube.GetComponent<Renderer>().bounds.center;

		float offset = cube.transform.localScale.x/2;

		// if (verticle) {
		// 	if (down) { //left
		// 		hinge.anchor += new Vector3(-offset,0,0);
		// 		hinge.connectedAnchor = new Vector3(-offset,-offset,0);
		// 	}
		// 	else {
		// 		hinge.anchor += new Vector3(offset,0,0);
		// 		hinge.connectedAnchor = new Vector3(offset,-offset,0);
		// 	}
		// }
		// else {
		// 	if (down) {
		// 		hinge.anchor += new Vector3(offset,-offset * 2,0);
		// 		hinge.connectedAnchor = new Vector3(-offset,-offset,0);
		// 	}
		// 	else {
		// 		hinge.anchor += new Vector3(offset,0,0);
		// 		hinge.connectedAnchor = new Vector3(-offset,offset,0);
		//
		// 	}
		// }

		// hinge.useSpring = true;
		// JointSpring spring = hinge.spring;
		// spring.spring = springValue;
		// spring.damper = damperValue;
		// //spring.targetPosition = 70;
		// hinge.spring = spring;


		hinge.enableCollision = true;
		hinge.axis = new Vector3(0,1,0);

		return hinge;

	}

	void drawJoints(GameObject panel) {
		HingeJoint[] HingeJoints = panel.GetComponents<HingeJoint>();

		foreach (HingeJoint joint in HingeJoints) {


			Debug.DrawLine(joint.anchor, joint.connectedAnchor);

		}
	}


	GameObject[,] groupComponents() {
		GameObject[,] panels;
		GameObject grid = Instantiate(Resources.Load("circlTest", typeof(GameObject))) as GameObject;

		grid.transform.Rotate(new Vector3(90,0,0));

		List<GameObject> squares = new List<GameObject>();
		List<GameObject> circles = new List<GameObject>();


		foreach (Transform panel in grid.GetComponentsInChildren<Transform>()) {
			if (panel.name.Contains("panel")) squares.Add(panel.gameObject);
			if (panel.name.Contains("circle")) circles.Add(panel.gameObject);
		}

		int circleIndex = 0;

		foreach(GameObject square in squares) {

			for (int i = circleIndex; i < circleIndex + 4; i++) {
				circles[i].transform.parent = square.transform;
			}
			circleIndex+=4;
		}


		int dim = (int)Mathf.Sqrt(squares.Count);

		panels = new GameObject[dim, dim];

		int counter = 0;

		for(int i = 0; i < dim; i++) {
			for(int j = 0; j < dim; j++) {
				panels[i,j] = squares[counter];
				panels[i,j].transform.position = panels[i,j].GetComponent<MeshRenderer>().bounds.center/2;
				counter++;
			}
		}
		return panels;
	}
}

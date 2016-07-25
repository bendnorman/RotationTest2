using UnityEngine;
using System.Collections;

public class Cubes : MonoBehaviour {
	public int gridX;
	public int gridY;
	public int spacing;
	public int springValue;
	public int damperValue;

	private GameObject[,] panels	;
	// Use this for initialization


	void Start () {
			panels = new GameObject[gridX, gridY];

			for (int i = 1; i <= gridX; i++) {
				for (int j = 1; j <= gridY; j++) {
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

					cube.transform.Translate(i * spacing,j * spacing, 0);

					Rigidbody panelRigidBody = cube.AddComponent<Rigidbody>();
					panelRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
					panelRigidBody.useGravity = false;

					BoxCollider panelBoxCollider = cube.AddComponent<BoxCollider>();

					panels[i-1,j-1] = cube;
				}
			}

			bool down = true;
			bool left = false;

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
					GameObject panel = panels[i,j];


					if (i < gridX - 1) {
						CharacterJoint hinge = createJoint(panel,  panels[i+1,j], false, down);

						down = !down;
					}

					if (j < gridY - 1) {
						CharacterJoint hinge = createJoint(panel,  panels[i,j+1], true, left);
						left = !left;


					}


				}
			}



	}

	CharacterJoint createJoint(GameObject cube, GameObject connectedCube, bool verticle, bool down) {
		CharacterJoint hinge = cube.AddComponent<CharacterJoint>();

		hinge.autoConfigureConnectedAnchor = false;

		hinge.connectedBody = connectedCube.GetComponent( typeof(Rigidbody) ) as Rigidbody;

		float offset = cube.transform.localScale.x/2;

		if (verticle) {
			if (down) { //left
				hinge.anchor += new Vector3(-offset,0,0);
				hinge.connectedAnchor = new Vector3(-offset,-offset,0);
			}
			else {
				hinge.anchor += new Vector3(offset,0,0);
				hinge.connectedAnchor = new Vector3(offset,-offset,0);
			}
		}
		else {
			if (down) {
				hinge.anchor += new Vector3(offset,-offset * 2,0);
				hinge.connectedAnchor = new Vector3(-offset,-offset,0);
			}
			else {
				hinge.anchor += new Vector3(offset,0,0);
				hinge.connectedAnchor = new Vector3(-offset,offset,0);

			}
		}

		hinge.enableCollision = true;

		return hinge;

	}

	// Update is called once per frame
	void Update () {

		for (int i = 0; i < gridX; i++) {
			for (int j = 0; j < gridY; j++) {
				GameObject panel = panels[i,j];
				drawJoints(panel);

			}
		}

	}

	void drawJoints(GameObject panel) {
		CharacterJoint[] CharacterJoints = panel.GetComponents<CharacterJoint>();
		foreach (CharacterJoint joint in CharacterJoints) {

			Debug.DrawLine(panel.transform.position, joint.connectedBody.transform.position);

		}
	}


}

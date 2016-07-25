using UnityEngine;
using System.Collections;

public class Cubes : MonoBehaviour {
	public int gridX;
	public int gridY;
	public int spacing;
	public int springValue;
	public int damperValue;
	public int SphereSize;

	private GameObject[,] panels	;
	// Use this for initialization


	void Start () {
			panels = new GameObject[gridX, gridY];

			for (int i = 1; i <= gridX; i++) {
				for (int j = 1; j <= gridY; j++) {
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

					cube.transform.Translate(i * spacing,j * spacing, 0);
					cube.transform.localScale = new Vector3(1,1,0.2f);
					cube.transform.parent = gameObject.transform;

					cube.GetComponent<Renderer>().material.color = new Color(0.5f,0,1);

					Rigidbody panelRigidBody = cube.AddComponent<Rigidbody>();
					panelRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
					panelRigidBody.useGravity = true;

					BoxCollider panelBoxCollider = cube.AddComponent<BoxCollider>();

					panels[i-1,j-1] = cube;
				}
			}

			gameObject.transform.localRotation =  Quaternion.Euler(0,0,0);
			gameObject.transform.localPosition -= new Vector3(0,5,0);


			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = getCenterOfGrid();
			sphere.transform.localScale = new Vector3(SphereSize,SphereSize,SphereSize);
			//sphere.GetComponent<Renderer>().material.color = new Color(0.5f,0,0);

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

	Vector3 getCenterOfGrid() {
		Vector3 low = panels[0,0].transform.position;
		Vector3 high = panels[gridX-1,0].transform.position;

		float x = low.x + (high.x - low.x)/2;

		high = panels[0,gridX-1].transform.position;

		float z = low.z + (high.z - low.z)/2;
		return new Vector3(x, 0, z);
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

		float limit = 200;

		hinge.enableCollision = true;
		//////twistLimitSpring////////////
    SoftJointLimitSpring spring = hinge.twistLimitSpring;
		spring.spring = springValue;
		spring.damper = damperValue;
		hinge.twistLimitSpring = spring;

		//////TwistLimits///////
		SoftJointLimit highTwistLimit = hinge.highTwistLimit;
		highTwistLimit.bounciness = 0;
		highTwistLimit.limit = limit;
		highTwistLimit.contactDistance = 100;
		hinge.highTwistLimit = highTwistLimit;

		SoftJointLimit lowTwistLimit = hinge.lowTwistLimit;
		lowTwistLimit.bounciness = 0;
		lowTwistLimit.limit = limit;
		lowTwistLimit.contactDistance = 100;
		hinge.lowTwistLimit = lowTwistLimit;

		//////swing1Limit//////
		SoftJointLimit swing1Limit = hinge.swing1Limit;
		swing1Limit.bounciness = 0;
		swing1Limit.limit = 0;
		swing1Limit.contactDistance = 100;
		hinge.swing1Limit = swing1Limit;

		//////swingLimitSpring////////////
    SoftJointLimitSpring swingLimitSpring = hinge.swingLimitSpring;
		swingLimitSpring.spring = springValue;
		swingLimitSpring.damper = damperValue;
		hinge.swingLimitSpring = swingLimitSpring;

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

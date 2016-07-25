using UnityEngine;
using System.Collections;



public class Grid : MonoBehaviour {
	private SpriteRenderer spriteRenderer;
	public int gridX;
	public int gridY;
	public int spacing;
	public int springValue;
	public int damperValue;

	private GameObject[,] panels;
	// Use this for initialization
	void Start () {



		Sprite rectangle = Resources.Load <Sprite> ("rect");

		panels = new GameObject[gridX, gridY];

		for (int i = 1; i <= gridX; i++) {
			for (int j = 1; j <= gridY; j++) {
				GameObject obj = new GameObject();
				SpriteRenderer sR = obj.AddComponent<SpriteRenderer>();

				Rigidbody panelRigidBody = obj.AddComponent<Rigidbody>();
				panelRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				panelRigidBody.useGravity = false;



				sR.sprite = rectangle;

				BoxCollider panelBoxCollider = obj.AddComponent<BoxCollider>();
				panelBoxCollider.size = new Vector3(sR.bounds.size.x, sR.bounds.size.y,0);

				obj.transform.Translate(i * spacing,j * spacing,0);
				obj.transform.parent = gameObject.transform;

				panels[i-1,j-1] = obj;
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
					Debug.Log(panels[i+1,j].transform.position);

					HingeJoint hinge = createJoint(panel,  panels[i+1,j], false, down);

					down = !down;
				}

				if (j < gridY - 1) {
					HingeJoint hinge = createJoint(panel,  panels[i,j+1], true, left);
					left = !left;
					//HingeJoint hinge = createJoint(panel,  panels[i,j+1], down);
					// hinge.connectedBody = panels[i,j+1].GetComponent( typeof(Rigidbody) ) as Rigidbody;

				}


			}
		}






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

	HingeJoint createJoint(GameObject panel, GameObject connectedPanel, bool verticle, bool down) {

		Vector3 panelPosition =	panel.transform.position;
		Vector3 panelScale = panel.transform.localScale;
		SpriteRenderer spriteRender = panel.GetComponent<SpriteRenderer>();


		HingeJoint hinge = panel.AddComponent<HingeJoint>();

		hinge.autoConfigureConnectedAnchor = false;

		hinge.connectedBody = connectedPanel.GetComponent( typeof(Rigidbody) ) as Rigidbody;

		float offset = spriteRender.bounds.size.x/2;

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



		// hinge.anchor += new Vector3(0,-spriteRender.bounds.size.y/2,0);

		hinge.axis = new Vector3(0,0,1);

		hinge.useSpring = true;
		JointSpring spring = hinge.spring;
		spring.spring = springValue;
		spring.damper = damperValue;
		//spring.targetPosition = 70;
		hinge.spring = spring;

		hinge.enableCollision = true;

		return hinge;
	}

	void drawJoints(GameObject panel) {
		HingeJoint[] hingeJoints = panel.GetComponents<HingeJoint>();
		foreach (HingeJoint joint in hingeJoints) {

			Debug.DrawLine(panel.transform.position, joint.connectedBody.transform.position);

		}
	}

	void drawCircleJoints(GameObject panel) {
		HingeJoint[] hingeJoints = panel.GetComponents<HingeJoint>();
		foreach (HingeJoint joint in hingeJoints) {

			Gizmos.DrawSphere(panel.transform.position + joint.anchor, 10);

		}
	}
}

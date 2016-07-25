using UnityEngine;
using System.Collections;

public class Import : MonoBehaviour {
	private GameObject[,] panels;
	public int springValue;
	public int damperValue;

	// Use this for initialization
	void Start () {
		 GameObject fullModel = Instantiate(Resources.Load("justPanelObject", typeof(GameObject))) as GameObject;
		 Debug.Log("FullModel" + fullModel.transform.position);
		 Debug.Log("gameObject" + gameObject.transform.position);


			Vector3[] coordinates = new Vector3[fullModel.GetComponentsInChildren<Renderer>().Length];

			int counter = 0;
			foreach (Renderer panel in fullModel.GetComponentsInChildren<Renderer>()) {
				coordinates[counter] = panel.bounds.center;
				counter++;

			}



			GameObject[] panelsFlat = new GameObject[coordinates.Length];

			Destroy(fullModel);

			for (int i = 0; i < panelsFlat.Length; i++) {
				GameObject onePanel = Instantiate(Resources.Load("onePanel", typeof(GameObject))) as GameObject;
				onePanel.transform.position = coordinates[i];
				onePanel.transform.GetChild(0).localPosition -= new Vector3(0,125,0);



				onePanel.transform.parent = gameObject.transform;

				Rigidbody panelRigidBody = onePanel.AddComponent<Rigidbody>();
				panelRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				panelRigidBody.useGravity = false;

				panelsFlat[i] = onePanel;

			}

			int dim = (int)Mathf.Sqrt(panelsFlat.Length);
			panels = new GameObject[dim, dim];


			counter = 0;

			for (int i = 0; i < dim; i++) {
				for (int j = 0; j < dim; j++) {
					panels[i,j] = panelsFlat[counter];
					counter++;

				}
			}

			for (int i = 0; i < dim; i++) {
				for (int j = 0; j < dim; j++) {
					GameObject panel = panels[i,j];

					if (i < panels.GetLength(1) - 1) {
						createJoint(panel, panels[i+1,j], true, true);
					}

					if (j < panels.GetLength(1) - 1) {
						createJoint(panel, panels[i,j+1], true, true);
					}

				}
			}



	}

	// Update is called once per frame
	void Update () {
		for (int i = 0; i < panels.GetLength(1); i++) {
			for (int j = 0; j < panels.GetLength(1); j++) {
				// Debug.Log("transfrom.position" + panels[i,j].transform.position);
				// Debug.Log("bounds.center" + panels[i,j].transform.GetChild(0).GetComponent<Renderer>().bounds.center);

				if (i < panels.GetLength(1) - 1) {
				   drawConnections(panels[i,j], panels[i+1,j]);
				}

				if (j < panels.GetLength(1) - 1) {
					drawConnections(panels[i,j], panels[i,j+1]);

				}
			}
		}
	}

	HingeJoint createJoint(GameObject cube, GameObject connectedCube, bool verticle, bool down) {
		HingeJoint hinge = cube.AddComponent<HingeJoint>();

		hinge.autoConfigureConnectedAnchor = true;

		hinge.connectedBody = connectedCube.GetComponent( typeof(Rigidbody) ) as Rigidbody;



		float offset = cube.transform.localScale.x/2;


		hinge.enableCollision = true;
		hinge.axis = new Vector3(0,0,1);

		hinge.useSpring = true;
		JointSpring spring = hinge.spring;
		spring.spring = springValue;
		spring.damper = damperValue;
		//spring.targetPosition = 70;
		hinge.spring = spring;

		return hinge;

	}



	void drawConnections(GameObject panel, GameObject otherPanel) {
		Debug.DrawLine(panel.transform.position, otherPanel.transform.position);

	}
}

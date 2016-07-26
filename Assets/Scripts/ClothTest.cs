using UnityEngine;
using System.Collections;

public class ClothTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
				gameObject.transform.localScale = new Vector3(300,300,300);

				Cloth cloth = gameObject.AddComponent<Cloth>();
				cloth.useGravity = false;
				SkinnedMeshRenderer skin = gameObject.GetComponent<SkinnedMeshRenderer>();
				skin.sharedMesh = CreatePrimitiveMesh(PrimitiveType.Cube);

	}

	// Update is called once per frame
	void Update () {

	}

	Mesh CreatePrimitiveMesh(PrimitiveType type) {
			GameObject gameObject = GameObject.CreatePrimitive(type);
			 Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			 GameObject.Destroy(gameObject);
			 return mesh;

	}
}

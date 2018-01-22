using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which handles a node's visualisation.
/// </summary>
public class NodeVisualiser : MonoBehaviour
{
	private Dictionary<Node.NodeType, Quaternion> rotationMap;
	private Quaternion targetRotation;

	/// <summary>
	/// When this node is instantiated, set rotations corresponding to the faces of the cube and map them 
	/// to the enumerated node types.
	/// </summary>
	void Awake ()
	{
		rotationMap = new Dictionary<Node.NodeType, Quaternion> ();
		rotationMap.Add (Node.NodeType.Start, Quaternion.Euler (new Vector3 (0, 0, 90)));
		rotationMap.Add (Node.NodeType.Goal, Quaternion.Euler (new Vector3 (0, 0, -90)));
		rotationMap.Add (Node.NodeType.Open, Quaternion.Euler (new Vector3 (0, 0, 0)));
		rotationMap.Add (Node.NodeType.Wall, Quaternion.Euler (new Vector3 (0, 0, 180)));
		rotationMap.Add (Node.NodeType.Explored, Quaternion.Euler (new Vector3 (90, 0, 0)));
		rotationMap.Add (Node.NodeType.Chosen, Quaternion.Euler (new Vector3 (-90, 0, 0)));
	}


	/// <summary>
	/// Set the visualisation for this node.
	/// </summary>
	public void Visualise (Node.NodeType type)
	{
		targetRotation = rotationMap [type];
		StartCoroutine (RotateToTarget ());
	}


	/// <summary>
	/// A coroutine which rotates this node to targetRotation.
	/// </summary>
	/// <param name="targetRotation">Target rotation.</param>
	private IEnumerator RotateToTarget ()
	{
		float rotationSpeed = 3.0f;
		float threshold = 0.01f;

		while (Quaternion.Angle (transform.rotation, targetRotation) > threshold) {
			transform.rotation = Quaternion.Lerp (
				transform.rotation, 
				targetRotation, 
				Time.deltaTime * rotationSpeed);
			yield return null;
		}

		// Set the rotation to the exact value after approximating it in the loop.
		transform.rotation = targetRotation;
	}
}

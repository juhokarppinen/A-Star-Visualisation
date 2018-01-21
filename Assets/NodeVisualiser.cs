using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which handles a node's visualisation.
/// </summary>
public class NodeVisualiser : MonoBehaviour
{
	private Quaternion startRotation;
	private Quaternion goalRotation;
	private Quaternion openRotation;
	private Quaternion wallRotation;
	private Quaternion exploredRotation;
	private Quaternion chosenRotation;
	private Quaternion targetRotation;

	private Dictionary<Node.NodeType, Quaternion> VisualisationMap;


	/// <summary>
	/// When this node is instantiated, set rotations corresponding to the faces of the cube and map them 
	/// to the enumerated node types.
	/// </summary>
	void Awake ()
	{
		VisualisationMap = new Dictionary<Node.NodeType, Quaternion> ();

		startRotation = Quaternion.Euler (new Vector3 (0, 0, 90));
		goalRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
		openRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		wallRotation = Quaternion.Euler (new Vector3 (0, 0, 180));
		exploredRotation = Quaternion.Euler (new Vector3 (90, 0, 0));
		chosenRotation = Quaternion.Euler (new Vector3 (-90, 0, 0));

		VisualisationMap.Add (Node.NodeType.Start, startRotation);
		VisualisationMap.Add (Node.NodeType.Goal, goalRotation);
		VisualisationMap.Add (Node.NodeType.Open, openRotation);
		VisualisationMap.Add (Node.NodeType.Wall, wallRotation);
		VisualisationMap.Add (Node.NodeType.Explored, exploredRotation);
		VisualisationMap.Add (Node.NodeType.Chosen, chosenRotation);
	}


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Alpha1))
			Visualise (Node.NodeType.Start);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			Visualise (Node.NodeType.Goal);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			Visualise (Node.NodeType.Open);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			Visualise (Node.NodeType.Wall);
		if (Input.GetKeyDown (KeyCode.Alpha5))
			Visualise (Node.NodeType.Explored);
		if (Input.GetKeyDown (KeyCode.Alpha6))
			Visualise (Node.NodeType.Chosen);
		
	}


	/// <summary>
	/// Set the visualisation for this node.
	/// </summary>
	public void Visualise (Node.NodeType type)
	{
		targetRotation = VisualisationMap [type];
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

		// Set the rotation to an exact value in case the loop undershoots the target rotation.
		transform.rotation = targetRotation;
	}
}

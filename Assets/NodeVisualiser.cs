using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which handles the node's visualisation.
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

	private Dictionary<VisualisationType, Quaternion> VisualisationMap;

	/// <summary>
	/// Enumerated visualisation types for a node.
	/// </summary>
	public enum VisualisationType
	{
		Start,
		Goal,
		Open,
		Wall,
		Explored,
		Chosen}
	;


	/// <summary>
	/// When this node is instantiated, set rotations corresponding to the faces of the cube and map them 
	/// to the enumerated visualisation types.
	/// </summary>
	void Start ()
	{
		VisualisationMap = new Dictionary<VisualisationType, Quaternion> ();

		startRotation = Quaternion.Euler (new Vector3 (0, 0, 90));
		goalRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
		openRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		wallRotation = Quaternion.Euler (new Vector3 (0, 0, 180));
		exploredRotation = Quaternion.Euler (new Vector3 (-90, 0, 0));
		chosenRotation = Quaternion.Euler (new Vector3 (90, 0, 0));

		VisualisationMap.Add (VisualisationType.Start, startRotation);
		VisualisationMap.Add (VisualisationType.Goal, goalRotation);
		VisualisationMap.Add (VisualisationType.Open, openRotation);
		VisualisationMap.Add (VisualisationType.Wall, wallRotation);
		VisualisationMap.Add (VisualisationType.Explored, exploredRotation);
		VisualisationMap.Add (VisualisationType.Chosen, chosenRotation);
	}


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Alpha1))
			Visualise (VisualisationType.Start);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			Visualise (VisualisationType.Goal);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			Visualise (VisualisationType.Open);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			Visualise (VisualisationType.Wall);
		if (Input.GetKeyDown (KeyCode.Alpha5))
			Visualise (VisualisationType.Explored);
		if (Input.GetKeyDown (KeyCode.Alpha6))
			Visualise (VisualisationType.Chosen);
		
	}


	/// <summary>
	/// Set the visualisation for this node.
	/// </summary>
	public void Visualise (VisualisationType type)
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
		float rotationSpeed = 10.0f;
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

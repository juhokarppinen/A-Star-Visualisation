using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Text wallPercentage;

	private LevelManager levelGenerator;
	private Vector3 startPosition;
	private Vector3 goalPosition;
	private int maxX;
	private int maxZ;

	/// <summary>
	/// Initialize the Game Manager and generate the grid.
	/// </summary>
	void Start ()
	{
		levelGenerator = FindObjectOfType<LevelManager> ();
		maxX = levelGenerator.GridWidth - 1;
		maxZ = levelGenerator.GridHeight - 1;
		startPosition = Vector3.zero;
		goalPosition = new Vector3 (maxX, 0, maxZ);

		levelGenerator.InstantiateGrid ();
		RandomizeGrid ();
	}


	/// <summary>
	/// Randomizes the grid until a valid path is found.
	/// </summary>
	private void RandomizeGrid ()
	{
		do {
			levelGenerator.InitializeGrid (startPosition, goalPosition);
		} while (!AStar.FindPath (levelGenerator.StartNode, levelGenerator.GoalNode));

	}


	/// <summary>
	/// Handle keyboard input and update visualisation accordingly.
	/// </summary>
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.W))
			startPosition = BoundedSum (startPosition, Vector3.forward);
		if (Input.GetKeyDown (KeyCode.S))
			startPosition = BoundedSum (startPosition, Vector3.back);
		if (Input.GetKeyDown (KeyCode.A))
			startPosition = BoundedSum (startPosition, Vector3.left);
		if (Input.GetKeyDown (KeyCode.D))
			startPosition = BoundedSum (startPosition, Vector3.right);

		if (Input.GetKeyDown (KeyCode.UpArrow))
			goalPosition = BoundedSum (goalPosition, Vector3.forward);
		if (Input.GetKeyDown (KeyCode.DownArrow))
			goalPosition = BoundedSum (goalPosition, Vector3.back);
		if (Input.GetKeyDown (KeyCode.LeftArrow))
			goalPosition = BoundedSum (goalPosition, Vector3.left);
		if (Input.GetKeyDown (KeyCode.RightArrow))
			goalPosition = BoundedSum (goalPosition, Vector3.right);

		if (Input.GetKeyDown (KeyCode.Space))
			RandomizeGrid ();

		if (Input.GetKeyDown (KeyCode.Alpha0))
			SetWallPercentage (0);
		if (Input.GetKeyDown (KeyCode.Alpha1))
			SetWallPercentage (0.1f);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			SetWallPercentage (0.2f);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			SetWallPercentage (0.3f);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			SetWallPercentage (0.4f);
		if (Input.GetKeyDown (KeyCode.Alpha5))
			SetWallPercentage (0.5f);
		if (Input.GetKeyDown (KeyCode.Alpha6))
			SetWallPercentage (0.6f);
		if (Input.GetKeyDown (KeyCode.Alpha7))
			SetWallPercentage (0.7f);
		if (Input.GetKeyDown (KeyCode.Alpha8))
			SetWallPercentage (0.8f);
		if (Input.GetKeyDown (KeyCode.Alpha9))
			SetWallPercentage (0.9f);

		levelGenerator.MoveStartNode (startPosition);
		levelGenerator.MoveGoalNode (goalPosition);
		AStar.FindPath (levelGenerator.StartNode, levelGenerator.GoalNode);
	}


	/// <summary>
	/// Return the sum of the two vectors if the resulting vector is within the grid. 
	/// Otherwise return the original vector.
	/// </summary>
	private Vector3 BoundedSum (Vector3 original, Vector3 direction)
	{
		// Prevent start and goal nodes from overriding one another.
		if (original + direction == startPosition || original + direction == goalPosition)
			return original;

		// Make sure the nodes stay within the boundaries.
		float x = original.x + direction.x;
		float z = original.z + direction.z;
		if (x < 0 || x > maxX || z < 0 || z > maxZ) {
			return original;
		} else {
			return new Vector3 (x, 0, z);
		}
	}


	/// <summary>
	/// Sets the wall percentage and updates the UI.
	/// </summary>
	private void SetWallPercentage (float percentage)
	{
		levelGenerator.PercentageOfWalls = percentage;
		wallPercentage.text = "Walls: " + percentage * 100 + "%";
	}
}

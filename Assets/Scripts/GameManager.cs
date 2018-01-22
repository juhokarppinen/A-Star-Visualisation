using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private LevelGenerator levelGenerator;
	private Vector3 startPosition;
	private Vector3 goalPosition;
	private int maxX;
	private int maxZ;

	/// <summary>
	/// Initialize the Game Manager and generate the grid.
	/// </summary>
	void Start ()
	{
		levelGenerator = FindObjectOfType<LevelGenerator> ();
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

		if (Input.GetKeyDown (KeyCode.R))
			RandomizeGrid ();

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
		float x = original.x + direction.x;
		float z = original.z + direction.z;
		if (x < 0 || x > maxX || z < 0 || z > maxZ) {
			return original;
		} else {
			return new Vector3 (x, 0, z);
		}
	}
}

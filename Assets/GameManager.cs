﻿using PathFinding;
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

	// Use this for initialization
	void Start ()
	{
		levelGenerator = FindObjectOfType<LevelGenerator> ();
		maxX = levelGenerator.gridWidth - 1;
		maxZ = levelGenerator.gridHeight - 1;
		startPosition = new Vector3 (0, 0, 0);
		goalPosition = new Vector3 (maxX, 0, maxZ);
	}


	/// <summary>
	/// Handle keyboard input and update visualisation accordingly.
	/// </summary>
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			AStar.FindPath (levelGenerator.StartNode, levelGenerator.GoalNode);

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


		levelGenerator.MoveStartNode (startPosition);
		levelGenerator.MoveGoalNode (goalPosition);
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
using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private LevelGenerator levelGenerator;

	// Use this for initialization
	void Start ()
	{
		levelGenerator = FindObjectOfType<LevelGenerator> ();
	}


	/// <summary>
	/// Handle keyboard input.
	/// </summary>
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			AStar.FindPath (levelGenerator.StartNode, levelGenerator.GoalNode);
		}
	}
}

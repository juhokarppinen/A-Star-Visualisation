using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which handles level generation and data storage.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
	[Range (2, 20)]
	public int gridWidth;
	[Range (2, 20)]
	public int gridHeight;
	[Range (0, 0.5f)]
	public float percentageOfWalls;
	public bool randomizeStartAndGoal;
	public GameObject nodeObject;

	private GameObject[,] nodes;
	private Node startNode;
	private Node goalNode;

	public Node StartNode {
		get { return startNode; }
	}

	public Node GoalNode {
		get { return goalNode; }
	}

	/// <summary>
	/// Generates a grid of nodes with start, goal and random walls.
	/// </summary>
	void Start ()
	{
		GenerateGrid ();
		startNode = SetNode (Node.NodeType.Start, 0, 0);
		goalNode = SetNode (Node.NodeType.Goal, gridHeight - 1, gridWidth - 1);
		CreateWalls ();
		ConnectNeighbors ();
	}


	/// <summary>
	/// Generates the initial grid of nodes.
	/// </summary>
	private void GenerateGrid ()
	{
		nodes = new GameObject[gridHeight, gridWidth];

		for (var z = 0; z < gridHeight; z++) {
			for (var x = 0; x < gridWidth; x++) {
				var newNode = (GameObject)Instantiate (nodeObject, new Vector3 (x, 0, z), Quaternion.identity);
				newNode.transform.parent = FindObjectOfType<LevelGenerator> ().transform;
				nodes [z, x] = newNode;
			}
		}
	}


	/// <summary>
	/// Sets a node's type if the chosen recipient's type is "Open".
	/// </summary>
	private Node SetNode (Node.NodeType type, int defaultX, int defaultZ)
	{
		int x;
		int z;
		if (randomizeStartAndGoal) {
			x = Random.Range (0, gridWidth - 1);
			z = Random.Range (0, gridHeight - 1);
		} else {
			x = defaultX;
			z = defaultZ;
		}

		Node recipient = nodes [z, x].GetComponent<Node> ();
		if (recipient.Type == Node.NodeType.Open) {
			recipient.Type = type;
		} else {
			// Call the method recursively until an open node is found.
			SetNode (type, defaultX, defaultZ);
		}
		return recipient;
	}


	/// <summary>
	/// Create the walls randomly. Walls can't overlap with start and goal nodes.
	/// </summary>
	private void CreateWalls ()
	{
		var amountOfWalls = percentageOfWalls * gridHeight * gridWidth;

		for (var i = 0; i < amountOfWalls; i++) {
			int x;
			int z;
			Node.NodeType candidateType;

			do {
				x = Random.Range (0, gridWidth);
				z = Random.Range (0, gridHeight);
				candidateType = nodes [z, x].GetComponent<Node> ().Type;
			} while (candidateType == Node.NodeType.Start || candidateType == Node.NodeType.Goal);

			nodes [z, x].GetComponent<Node> ().Type = Node.NodeType.Wall;
		}
	}


	/// <summary>
	/// Make all neighbor connections on the grid.
	/// </summary>
	private void ConnectNeighbors ()
	{
		foreach (var node in nodes) {
			int nodeX = (int)node.transform.position.x;
			int nodeZ = (int)node.transform.position.z;
			for (var z = nodeZ - 1; z <= nodeZ + 1; z++) {
				for (var x = nodeX - 1; x <= nodeX + 1; x++) {
					Connect (node, x, z);
				}
			}
		}
	}


	/// <summary>
	/// Make a connection between the specified node and another node at nodes[nodeY,nodeX].
	/// </summary>
	private void Connect (GameObject node, int otherX, int otherZ)
	{
		// Avoid index out of bounds errors.
		if (otherX < 0 || otherX >= gridWidth || otherZ < 0 || otherZ >= gridHeight)
			return;

		Node thisNode = node.GetComponent<Node> ();
		Node otherNode = nodes [otherZ, otherX].GetComponent<Node> ();

		// Walls are ignored.
		if (thisNode.Type == Node.NodeType.Wall || otherNode.Type == Node.NodeType.Wall)
			return;

		// A node can't connect to itself.
		if (thisNode == otherNode)
			return;
		
		// Create a valid connection.
		thisNode.AddConnection (otherNode);
	}
}

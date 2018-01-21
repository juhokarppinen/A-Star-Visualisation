using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


	/// <summary>
	/// Generates a grid of nodes with start, goal and random walls.
	/// </summary>
	void Start ()
	{
		GenerateGrid ();
		SetNode (Node.NodeType.Start, 0, 0);
		SetNode (Node.NodeType.Goal, gridHeight - 1, gridWidth - 1);
		CreateWalls ();
		ConnectNeighbors ();
	}


	/// <summary>
	/// Generates the initial grid of nodes.
	/// </summary>
	private void GenerateGrid ()
	{
		nodes = new GameObject[gridHeight, gridWidth];

		for (var y = 0; y < gridHeight; y++) {
			for (var x = 0; x < gridWidth; x++) {
				var newNode = (GameObject)Instantiate (nodeObject, new Vector3 (x, 0, y), Quaternion.identity);
				newNode.transform.parent = FindObjectOfType<LevelGenerator> ().transform;
				nodes [y, x] = newNode;
			}
		}
	}


	/// <summary>
	/// Sets a node's type if the chosen recipient's type is "Open".
	/// </summary>
	/// <param name="type">Type.</param>
	private void SetNode (Node.NodeType type, int defaultX, int defaultY)
	{
		int x;
		int y;
		if (randomizeStartAndGoal) {
			x = Random.Range (0, gridWidth - 1);
			y = Random.Range (0, gridHeight - 1);
		} else {
			x = defaultX;
			y = defaultY;
		}

		Node recipient = nodes [y, x].GetComponent<Node> ();
		if (recipient.Type == Node.NodeType.Open) {
			recipient.Type = type;
		} else {
			// Call the method recursively until an open node is found.
			SetNode (type, defaultX, defaultY);
		}
	}


	/// <summary>
	/// Creates the walls randomly. Walls can't overlap with start and goal nodes.
	/// </summary>
	private void CreateWalls ()
	{
		var amountOfWalls = percentageOfWalls * gridHeight * gridWidth;

		for (var i = 0; i < amountOfWalls; i++) {
			int x;
			int y;
			Node.NodeType candidateType;

			do {
				x = Random.Range (0, gridWidth);
				y = Random.Range (0, gridHeight);
				candidateType = nodes [y, x].GetComponent<Node> ().Type;
			} while (candidateType == Node.NodeType.Start || candidateType == Node.NodeType.Goal);

			nodes [y, x].GetComponent<Node> ().Type = Node.NodeType.Wall;
		}
	}


	/// <summary>
	/// Connects the neighbors.
	/// </summary>
	private void ConnectNeighbors ()
	{
		foreach (var node in nodes) {
			int nodeX = (int)node.transform.position.x;
			int nodeY = (int)node.transform.position.y;
			for (var y = nodeY - 1; y <= nodeY + 1; y++) {
				for (var x = nodeX - 1; x <= nodeX + 1; x++) {
					Connect (node, x, y);
				}
			}
		}
	}


	/// <summary>
	/// Make a connection between the node and another node at nodes[nodeY,nodeX].
	/// </summary>
	/// <param name="node">Node.</param>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeY">Node y.</param>
	private void Connect (GameObject node, int nodeX, int nodeY)
	{
		// Handle array index out of bounds errors.
		if (nodeX < 0 || nodeX >= gridWidth || nodeY < 0 || nodeY >= gridHeight)
			return;

		// Walls are ignored.
		Node thisNode = node.GetComponent<Node> ();
		Node otherNode = nodes [nodeY, nodeX].GetComponent<Node> ();
		if (thisNode.Type == Node.NodeType.Wall || otherNode.Type == Node.NodeType.Wall)
			return;

		// A node can't connect to itself.
		if (thisNode == otherNode)
			return;
		
		// Create a valid connection.
		thisNode.AddConnection (otherNode);
	}


	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.Space)) {
			int x = Random.Range (0, gridWidth);
			int y = Random.Range (0, gridHeight);
			int i = Random.Range (0, 5);
			Node.NodeType vis;

			switch (i) {
			case 0:
				vis = Node.NodeType.Start;
				break;
			case 1:
				vis = Node.NodeType.Goal;
				break;
			case 2:
				vis = Node.NodeType.Open;
				break;
			case 3:
				vis = Node.NodeType.Wall;
				break;
			case 4:
				vis = Node.NodeType.Explored;
				break;
			case 5:
				vis = Node.NodeType.Chosen;
				break;
			default:
				vis = Node.NodeType.Open;
				break;
			}

			nodes [y, x].GetComponent<NodeVisualiser> ().Visualise (vis);
		}
	}
}

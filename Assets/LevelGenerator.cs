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
	/// <param name="type">Type.</param>
	private void SetNode (Node.NodeType type, int defaultX, int defaultZ)
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
	}


	/// <summary>
	/// Creates the walls randomly. Walls can't overlap with start and goal nodes.
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
	/// Connects the neighbors.
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
	/// Make a connection between the node and another node at nodes[nodeY,nodeX].
	/// </summary>
	/// <param name="node">Node.</param>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeZ">Node z.</param>
	private void Connect (GameObject node, int nodeX, int nodeZ)
	{
		// Handle array index out of bounds errors.
		if (nodeX < 0 || nodeX >= gridWidth || nodeZ < 0 || nodeZ >= gridHeight)
			return;

		Node thisNode = node.GetComponent<Node> ();
		Node otherNode = nodes [nodeZ, nodeX].GetComponent<Node> ();

		// Walls are ignored.
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
			int z = Random.Range (0, gridHeight);
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

			nodes [z, x].GetComponent<NodeVisualiser> ().Visualise (vis);
		}

		if (Input.GetKey (KeyCode.B)) {
			int x = Random.Range (0, gridWidth - 1);
			int z = Random.Range (0, gridHeight - 1);
			Node node = nodes [z, x].GetComponent<Node> ();
			if (node.Type == Node.NodeType.Wall)
				return;
			
			node.Type = Node.NodeType.Explored;

			foreach (Node neighbor in node.Connections.Keys) {
				neighbor.Type = Node.NodeType.Chosen;
			}
		}
	}
}

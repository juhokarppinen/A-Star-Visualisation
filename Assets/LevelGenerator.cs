using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which handles level generation and data storage.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
	[Range (0, 0.5f)]
	public float percentageOfWalls;
	public GameObject nodeObject;

	private int gridWidth = 20;
	private int gridHeight = 20;
	private GameObject[,] nodes;
	private Node startNode;
	private Node goalNode;

	public Node StartNode {
		get { return startNode; }
	}

	public Node GoalNode {
		get { return goalNode; }
	}

	public int GridWidth {
		get { return gridWidth; }
	}

	public int GridHeight {
		get { return gridHeight; }
	}


	/// <summary>
	/// Moves the start node and clear the path if the given position is different from the start node's position.
	/// </summary>
	public void MoveStartNode (Vector3 newPosition)
	{
		if (newPosition != startNode.transform.position) {
			SwitchStartNodeWith (GetNodeAt (newPosition));
			Clear (new List<Node.NodeType> () {
				Node.NodeType.Explored,
				Node.NodeType.Chosen
			});
		}
	}


	/// <summary>
	/// Moves the goal node and clear the path if the given position is different from the goal node's position.
	/// </summary>
	public void MoveGoalNode (Vector3 newPosition)
	{
		if (newPosition != goalNode.transform.position) {
			SwitchGoalNodeWith (GetNodeAt (newPosition));
			Clear (new List<Node.NodeType> () {
				Node.NodeType.Explored,
				Node.NodeType.Chosen
			});
		}
	}


	/// <summary>
	/// Returns the node at the specified position.
	/// </summary>
	private Node GetNodeAt (Vector3 position)
	{
		var x = (int)position.x;
		var z = (int)position.z;
		return nodes [z, x].GetComponent<Node> ();
	}


	/// <summary>
	/// Switchs the start node with the specified node.
	/// </summary>
	/// <param name="newNode">New node.</param>
	private void SwitchStartNodeWith (Node newNode)
	{
		startNode.Type = Node.NodeType.Open;
		newNode.Type = Node.NodeType.Start;
		ConnectNeighbors (newNode);
		startNode = newNode;
	}


	/// <summary>
	/// Switchs the goal node with the specified node.
	/// </summary>
	private void SwitchGoalNodeWith (Node newNode)
	{
		goalNode.Type = Node.NodeType.Open;
		newNode.Type = Node.NodeType.Goal;
		ConnectNeighbors (newNode);
		goalNode = newNode;	
	}


	/// <summary>
	/// Generates a grid of nodes with start, goal and random walls.
	/// </summary>
	void Start ()
	{
		InstantiateGrid ();
		InitialiseGrid (Vector3.zero, new Vector3 (gridWidth - 1, 0, gridHeight - 1));
	}


	/// <summary>
	/// Instantiates the initial grid of nodes.
	/// </summary>
	private void InstantiateGrid ()
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
	/// Initialises the nodes on the grid.
	/// </summary>
	public void InitialiseGrid (Vector3 startPosition, Vector3 goalPosition)
	{
		foreach (var item in FindObjectsOfType<Node>()) {
			item.Type = Node.NodeType.Open;
			item.ClearConnections ();
		}
		
		startNode = SetNode (Node.NodeType.Start, startPosition);
		goalNode = SetNode (Node.NodeType.Goal, goalPosition);
		CreateRandomWalls ();
		ConnectAllNeighbors ();
	}

	
	/// <summary>
	/// Sets a node's type if the chosen recipient's type is "Open".
	/// </summary>
	private Node SetNode (Node.NodeType type, Vector3 position)
	{
		var x = (int)position.x;
		var z = (int)position.z;
		Node recipient = nodes [z, x].GetComponent<Node> ();
		if (recipient.Type == Node.NodeType.Open) {
			recipient.Type = type;
		} else {
			// Call the method recursively until an open node is found.
			SetNode (type, position);
		}
		return recipient;
	}


	/// <summary>
	/// Create the walls randomly. Walls can't overlap with start and goal nodes.
	/// </summary>
	public void CreateRandomWalls ()
	{
		var amountOfWalls = percentageOfWalls * gridHeight * gridWidth;

		for (var i = 0; i < amountOfWalls; i++) {
			int x;
			int z;
			Node.NodeType candidateType;

			do {
				x = Random.Range (0, gridWidth - 1);
				z = Random.Range (0, gridHeight - 1);
				candidateType = nodes [z, x].GetComponent<Node> ().Type;
			} while (candidateType == Node.NodeType.Start || candidateType == Node.NodeType.Goal);

			nodes [z, x].GetComponent<Node> ().Type = Node.NodeType.Wall;
		}
	}


	/// <summary>
	/// Make all neighbor connections on the grid.
	/// </summary>
	public void ConnectAllNeighbors ()
	{
		foreach (var nodeContainer in nodes) {
			Node node = nodeContainer.GetComponent<Node> ();
			ConnectNeighbors (node);
		}
	}


	/// <summary>
	/// Connect neighbors to a single node.
	/// </summary>
	private void ConnectNeighbors (Node node)
	{
		int nodeX = (int)node.transform.position.x;
		int nodeZ = (int)node.transform.position.z;
		for (var z = nodeZ - 1; z <= nodeZ + 1; z++) {
			for (var x = nodeX - 1; x <= nodeX + 1; x++) {
				Connect (node, x, z);
			}
		}
	}


	/// <summary>
	/// Make a connection between the specified node and another node at nodes[nodeY,nodeX].
	/// </summary>
	private void Connect (Node node, int otherX, int otherZ)
	{
		// Avoid index out of bounds errors.
		if (otherX < 0 || otherX >= gridWidth || otherZ < 0 || otherZ >= gridHeight)
			return;

		Node otherNode = nodes [otherZ, otherX].GetComponent<Node> ();

		// Walls are ignored.
		if (node.Type == Node.NodeType.Wall || otherNode.Type == Node.NodeType.Wall)
			return;

		// A node can't connect to itself.
		if (node == otherNode)
			return;
		
		// Create a valid connection.
		node.AddConnection (otherNode);
	}


	/// <summary>
	/// Clear all nodes which have at least one of the specified types.
	/// </summary>
	public void Clear (List<Node.NodeType> types)
	{
		foreach (var nodeContainer in nodes) {
			Node node = nodeContainer.GetComponent<Node> ();
			if (types.Contains (node.Type))
				node.Type = Node.NodeType.Open;
		}
	}
}

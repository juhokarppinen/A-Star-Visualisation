using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which contains the abstraction of a single node.
/// </summary>
public class Node : MonoBehaviour
{
	public enum NodeType
	{
		Start,
		Goal,
		Open,
		Wall,
		Explored,
		Chosen}
	;

	private NodeType type = NodeType.Open;

	public NodeType Type {
		get { return type; }
		set { 
			type = value; 
			GetComponent<NodeVisualiser> ().Visualise (type);
		}
	}

	private Dictionary<Node,float> connections;

	public Dictionary<Node,float> Connections {
		get { return connections; }
	}


	/// <summary>
	/// Adds a connection to another node with a given edge length. 
	/// </summary>
	/// <param name="node">Node to connect.</param>
	/// <param name="edgeLength">Edge length between the nodes.</param>
	public void AddConnection (Node node, float edgeLength)
	{
		connections.Add (node, edgeLength);
	}


	/// <summary>
	/// Adds a connection to another node. The distance of the nodes is used as the edge length.
	/// </summary>
	/// <param name="node">Node to connect.</param>
	public void AddConnection (Node node)
	{
		connections.Add (node, Vector3.Distance (node.transform.position, transform.position));
	}


	void Awake ()
	{
		connections = new Dictionary<Node, float> ();
	}


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


}

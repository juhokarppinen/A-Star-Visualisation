using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Star : MonoBehaviour
{
	/// <summary>
	/// A simple default value wrapper for a use specific dictionary. The default value is float.MaxValue.
	/// </summary>
	class DefaultDict
	{
		Dictionary<Node,float> container;


		public DefaultDict ()
		{
			container = new Dictionary<Node,float> ();
		}


		public void Set (Node node, float value)
		{
			container [node] = value;
		}


		public float Get (Node node)
		{
			if (container.ContainsKey (node)) {
				return container [node];
			} else {
				return float.MaxValue;
			}
		}
	}


	/// <summary>
	/// Find the path from start to goal using the A* algorithm.
	/// </summary>
	/// <returns><c>true</c>, if path was found, <c>false</c> otherwise.</returns>
	public bool FindPath (Node start, Node goal)
	{
		var closedSet = new HashSet<Node> ();

		var openSet = new HashSet<Node> ();
		openSet.Add (start);

		var gScore = new DefaultDict ();
		gScore.Set (start, 0);

		var fScore = new DefaultDict ();
		fScore.Set (start, HeuristicEstimate (start, goal));

		var cameFrom = new Dictionary<Node, Node> ();

		Node current = start;
		while (openSet.Count > 0) {
			current = FindLowestFScore (openSet, current, fScore);
			if (current.Type != Node.NodeType.Start && current.Type != Node.NodeType.Goal) {
				current.Type = Node.NodeType.Explored;
			}
			if (current == goal) {
				ReconstructPath (cameFrom, current);
				return true;
			}
			openSet.Remove (current);
			closedSet.Add (current);

			foreach (var neighbor in current.Connections.Keys) {
				if (closedSet.Contains (neighbor)) {
					continue;
				}

				if (!openSet.Contains (neighbor)) {
					openSet.Add (neighbor);
				}

				var tentativeGScore = gScore.Get (current) + current.Connections [neighbor];
				if (tentativeGScore	>= gScore.Get (neighbor)) {
					continue;
				}

				cameFrom [neighbor] = current;
				gScore.Set (neighbor, tentativeGScore);
				fScore.Set (neighbor, gScore.Get (neighbor) + HeuristicEstimate (neighbor, goal));
			}
		}

		return false;
	}


	/// <summary>
	/// Find the node with the lowest F score.
	/// </summary>
	private Node FindLowestFScore (HashSet<Node> openSet, Node current, DefaultDict fScore)
	{
		var lowest = float.MaxValue;
		Node selectedNode = current;

		foreach (var node in openSet) {
			if (fScore.Get (node) < lowest) {
				selectedNode = node;
				lowest = fScore.Get (node);
			}
		}

		return selectedNode;
	}


	/// <summary>
	/// Use the Euclidean distance as a heuristic estimate.
	/// </summary>
	private float HeuristicEstimate (Node from, Node to)
	{
		return Vector3.Distance (from.transform.position, to.transform.position);
	}


	/// <summary>
	/// Reconstructs the path.
	/// </summary>
	private void ReconstructPath (Dictionary<Node,Node> cameFrom, Node current)
	{
		List<Node> totalPath = new List<Node> ();
		totalPath.Add (current);

		while (cameFrom.ContainsKey (current)) {
			current = cameFrom [current];
			totalPath.Add (current);
			if (current.Type != Node.NodeType.Start && current.Type != Node.NodeType.Goal) {
				current.Type = Node.NodeType.Chosen;
			}
		}
	}
}

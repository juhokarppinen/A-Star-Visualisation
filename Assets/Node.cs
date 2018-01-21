using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which contains the abstraction of a single node.
/// </summary>
public class Node : MonoBehaviour
{
	public float X {
		get { return transform.position.x; }
	}

	public float Y {
		get { return transform.position.y; }
	}

	public float Z {
		get { return transform.position.z; }
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

using UnityEngine;
using System.Collections;

public class DemoSeeker : MonoBehaviour
{
	void OnNewPath (Path path)
	{
		Debug.Log ("Received new Path from " + path.Nodes[0] + " to " + path.Nodes[path.Nodes.Count - 1] + ". Took " + path.SeekTime + " seconds.");
	}
}

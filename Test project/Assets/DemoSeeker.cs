using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Navigator))]
public class DemoSeeker : MonoBehaviour
{
	public Transform m_Target;
	
	private Path m_CurrentPath;
	
	
	void Start ()
	{
		GetComponent<Navigator> ().RegisterWeightHandler ("Water", OnHandleWaterWeight);
	}
	
	
	void OnNewPath (Path path)
	// When pathfinding via Navigator.targetPosition
	{
		Debug.Log ("Received new Path from " + path.StartNode + " to " + path.EndNode + ". Took " + path.SeekTime + " seconds.");
		m_CurrentPath = path;
	}
	
	
	void OnTargetUnreachable ()
	// When pathfinding via Navigator.targetPosition
	{
		Debug.Log ("Could not pathfind to target position");
		m_CurrentPath = null;
	}
	
	
	void OnPathAvailable (Path path)
	// When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	{
		Debug.Log ("Requested Path from " + path.StartNode + " to " + path.EndNode + " is now available. Took " + path.SeekTime + " seconds.");
	}
	
	
	void OnPathUnavailable ()
	// When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	{
		Debug.Log ("The requested path could not be established.");
	}
	
	
	void OnGUI ()
	{
		if (GUILayout.Button ("Pathfind"))
		{
			GetComponent<Navigator> ().targetPosition = m_Target.position;
		}
	}
	
	
	void OnDrawGizmos ()
	{
		if (m_CurrentPath == null)
		{
			return;
		}
		
		Gizmos.DrawLine (m_CurrentPath.StartPosition, m_CurrentPath.StartNode.Position);
		foreach (Connection connection in m_CurrentPath.Segments)
		{
			Gizmos.DrawLine (connection.From.Position, connection.To.Position);
		}
		Gizmos.DrawLine (m_CurrentPath.EndNode.Position, m_CurrentPath.EndPosition);
	}
	
	
	float OnHandleWaterWeight (object obj)
	{
		return 3.0f;
	}
}

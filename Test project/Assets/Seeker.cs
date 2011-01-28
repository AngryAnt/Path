using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate float WeightHandler (object obj);

public class Seeker
{
	private Navigator m_Owner;
	private Vector3 m_StartPosition, m_EndPosition;
	private int m_IterationCap;
	private double m_StartTime;
	
	
	public Seeker (Vector3 startPosition, Vector3 endPosition, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Owner = owner;
		m_IterationCap = Navigation.SeekerIterationCap;
	}
	
	
	public IEnumerator Seek ()
	{
		Waypoint startNode = null, endNode = null;
		
		m_StartTime = Time.realtimeSinceStartup;
		
		foreach (Waypoint waypoint in Navigation.Waypoints)
		{
			if (
				waypoint.Enabled &&
				(
					startNode == null ||
					(startNode.Position - m_StartPosition).sqrMagnitude > (waypoint.Position - m_StartPosition).sqrMagnitude
				)
			)
			{
				startNode = waypoint;
			}
			
			if (
				waypoint.Enabled &&
				(
					endNode == null ||
					(endNode.Position - m_EndPosition).sqrMagnitude > (waypoint.Position - m_EndPosition).sqrMagnitude
				)
			)
			{
				endNode = waypoint;
			}
		}
		
		if (startNode == endNode)
		{
			OnPathResult (new Path (m_StartPosition, m_EndPosition));
			yield break;
		}
		
		Dictionary<Connection, SeekerData> openSet = new Dictionary<Connection, SeekerData> ();
		foreach (Connection connection in startNode.Connections)
		{
			if (!connection.Enabled)
			{
				continue;
			}
			openSet[connection] = new SeekerData (connection, GScore (connection), HScore (connection));
		}
		
		List<Connection> closedSet = new List<Connection> ();
		
		while (Application.isPlaying)
		{
			yield return null;
			for (int i = 0; i < m_IterationCap; i++)
			{
				if (openSet.Count == 0)
				// Unable to find path
				{
					OnPathFailed ();
					yield break;
				}
				
				List<SeekerData> openSetValues = new List<SeekerData> (openSet.Values);
				openSetValues.Sort ();
				SeekerData currentPath = openSetValues[0];
				
				if (currentPath.Destination == endNode)
				// Did find the path
				{
					OnPathResult (new Path (m_StartPosition, m_EndPosition, currentPath));
					yield break;
				}
				
				openSet.Remove (currentPath.LastSegment);
				closedSet.Add (currentPath.LastSegment);
				
				foreach (Connection connection in currentPath.Options)
				{
					if (!connection.Enabled || closedSet.Contains (connection) || openSet.ContainsKey (connection))
					{
						continue;
					}
					
					openSet[connection] = new SeekerData (currentPath, connection, GScore (connection), HScore (connection));
				}
			}
		}
	}
	
	
	private void OnPathResult (Path path)
	{
		path.SeekTime = (float)(Time.realtimeSinceStartup - m_StartTime);
		m_Owner.OnPathResult (m_EndPosition, path);
	}
	
	
	private void OnPathFailed ()
	{
		m_Owner.OnPathFailed (m_EndPosition);
	}
	
	
	private float GScore (Connection connection)
	{
		float score = connection.Cost;
		
		foreach (WeightHandler handler in m_Owner.WeightHandlers (connection.Tag))
		{
			score *= handler (connection);
		}
		
		return score;
	}
	
	
	private float HScore (Connection connection)
	{
		return (m_EndPosition - connection.To.Position).sqrMagnitude;
	}
}

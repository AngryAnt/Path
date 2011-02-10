using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate float WeightHandler (object obj);

internal class Seeker
{
	private Navigator m_Owner;
	private Vector3 m_StartPosition, m_EndPosition;
	private int m_IterationCap;
	private double m_StartTime;
	private bool m_Seeking;
	
	
	public Seeker (Vector3 startPosition, Vector3 endPosition, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Owner = owner;
		m_IterationCap = Navigation.SeekerIterationCap;
	}
	
	
	public IEnumerator Seek ()
	{
		m_StartTime = Time.realtimeSinceStartup;
		
		m_Seeking = true;
		
		if (m_Owner.DirectPath (m_StartPosition, m_EndPosition))
		{
			#if DEBUG_SEEKER
				Debug.Log ("Seeker: DirectPath. Early out.");
			#endif
			OnPathResult (new Path (m_StartPosition, m_EndPosition, m_Owner));
			yield break;
		}
		
		Waypoint	startNode = Navigation.GetNearestNode (m_StartPosition, m_Owner),
					endNode = Navigation.GetNearestNode (m_EndPosition, m_Owner);
		
		if (startNode == endNode)
		{
			#if DEBUG_SEEKER
				Debug.Log (string.Format ("Seeker: Start and end node shared: {0}. Early out.", startNode));
			#endif
			OnPathResult (new Path (m_StartPosition, m_EndPosition, startNode, m_Owner));
			yield break;
		}
		
		Dictionary<Connection, SeekerData> openSet = new Dictionary<Connection, SeekerData> ();
		foreach (Connection connection in startNode.Connections)
		{
			if (!connection.Enabled)
			{
				#if DEBUG_SEEKER
					Debug.Log (string.Format ("Seeker: Skipping disabled connection {0}.", connection));
				#endif
				continue;
			}
			openSet[connection] = new SeekerData (connection, GScore (connection), HScore (connection));
			#if DEBUG_SEEKER
				Debug.Log ("Added " + connection + " to open set.");
			#endif
		}
		
		List<Connection> closedSet = new List<Connection> ();
		
		while (Application.isPlaying && m_Seeking)
		{
			yield return null;
			for (int i = 0; i < m_IterationCap; i++)
			{
				if (openSet.Count == 0)
				// Unable to find path
				{
					#if DEBUG_SEEKER
						Debug.Log (string.Format ("Seeker: Empty open set while trying to pathfind from {0} to {1}. Failure.", startNode, endNode));
					#endif
					OnPathFailed ();
					yield break;
				}
				
				List<SeekerData> openSetValues = new List<SeekerData> (openSet.Values);
				openSetValues.Sort ();
				SeekerData currentPath = openSetValues[0];
				
				if (currentPath.Destination == endNode)
				// Did find the path
				{
					Path path = new Path (m_StartPosition, m_EndPosition, currentPath, m_Owner);
					if (path.Valid)
					{
						OnPathResult (path);
						yield break;
					}
					else
					{
						#if DEBUG
							Debug.Log ("Seeker: Path invalidated in middle of search. Re-seeking.");
						#endif
						yield return Seek ();
						yield break;
					}
				}
				
				openSet.Remove (currentPath.LastSegment);
				closedSet.Add (currentPath.LastSegment);
				
				foreach (Connection connection in currentPath.Options)
				{
					if (!connection.Enabled)
					{
						#if DEBUG_SEEKER
							Debug.Log (string.Format ("Seeker: Skipping disabled connection {0} in path {1}.", connection, currentPath));
						#endif
						continue;
					}
					
					if (connection.Width < m_Owner.width || connection.To.Radius * 2.0f < m_Owner.width)
					{
						#if DEBUG_SEEKER
							Debug.Log (string.Format ("Seeker: Skipping too narrow connection {0} in path {1}.", connection, currentPath));
						#endif
						continue;
					}
					
					if (closedSet.Contains (connection))
					{
						#if DEBUG_SEEKER
							Debug.Log (string.Format ("Seeker: Skipping closed set connection {0} in path {1}.", connection, currentPath));
						#endif
						continue;
					}
					
					if (openSet.ContainsKey (connection))
					{
						#if DEBUG_SEEKER
							Debug.Log (string.Format ("Seeker: Skipping open set connection {0} in path {1}.", connection, currentPath));
						#endif
						continue;
					}
					
					openSet[connection] = new SeekerData (currentPath, connection, GScore (connection), HScore (connection));
					
					#if DEBUG_SEEKER
						Debug.Log ("Added " + connection + " to open set.");
					#endif
				}
			}
		}
	}
	
	
	public void Stop ()
	{
		m_Seeking = false;
	}
	
	
	private void OnPathResult (Path path)
	{
		m_Seeking = false;
		path.SeekTime = (float)(Time.realtimeSinceStartup - m_StartTime);
		m_Owner.OnPathResult (this, path);
		Navigation.WatchPath (path);
	}
	
	
	private void OnPathFailed ()
	{
		m_Seeking = false;
		m_Owner.OnPathFailed (this);
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

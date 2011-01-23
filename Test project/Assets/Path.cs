using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Path
{
	private Vector3 m_StartPosition, m_EndPosition;
	private List<Waypoint> m_Nodes = new List<Waypoint> ();
	
	
	public Path (Vector3 startPosition, Vector3 endPosition)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
	}
	
	
	public Path (Vector3 startPosition, Vector3 endPosition, SeekerData data)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		
		m_Nodes.Add (data.Path[0].From);
		foreach (Connection connection in data.Path)
		{
			m_Nodes.Add (connection.To);
		}
	}
	
	
	public ReadOnlyCollection<Waypoint> Nodes
	{
		get
		{
			return m_Nodes.AsReadOnly ();
		}
	}
	
	
	public Vector3 StartPosition
	{
		get
		{
			return m_StartPosition;
		}
	}
	
	
	public Vector3 EndPosition
	{
		get
		{
			return m_EndPosition;
		}
	}
	
	
	public void ArrivedAt (Waypoint waypoint)
	{
		m_Nodes.Remove (waypoint);
	}
}

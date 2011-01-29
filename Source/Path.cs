using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Path
{
	private Vector3 m_StartPosition, m_EndPosition;
	private List<Connection> m_Segments = new List<Connection> ();
	private float m_SeekTime;
	
	
	public Path (Vector3 startPosition, Vector3 endPosition)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
	}
	
	
	public Path (Vector3 startPosition, Vector3 endPosition, SeekerData data)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Segments = new List<Connection> (data.Path);
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
	
	
	public Waypoint StartNode
	{
		get
		{
			return m_Segments[0].From;
		}
	}
	
	
	public Waypoint EndNode
	{
		get
		{
			return m_Segments[m_Segments.Count - 1].To;
		}
	}
	
	
	public ReadOnlyCollection<Connection> Segments
	{
		get
		{
			return m_Segments.AsReadOnly ();
		}
	}
	
	
	public float SeekTime
	{
		get
		{
			return m_SeekTime;
		}
		set
		{
			m_SeekTime = value;
		}
	}
	
	
	public void ArrivedAt (Waypoint waypoint)
	{
		for (int i = 0; i < m_Segments.Count; i++)
		{
			if (m_Segments[i].To == waypoint)
			{
				m_Segments.RemoveRange (0, i + 1);
				return;
			}
		}
	}
	
	
	public void OnDrawGizmos ()
	{
		Gizmos.DrawLine (StartPosition, StartNode.Position);
		foreach (Connection connection in Segments)
		{
			Gizmos.DrawLine (connection.From.Position, connection.To.Position);
		}
		Gizmos.DrawLine (EndNode.Position, EndPosition);
	}
	
	
	public override string ToString ()
	{
		string value = "Path from " + StartNode;
		
		foreach (Connection connection in Segments)
		{
			value += " to " + connection.To;
		}
		
		return value + ".";
	}
}

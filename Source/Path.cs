using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Path
{
	private Vector3 m_StartPosition, m_EndPosition;
	private List<Connection> m_Segments = new List<Connection> ();
	private float m_SeekTime;
	private Navigator m_Owner;
	
	
	internal Path (Vector3 startPosition, Vector3 endPosition, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Owner = owner;
	}
	
	
	internal Path (Vector3 startPosition, Vector3 endPosition, SeekerData data, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Segments = new List<Connection> (data.Path);
		m_Owner = owner;
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
			return m_Segments.Count > 0 ? m_Segments[0].From : null;
		}
	}
	
	
	public Waypoint EndNode
	{
		get
		{
			return m_Segments.Count > 0 ? m_Segments[m_Segments.Count - 1].To : null;
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
	
	
	public Navigator Owner
	{
		get
		{
			return m_Owner;
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
	
	
	public bool Contains (Connection connection)
	{
		return m_Segments.Contains (connection);
	}
	
	
	public bool Contains (Waypoint waypoint)
	{
		if (waypoint == StartNode)
		{
			return true;
		}
		
		foreach (Connection connection in m_Segments)
		{
			if (connection.To == waypoint)
			{
				return true;
			}
		}
		
		return false;
	}
	
	
	internal void OnDrawGizmos ()
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

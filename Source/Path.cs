using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


/// Path pathfinding result.
public class Path
{
	private Vector3 m_StartPosition, m_EndPosition;
	private List<Connection> m_Segments = new List<Connection> ();
	private float m_SeekTime;
	private Navigator m_Owner;
	private Waypoint m_OnlyNode = null;
	
	
	internal Path (Vector3 startPosition, Vector3 endPosition, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Owner = owner;
	}
	
	
	internal Path (Vector3 startPosition, Vector3 endPosition, Waypoint onlyNode, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_OnlyNode = onlyNode;
		m_Owner = owner;
	}
	
	
	internal Path (Vector3 startPosition, Vector3 endPosition, SeekerData data, Navigator owner)
	{
		m_StartPosition = startPosition;
		m_EndPosition = endPosition;
		m_Segments = new List<Connection> (data.Path);
		m_Owner = owner;
		Optimize ();
	}
	
	
	private void Optimize ()
	{
		if (!m_Owner.takeShortcuts || m_Segments.Count < 2)
		{
			#if DEBUG_SEEKER
				Debug.Log ("Path: Not optimizing. Too few segments or no shortcut taking.");
			#endif
			return;
		}
		
		#if DEBUG_SEEKER
			Debug.Log ("Path: Optimizing start.");
		#endif
		for (int i = 1; i < m_Segments.Count - 1;)
		{
			if (m_Segments[i].From.Contains (m_StartPosition) || m_Owner.DirectPath (m_StartPosition, m_Segments[i].From.Position))
			{
				#if DEBUG_SEEKER
					Debug.Log ("Path: Cut to starting at " + m_Segments[i].From + ".");
				#endif
				m_Segments.RemoveRange (0, i);
				i = 1;
			}
			else
			{
				i++;
			}
		}
		
		#if DEBUG_SEEKER
			Debug.Log ("Path: Optimizing end.");
		#endif
		for (int i = m_Segments.Count - 1; i > 0;)
		{
			if (m_Segments[i].To.Contains (m_EndPosition) || m_Owner.DirectPath (m_Segments[i].To.Position, m_EndPosition))
			{
				#if DEBUG_SEEKER
					Debug.Log ("Path: Cut to ending at " + m_Segments[i].To + ".");
				#endif
				m_Segments.RemoveRange (i, m_Segments.Count - i);
				i = m_Segments.Count - 1;
			}
			else
			{
				i--;
			}
		}
	}
	
	
	/// The position where this Path starts.
	public Vector3 StartPosition
	{
		get
		{
			return m_StartPosition;
		}
	}
	
	
	/// The end position where this Path leads to.
	public Vector3 EndPosition
	{
		get
		{
			return m_EndPosition;
		}
	}
	
	
	/// The node where this Path starts.
	public Waypoint StartNode
	{
		get
		{
			return m_OnlyNode != null ? m_OnlyNode : (m_Segments.Count > 0 ? m_Segments[0].From : null);
		}
	}
	
	
	/// The node where this Path ends.
	public Waypoint EndNode
	{
		get
		{
			return m_OnlyNode != null ? m_OnlyNode : (m_Segments.Count > 0 ? m_Segments[m_Segments.Count - 1].To : null);
		}
	}
	
	
	/// The segments making up this Path.
	public ReadOnlyCollection<Connection> Segments
	{
		get
		{
			return m_Segments.AsReadOnly ();
		}
	}
	
	
	/// The time it took to find this Path.
	public float SeekTime
	{
		get
		{
			return m_SeekTime;
		}
		internal set
		{
			m_SeekTime = value;
		}
	}
	
	
	/// The Navigator which requested this Path.
	public Navigator Owner
	{
		get
		{
			return m_Owner;
		}
	}
	
	
	/// Is this Path still valid?
	public bool Valid
	{
		get
		{
			if (StartNode == null && EndNode == null)
			{
				return true;
			}
			else if (StartNode == null || EndNode == null)
			{
				return false;
			}
			
			if (!StartNode.Enabled || !EndNode.Enabled)
			{
				return false;
			}
			
			foreach (Connection connection in m_Segments)
			{
				if (!connection.Enabled || !connection.To.Enabled)
				{
					return false;
				}
			}
			
			return true;
		}
	}
	
	
	/// Signal that the Path user has now arrived at this node. Removes the node from the Path and any nodes leading up to it.
	public void ArrivedAt (Waypoint waypoint)
	{
		if (waypoint == null)
		{
			return;
		}
		
		if (m_OnlyNode != null && waypoint == m_OnlyNode)
		{
			m_OnlyNode = null;
			return;
		}
		
		for (int i = 0; i < m_Segments.Count; i++)
		{
			if (m_Segments[i].To == waypoint)
			{
				m_Segments.RemoveRange (0, i + 1);
				return;
			}
		}
	}
	
	
	/// Does this Path contain the specified Connection?
	public bool Contains (Connection connection)
	{
		return m_Segments.Contains (connection);
	}
	
	
	/// Does this Path contain the specified Waypoint?
	public bool Contains (Waypoint waypoint)
	{
		if (waypoint == null)
		{
			return false;
		}
		
		if (waypoint == m_OnlyNode || waypoint == StartNode || waypoint == EndNode)
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
	
	
	/// Handy for visualizing the Path via the Gizmos system.
	public void OnDrawGizmos ()
	{
		if (StartNode == null)
		{
			Gizmos.DrawLine (StartPosition, EndPosition);
			return;
		}
		
		Gizmos.DrawLine (StartPosition, StartNode.Position);
		foreach (Connection connection in Segments)
		{
			Gizmos.DrawLine (connection.From.Position, connection.To.Position);
		}
		Gizmos.DrawLine (EndNode.Position, EndPosition);
	}
	
	
	/// Overridden for easy debugging.
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

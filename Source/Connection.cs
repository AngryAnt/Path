using UnityEngine;


/// A connection between two Path nodes.
[System.Serializable]
public class Connection
{
	[SerializeField]
	private Waypoint m_From, m_To;
	[SerializeField]
	private float m_Width = 1.0f;
	[SerializeField]
	private string m_Tag = "Untagged";
	[SerializeField]
	private bool m_Enabled = true;
	
	
	/// Forms a new connection between two nodes. The connection lists itself with the from node.
	public Connection (Waypoint from, Waypoint to)
	{
		Resources.Assert (from != null && to != null && from != to);
		
		m_From = from;
		m_To = to;
		
		if (!m_From.ConnectsTo (m_To))
		{
			m_From.AddConnection (this);
		}
	}
	
	
	/// The origin of the connection.
	public Waypoint From
	{
		get
		{
			return m_From;
		}
	}
	
	
	/// The target of the connection.
	public Waypoint To
	{
		get
		{
			return m_To;
		}
	}
	
	
	/// The width of the connection. Connections narrower than the pathfinding Navigator will not be picked.
	public float Width
	{
		get
		{
			return m_Width;
		}
		set
		{
			m_Width = value > 0 ? value : m_Width;
		}
	}
	
	
	/// The Connection Tag. This is used to weigh the Connection when pathfinding, assuming the pathfinding
	/// Navigator has registered any weight handlers with the tag.
	public string Tag
	{
		get
		{
			return m_Tag;
		}
		set
		{
			m_Tag = value;
		}
	}
	
	
	/// The Connection Enabled flag. If set to false, pathfinding will ignore the Connection and any already
	/// found pathes going via this Connection will be invalidated.
	public bool Enabled
	{
		get
		{
			return m_Enabled;
		}
		set
		{
			m_Enabled = value;
			if (!m_Enabled)
			{
				Navigation.OnDisable (this);
			}
		}
	}
	
	
	/// The Connection cost. This is the pathfinding cost of the connection, before any tag-based weighting.
	public float Cost
	{
		get
		{
			return (To.Position - From.Position).sqrMagnitude;
		}
	}
	
	
	/// Overridden for easy debugging.
	public override string ToString ()
	{
		return "Connection to " + To.ToString ();
	}
}

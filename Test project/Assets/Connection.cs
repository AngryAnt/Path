using UnityEngine;

[System.Serializable]
public class Connection
{
	[SerializeField]
	private Waypoint m_From, m_To;
	[SerializeField]
	private float m_Width = 1.0f;
	[SerializeField]
	private string m_Tag = "Untagged";
	
	
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
	
	
	public Waypoint From
	{
		get
		{
			return m_From;
		}
	}
	
	
	public Waypoint To
	{
		get
		{
			return m_To;
		}
	}
	
	
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
	
	
	public float Cost
	{
		get
		{
			return (To.Position - From.Position).sqrMagnitude;
		}
	}
	
	
	public override string ToString ()
	{
		return "Connection to " + To.ToString ();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{
	[SerializeField]
	public List<Connection> m_Connections = new List<Connection> ();
	[SerializeField]
	public float m_Radius = 1;
	
	
	void Start ()
	{
		Navigation.RegisterWaypoint (this);
	}
	
	
	void OnDisable ()
	{
		Navigation.OnDisable (this);
	}
	
	
	void OnDestroy ()
	{
		Navigation.UnregisterWaypoint (this);
	}
	
	
	public Connection AddConnection (Connection connection)
	{
		Resources.Assert (connection.From == this);
		
		if (!m_Connections.Contains (connection))
		{
			m_Connections.Add (connection);
		}
		
		return connection;
	}
	
	
	public void RemoveConnection (Connection connection)
	{
		m_Connections.Remove (connection);
		Navigation.OnDisable (connection);
	}
	
	
	public void RemoveConnection (Waypoint waypoint)
	{
		for (int i = 0; i < m_Connections.Count;)
		{
			if (m_Connections[i].To == waypoint)
			{
				m_Connections.RemoveAt (i);
			}
			else
			{
				i++;
			}
		}
	}
	
	
	public bool ConnectsTo (Waypoint waypoint)
	{
		foreach (Connection connection in m_Connections)
		{
			if (connection.To == waypoint)
			{
				return true;
			}
		}
		
		return false;
	}
	
	
	public void Disconnect ()
	{
		m_Connections = new List<Connection> ();
	}
	
	
	public ReadOnlyCollection<Connection> Connections
	{
		get
		{
			return m_Connections.AsReadOnly ();
		}
	}
	
	
	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}
	
	
	public Vector3 Position
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = value;
		}
	}
	
	
	public float Radius
	{
		get
		{
			return m_Radius;
		}
		set
		{
			m_Radius = value > 0 ? value : m_Radius;
		}
	}
	
	
	public string Tag
	{
		get
		{
			return gameObject.tag;
		}
		set
		{
			gameObject.tag = value;
		}
	}
	
	
	public override string ToString ()
	{
		return gameObject.name;
	}
}

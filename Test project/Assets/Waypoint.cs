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
	
	
	void OnEnable ()
	{
		Navigation.RegisterWaypoint (this);
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
	
	
	public void RenderGizmos ()
	{
		Gizmos.color = PathInspector.SelectedWaypoint == this ? Color.white : Color.yellow;
		
		Gizmos.DrawWireSphere (Position, Radius);
		foreach (Connection connection in Connections)
		{
			Gizmos.color = PathInspector.SelectedConnection == connection ? Color.white : Color.green;
			
			if (PathInspector.ShowConnectionWidth || PathInspector.SelectedConnection == connection)
			{
				Vector3 vector, vectorCrossNormal, fromOffsetA, fromOffsetB, toOffsetA, toOffsetB;

				vector = connection.To.Position - Position;
				vectorCrossNormal = Vector3.Cross (vector, Vector3.up).normalized;

				fromOffsetA = 
					Position +
					vector.normalized * Radius +
					vectorCrossNormal * -(connection.Width / 2.0f);

				fromOffsetB =
					Position +
					vector.normalized * Radius +
					vectorCrossNormal * (connection.Width / 2.0f);

				toOffsetA =
					connection.To.Position - 
					vector.normalized * connection.To.Radius +
					vectorCrossNormal * -(connection.Width / 2.0f);

				toOffsetB =
					connection.To.Position -
					vector.normalized * connection.To.Radius +
					vectorCrossNormal * (connection.Width / 2.0f);

				Gizmos.DrawLine (fromOffsetA, toOffsetA);
				Gizmos.DrawLine (fromOffsetB, toOffsetB);
				Gizmos.DrawLine (fromOffsetA, fromOffsetB);
				Gizmos.DrawLine (toOffsetA, connection.To.Position);
				Gizmos.DrawLine (toOffsetB, connection.To.Position);
			}
			else
			{
				Gizmos.DrawLine (connection.From.Position, connection.To.Position);
			}
		}
	}
}

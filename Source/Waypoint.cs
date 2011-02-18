using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace PathRuntime
{
	/// A Path waypoint. Each waypoint is defined by a Position, a Radius and optinally a list of Connections.
	[ExecuteInEditMode]
	[AddComponentMenu ("Path/Waypoint")]
	public class Waypoint : MonoBehaviour
	{
		[SerializeField]
		private List<Connection> m_Connections = new List<Connection> ();
		[SerializeField]
		private float m_Radius = 1;


		void Start ()
		{
			Navigation.RegisterWaypoint (this);
		}
		
		
		void OnEnable ()
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


		internal Connection AddConnection (Connection connection)
		{
			Resources.Assert (connection.From == this);

			if (!m_Connections.Contains (connection))
			{
				m_Connections.Add (connection);
			}

			return connection;
		}


		/// Removes a given connection.
		public void RemoveConnection (Connection connection)
		{
			m_Connections.Remove (connection);
			Navigation.OnDisable (connection);
		}


		/// Removes any connection to the specified node.
		public void RemoveConnection (Waypoint waypoint)
		{
			for (int i = 0; i < m_Connections.Count;)
			{
				if (m_Connections[i].To == waypoint)
				{
					RemoveConnection (m_Connections[i]);
				}
				else
				{
					i++;
				}
			}
		}


		/// Returns true of this Waypoint connects to the given node.
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


		/// Removes all outgoing connections from the node.
		public void Disconnect ()
		{
			m_Connections = new List<Connection> ();
		}


		/// The list of connections going out from this Waypoint.
		public ReadOnlyCollection<Connection> Connections
		{
			get
			{
				return m_Connections.AsReadOnly ();
			}
		}


		/// The Waypoint Enabled flag. If set to false, pathfinding will ignore the Waypoint and any already
		/// found pathes going through this Waypoint will be invalidated.
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


		/// The Waypoint position.
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


		/// The Waypoint radius. Waypoints narrower than the pathfinding Navigator will not be picked.
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


		/// Is the given position inside the area covered by the Waypoint?
		public bool Contains (Vector3 position)
		{
			return (Position - position).magnitude < Radius;
		}


		/// The Waypoint Tag. This is used to weigh the Waypoint when pathfinding, assuming the pathfinding
		/// Navigator has registered any weight handlers with the tag.
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


		/// Overridden for easy debugging.
		public override string ToString ()
		{
			return gameObject.name;
		}
	}
}

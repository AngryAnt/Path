using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;


/// @mainpage
/// Welcome to the API documentation for %Path 2.0. You will find an API overview on the Classes page, linked
/// from the tab bar above here. For demo videos and non-API documentation, please check out
/// http://angryant.com/path/documentation/.


[assembly: System.Runtime.CompilerServices.InternalsVisibleTo ("Path.Editor, PublicKey=" +
"0024000004800000940000000602000000240000525341310004000011000000155975c8857abb" + 
"caf0be25ba0bb94c9298e2ad84cd98efb15e34b8763884597663c14e12b9276aa8e97b330421e8" + 
"31660b20014180df8d00316fc6a6633f0a848545c18f0b2f040141d95baa8af56b40286706b211" + 
"ff04d0c0ce4de0f726b8f082bbd86f020a7507a5a1c7a4be8400416b2f04abc9c537f72369f137" + 
"f0f15987")]


internal delegate void Handler ();


namespace PathRuntime
{
	/// Path system root. This class is the core of the Path system. All nodes and connections register here and the
	/// Navigator pulls data from here - as can your own code.
	[ExecuteInEditMode]
	[AddComponentMenu ("Path/Navigation")]
	public class Navigation : MonoBehaviour
	{
		private static Navigation s_Instance;

		private List<Waypoint> m_Waypoints = new List<Waypoint> ();
		[SerializeField]
		private int m_SeekerIterationCap = 10;
		private Handler m_DrawGizmosHandler = null;
		private List<WeakReference> m_CalculatedPathes = new List<WeakReference> ();


		internal static Navigation Instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = FindObjectOfType (typeof (Navigation)) as Navigation;
					if (s_Instance == null)
					{
						s_Instance = new GameObject ("Navigation").AddComponent<Navigation> ();
					}
				}

				return s_Instance;
			}
		}


		void Awake ()
		{
			if (s_Instance != null && s_Instance != this)
			{
				Debug.LogError ("Second singleton instance. Bailing.");
				Destroy (gameObject);

				return;
			}

			for (int i = 0; i < m_Waypoints.Count;)
			{
				if (m_Waypoints[i] == null)
				{
					m_Waypoints.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}

			s_Instance = this;
		}


		void OnDestroy ()
		{
			if (s_Instance == this)
			{
				s_Instance = null;
			}
		}


		internal static Handler DrawGizmosHandler
		{
			get
			{
				return Instance.m_DrawGizmosHandler;
			}
			set
			{
				Instance.m_DrawGizmosHandler = value;
			}
		}


		/// Pathfinding resource throttle. The SeekerIterationCap specifies how many pathfinding iterations can be
		/// run per frame. Reduce this number if you are getting Path related performance issues or increase if
		/// pathfinding starts taking a while. Default value is 10.
		public static int SeekerIterationCap
		{
			get
			{
				return Instance.m_SeekerIterationCap;
			}
			set
			{
				Instance.m_SeekerIterationCap = value;
			}
		}


		/// All registered waypoints. When a waypoint - in the standard scene setup or instantiated runtime - is
		/// set up, it registers with Navigation. Similarly, when it is destroyed, it unregisters. This API point
		/// gives you access to all the in-scene Waypoints.
		public static ReadOnlyCollection<Waypoint> Waypoints
		{
			get
			{
				return Instance.m_Waypoints.AsReadOnly ();
			}
		}


		/// Returns nearest node to a position for a given Navigator. If the Navigator has non-zero pathBlockingLayers
		/// specified, each potential node will be checked for accessibility with a sphere cast.
		public static Waypoint GetNearestNode (Vector3 position, Navigator navigator)
		{
			Waypoint nearest = null;

			foreach (Waypoint waypoint in Navigation.Waypoints)
			{
				if (
					waypoint.Enabled &&
					(
						nearest == null ||
						(nearest.Position - position).sqrMagnitude > (waypoint.Position - position).sqrMagnitude
					) &&
					(
						(waypoint.Position - position).magnitude < waypoint.Radius ||
						navigator == null ||
						navigator.DirectPath (position, waypoint.Position)
					)
				)
				{
					nearest = waypoint;
				}
			}

			return nearest;
		}


		/// Returns nearest node to a position.
		public static Waypoint GetNearestNode (Vector3 position)
		{
			return GetNearestNode (position, null);
		}


		/// Maximize scale of all waypoints. Also available in the inspector, this method lets you scale up all
		/// waypoints so that they have the biggest possible radius, while not overlapping any colliders in the given
		/// layerMask.
		public static void AutoScale (LayerMask layerMask, float minWidth, float maxWidth, float step)
		{
			foreach (Waypoint waypoint in Instance.m_Waypoints)
			{
				float radius = maxWidth;

				while (radius > minWidth)
				{
					if (!Physics.CheckSphere (waypoint.Position, radius, layerMask))
					{
						waypoint.Radius = radius;
						break;
					}

					radius -= step;
				}
			}
		}


		/// Form any available, non-existing, waypoint connections. Also available in the inspector, this method lets
		/// you connect any waypoints that can, which are not already connected. Like AutoScale, the established
		/// connections will be formed based on the given layerMask and maximized in size.
		public static void AutoConnect (LayerMask layerMask, float minWidth, float maxWidth, float step)
		{
			foreach (Waypoint one in Instance.m_Waypoints)
			{
				foreach (Waypoint other in Instance.m_Waypoints)
				{
					if (one == other || one.ConnectsTo (other))
					{
						continue;
					}

					float radius = maxWidth;

					while (radius > minWidth)
					{
						RaycastHit hit;
						if (!Physics.CheckSphere (one.Position, radius, layerMask) && 
							!Physics.SphereCast (
								one.Position,
								radius,
								other.Position - one.Position,
								out hit,
								(other.Position - one.Position).magnitude,
								layerMask
							)
						)
						{
							new Connection (one, other).Width = radius * 2.0f;
							break;
						}

						radius -= step;
					}
				}
			}
		}


		internal static Waypoint RegisterWaypoint (Waypoint waypoint)
		{
			if (!Instance.m_Waypoints.Contains (waypoint))
			{
				Instance.m_Waypoints.Add (waypoint);
			}

			return waypoint;
		}


		internal static Waypoint UnregisterWaypoint (Waypoint waypoint)
		{
			if (s_Instance == null)
			{
				return waypoint;
			}

			Instance.m_Waypoints.Remove (waypoint);

			foreach (Waypoint other in Instance.m_Waypoints)
			{
				other.RemoveConnection (waypoint);
			}

			return waypoint;
		}


		internal static void WatchPath (Path path)
		{
			WeakReference reference = new WeakReference (path);
			if (!Instance.m_CalculatedPathes.Contains (reference))
			{
				Instance.m_CalculatedPathes.Add (reference);
			}
		}


		private void HandleDisable (object subject)
		{
			Waypoint waypoint = subject as Waypoint;
			Connection connection = subject as Connection;

			for (int i = 0; i < m_CalculatedPathes.Count;)
			{
				if (!m_CalculatedPathes[i].IsAlive)
				{
					m_CalculatedPathes.RemoveAt (i);
					continue;
				}

				Path path = (Path)m_CalculatedPathes[i].Target;

				if (path.Owner == null)
				{
					m_CalculatedPathes.RemoveAt (i);
					continue;
				}

				if (waypoint != null && path.Contains (waypoint))
				{
					path.Owner.gameObject.SendMessage ("OnPathInvalidated", path, SendMessageOptions.DontRequireReceiver);
				}
				else if (connection != null && path.Contains (connection))
				{
					path.Owner.gameObject.SendMessage ("OnPathInvalidated", path, SendMessageOptions.DontRequireReceiver);
				}

				i++;
			}
		}


		internal static void OnDisable (Waypoint waypoint)
		{
			if (s_Instance == null)
			{
				return;
			}
			s_Instance.HandleDisable (waypoint);
		}


		internal static void OnDisable (Connection connection)
		{
			if (s_Instance == null)
			{
				return;
			}
			s_Instance.HandleDisable (connection);
		}


		internal void OnDrawGizmos ()
		{
			if (m_DrawGizmosHandler != null)
			{
				m_DrawGizmosHandler ();
			}
		}
	}
}

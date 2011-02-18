using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace PathRuntime
{
	/// Runtime pathfinding interface. This is your runtime interface for Path. The navigator can be given a position to
	/// pathfind for, via the targetPosition variable, or you can call RequestPath in order to pre-calculate pathes.
	///
	/// Sent messages:
	/// <table border="0" cellpadding="10">
	/// <tr><td style="white-space:nowrap;">OnNewPath (Path path)</td><td>Sent when a new path is available, after a new targetPosition has been
	/// specified.</td></tr>
	/// <tr><td style="white-space:nowrap;">OnTargetUnreachable ()</td><td>Sent when a given targetPosition is unreachable.</td></tr>
	/// <tr><td style="white-space:nowrap;">OnPathAvailable (Path path)</td><td>Sent when pathfinding a new path is available, after being requested
	/// via RequestPath.</td></tr>
	/// <tr><td style="white-space:nowrap;">OnPathUnavailable ()</td><td>Sent when a RequestPath call fails to produce a valid path.</td></tr>
	/// <tr><td style="white-space:nowrap;">OnPathInvalidated (Path path)</td><td>Send when the given path has been invalidated - by a connection or
	/// node being disabled or deleted. A given Navigator receives this message because the Path was originally requested
	/// by the Navigator - either via targetPosition or RequestPath.</td></tr>
	/// </table>
	[AddComponentMenu ("Path/Navigator")]
	public class Navigator : MonoBehaviour
	{
		private Vector3 m_PathfoundTargetPosition;
		private Dictionary<string, List<WeightHandler>> m_WeightHandlers = new Dictionary<string, List<WeightHandler>> ();
		private Seeker m_ActiveMainSeeker = null;


		/// Directs Navigator pathfinding. When set to a new value, a pathfinding request will run. Resetting targetPosition
		/// to the same value will not re-run pathfinding.
		public Vector3 targetPosition = Vector3.zero;
		/// Used for validating connections, nodes and if applicable, closest nodes. Pathes going through connections or
		/// nodes narrower than this will not be picked.
		public float width = 1.0f;
		/// Should target position be set to start position? Enabled by default, this setting instructs the Navigator to,
		/// on Start, set targetPosition to its current position. If disabled and targetPosition is different from the
		/// Navigators starting position, the Navigator will pathfind immediately after Start.
		public bool selfTargetOnAwake = true;
		/// Used for evaluating closest waypoint. If non-zero (anything but "none" in the inspector), sphere casts against
		/// this mask will be performed in order to validate possible start and end nodes for a pathfinding request. If
		/// set to zero (or "none" in the inspector), no evaluation other than enabled / disabled will be performed when
		/// doing closest node requests.
		public LayerMask pathBlockingLayers = 0;
		/// Toggles path optimization. If enabled, the Navigator will, when pathfinding, look for shortcuts. This involves
		/// doing some physics sphere casts, so it will add to the total seek time. If you experience performance issues,
		/// try disabling this option.
		public bool takeShortcuts = true;


		void Awake ()
		{
			m_PathfoundTargetPosition = transform.position;
			if (selfTargetOnAwake)
			{
				targetPosition = transform.position;
			}
		}


		void Update ()
		{
			if (targetPosition != m_PathfoundTargetPosition)
			{
				ReSeek ();
			}
		}


		/// Can this Navigator travel the given segment unobstructed?
		public bool DirectPath (Vector3 fromPosition, Vector3 toPosition)
		{
			if (pathBlockingLayers == 0)
			{
				return true;
			}

			Vector3 target = toPosition - fromPosition;
			RaycastHit hit;

			if (
				!Physics.SphereCast (
					fromPosition,
					width / 2.0f,
					target,
					out hit,
					target.magnitude,
					pathBlockingLayers
				)
			)
			{
				return true;
			}

			return false;
		}


		/// Can this Navigator travel to the given position unobstructed?
		public bool DirectPath (Vector3 toPosition)
		{
			return DirectPath (transform.position, toPosition);
		}


		/// Force pathfinding to targetPosition. This is useful if the current path to targetPosition has for some reason
		/// been invalidated. Calling ReSeek will recalculate it, even though targetPosition has not changed.
		public void ReSeek ()
		{
			if (m_ActiveMainSeeker != null)
			{
				m_ActiveMainSeeker.Stop ();
			}
			m_ActiveMainSeeker = new Seeker (transform.position, targetPosition, this);
			StartCoroutine (m_ActiveMainSeeker.Seek ());
			m_PathfoundTargetPosition = targetPosition;
		}


		/// Pre-calculation interface. This method enables you to have a path calculated for you, independent of your
		/// currently set targetPosition. It is useful for, for instance, pre-calculating pathes to nearest cover or exit.
		public void RequestPath (Vector3 startPosition, Vector3 endPosition)
		{
			StartCoroutine (new Seeker (startPosition, endPosition, this).Seek ());
		}


		/// Interface for per-tag custom weights. This method populates a per-Navigator list of custom weight handlers.
		/// A custom weight handler is associated with a tag and will be invoked whenever Path is considering the
		/// weight of an object with the corresponding tag. A neutral return value of a WeightHandler is 1.0f.
		public void RegisterWeightHandler (string tag, WeightHandler handler)
		{
			if (m_WeightHandlers.ContainsKey (tag))
			{
				if (!m_WeightHandlers[tag].Contains (handler))
				{
					m_WeightHandlers[tag].Add (handler);
				}
			}
			else
			{
				m_WeightHandlers[tag] = new List<WeightHandler> ();
				m_WeightHandlers[tag].Add (handler);
			}
		}


		internal ReadOnlyCollection<WeightHandler> WeightHandlers (string tag)
		{
			if (m_WeightHandlers.ContainsKey (tag))
			{
				return m_WeightHandlers[tag].AsReadOnly ();
			}

			return new List<WeightHandler> ().AsReadOnly ();
		}


		internal void OnPathFailed (Seeker seeker)
		{
			if (seeker == m_ActiveMainSeeker)
			{
				SendMessage ("OnTargetUnreachable", SendMessageOptions.DontRequireReceiver);
				m_ActiveMainSeeker = null;
				return;
			}

			SendMessage ("OnPathUnavailable", SendMessageOptions.DontRequireReceiver);
		}


		internal void OnPathResult (Seeker seeker, Path result)
		{
			if (seeker == m_ActiveMainSeeker)
			{
				SendMessage ("OnNewPath", result, SendMessageOptions.DontRequireReceiver);
				m_ActiveMainSeeker = null;
				return;
			}

			SendMessage ("OnPathAvailable", result, SendMessageOptions.DontRequireReceiver);
		}
	}
}

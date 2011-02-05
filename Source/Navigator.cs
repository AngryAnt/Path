using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


/// Runtime pathfinding interface. This is your runtime interface for Path. The navigator can be given a position to
/// pathfind for, via the targetPosition variable, or you can call RequestPath in order to pre-calculate pathes.
[AddComponentMenu ("Path/Navigator")]
public class Navigator : MonoBehaviour
{
	private Vector3 m_PathfoundTargetPosition;
	private Dictionary<string, List<WeightHandler>> m_WeightHandlers = new Dictionary<string, List<WeightHandler>> ();
	
	
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
			StartCoroutine (new Seeker (transform.position, targetPosition, this).Seek ());
			m_PathfoundTargetPosition = targetPosition;
		}
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
	
	
	internal void OnPathFailed (Vector3 endPosition)
	{
		if (endPosition == targetPosition)
		{
			SendMessage ("OnTargetUnreachable", SendMessageOptions.DontRequireReceiver);
			return;
		}
		
		SendMessage ("OnPathUnavailable", SendMessageOptions.DontRequireReceiver);
	}
	
	
	internal void OnPathResult (Vector3 endPosition, Path result)
	{
		if (endPosition == targetPosition)
		{
			SendMessage ("OnNewPath", result, SendMessageOptions.DontRequireReceiver);
			return;
		}
		
		SendMessage ("OnPathAvailable", result, SendMessageOptions.DontRequireReceiver);
	}
}

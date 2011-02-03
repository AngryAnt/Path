using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


[AddComponentMenu ("Path/Navigator")]
public class Navigator : MonoBehaviour
{
	private Vector3 m_PathfoundTargetPosition;
	private Dictionary<string, List<WeightHandler>> m_WeightHandlers = new Dictionary<string, List<WeightHandler>> ();
	
	public Vector3 targetPosition = Vector3.zero;
	public float width = 1.0f;
	public bool selfTargetOnAwake = true;
	
	
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
	
	
	public void RequestPath (Vector3 startPosition, Vector3 endPosition)
	{
		StartCoroutine (new Seeker (startPosition, endPosition, this).Seek ());
	}
	
	
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
	
	
	public ReadOnlyCollection<WeightHandler> WeightHandlers (string tag)
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

using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour
{
	private Vector3 m_PathfoundTargetPosition;
	
	public Vector3 targetPosition;
	
	
	void Awake ()
	{
		m_PathfoundTargetPosition = transform.position;
	}
	
	
	void Update ()
	{
		if (targetPosition != m_PathfoundTargetPosition)
		{
			StartCoroutine (new Seeker (transform.position, targetPosition, this).Seek ());
		}
	}
	
	
	public void RequestPath (Vector3 startPosition, Vector3 endPosition)
	{
		StartCoroutine (new Seeker (startPosition, endPosition, this).Seek ());
	}
	
	
	public void OnPathFailed (Vector3 endPosition)
	{
		if (endPosition == targetPosition)
		{
			m_PathfoundTargetPosition = targetPosition;
			SendMessage ("OnTargetUnreachable", SendMessageOptions.DontRequireReceiver);
			return;
		}
		
		SendMessage ("OnPathUnavailable", SendMessageOptions.DontRequireReceiver);
	}
	
	
	public void OnPathResult (Vector3 endPosition, Path result)
	{
		if (endPosition == targetPosition)
		{
			m_PathfoundTargetPosition = targetPosition;
			SendMessage ("OnNewPath", result, SendMessageOptions.DontRequireReceiver);
			return;
		}
		
		SendMessage ("OnPathAvailable", result, SendMessageOptions.DontRequireReceiver);
	}
}

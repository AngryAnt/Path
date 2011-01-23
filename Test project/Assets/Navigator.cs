using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour
{
	private Vector3 m_PathfoundTargetPosition;
	
	public Vector3 m_TargetPosition;
	
	
	void Awake ()
	{
		m_PathfoundTargetPosition = transform.position;
	}
	
	
	void Update ()
	{
		if (m_TargetPosition != m_PathfoundTargetPosition)
		{
			StartCoroutine (new Seeker (transform.position, m_TargetPosition, this).Seek ());
		}
	}
	
	
	public void RequestPath (Vector3 startPosition, Vector3 endPosition)
	{
		StartCoroutine (new Seeker (startPosition, endPosition, this).Seek ());
	}
	
	
	public void OnPathFailed (Vector3 endPosition)
	{
		if (endPosition == m_TargetPosition)
		{
			m_PathfoundTargetPosition = m_TargetPosition;
			SendMessage ("OnTargetUnreachable", SendMessageOptions.DontRequireReceiver);
			return;
		}
		
		SendMessage ("OnPathUnavailable", SendMessageOptions.DontRequireReceiver);
	}
	
	
	public void OnPathResult (Vector3 endPosition, Path result)
	{
		if (endPosition == m_TargetPosition)
		{
			m_PathfoundTargetPosition = m_TargetPosition;
			SendMessage ("OnNewPath", result, SendMessageOptions.DontRequireReceiver);
			return;
		}
		
		SendMessage ("OnPathAvailable", result, SendMessageOptions.DontRequireReceiver);
	}
}

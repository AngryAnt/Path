using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public delegate void Handler ();


[ExecuteInEditMode]
[AddComponentMenu ("Path/Navigation")]
public class Navigation : MonoBehaviour
{
	private static Navigation s_Instance;
	
	private List<Waypoint> m_Waypoints = new List<Waypoint> ();
	[SerializeField]
	private int m_SeekerIterationCap = 10;
	private Handler m_DrawGizmosHandler = null;
	
	
	public static Navigation Instance
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
	
	
	public static Handler DrawGizmosHandler
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
	
	
	public static ReadOnlyCollection<Waypoint> Waypoints
	{
		get
		{
			return Instance.m_Waypoints.AsReadOnly ();
		}
	}

	
	public static Waypoint RegisterWaypoint (Waypoint waypoint)
	{
		if (!Instance.m_Waypoints.Contains (waypoint))
		{
			Instance.m_Waypoints.Add (waypoint);
		}
		
		return waypoint;
	}
	
	
	public static Waypoint UnregisterWaypoint (Waypoint waypoint)
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
	
	
	public static Waypoint GetNearestNode (Vector3 position)
	{
		Waypoint nearest = null;
		
		foreach (Waypoint waypoint in Navigation.Waypoints)
		{
			if (
				waypoint.Enabled &&
				(
					nearest == null ||
					(nearest.Position - position).sqrMagnitude > (waypoint.Position - position).sqrMagnitude
				)
			)
			{
				nearest = waypoint;
			}
		}
		
		return nearest;
	}
	
	
	public void OnDrawGizmos ()
	{
		if (m_DrawGizmosHandler != null)
		{
			m_DrawGizmosHandler ();
		}
	}
}

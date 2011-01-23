using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[CustomEditor (typeof (Navigation))]
public class NavigationInspector : PathInspector
{}


[CustomEditor (typeof (Waypoint))]
public class WaypointInspector : PathInspector
{}


public class PathInspector : Editor
{
	private static List<Waypoint> s_Waypoints = new List<Waypoint> ();
	private static string[] s_OtherWaypointNames = new string[0], s_WaypointSelectionNames = new string[0];
	private static int s_WaypointDropDownIndex = 0;
	private static float s_AutoConnectMaxWidth = 10;
	// Waypoint specific
	private static string[] s_ConnectionNames = new string[0];
	private static int s_ConnectionDropDownIndex = 0, s_ConnectionFormingWaypointIndex = 0;
	
	const float kWaypointListHeight = 200.0f;
	
	
	public void OnEnable ()
	{
		UpdateLists (target);
	}
	
	
	static void UpdateLists (Object target)
	{
		s_Waypoints = new List<Waypoint> (Navigation.Waypoints);
		string[] waypointNames = s_Waypoints.Select (waypoint => waypoint.ToString ()).ToArray ();
		s_WaypointSelectionNames = new string[waypointNames.Length + 2];
		s_WaypointSelectionNames[0] = "Select waypoint";
		s_WaypointSelectionNames[1] = "";
		System.Array.Copy (waypointNames, 0, s_WaypointSelectionNames, 2, waypointNames.Length);
		
		Debug.Log (target == null ? "null" : (target.ToString () + " - " + target.GetType ().ToString ()));
		
		Waypoint targetWaypoint = target as Waypoint;
		if (targetWaypoint != null)
		{
			s_ConnectionNames = targetWaypoint.Connections.Select (connection => connection.ToString ()).ToArray ();
			s_ConnectionFormingWaypointIndex = 0;
			
			if (waypointNames.Length == 1)
			{
				s_OtherWaypointNames = new string[0];
			}
			else
			{
				s_OtherWaypointNames = new string[waypointNames.Length - 1];
				System.Array.Copy (waypointNames, 0, s_OtherWaypointNames, 0, waypointNames.Length - 1);
				int currentIndex = System.Array.IndexOf (waypointNames, targetWaypoint);
				Debug.Log ("Current index: " + currentIndex);
				if (currentIndex != waypointNames.Length - 1)
				{
					s_OtherWaypointNames[currentIndex] = waypointNames[waypointNames.Length - 1];
				}
			}
		}
	}
	
	
	public override void OnInspectorGUI ()
	{
		OnNavigationGUI ();
		
		Waypoint waypoint = target as Waypoint;
		if (waypoint != null)
		{
			EditorGUILayout.Space ();
			GUILayout.BeginHorizontal ();
				EditorGUILayout.Space ();
				GUILayout.BeginVertical ();
					OnWaypointGUI (waypoint);
				GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}
	}
	
	
	public static void OnNavigationGUI ()
	{
		GUILayout.Label ("Navigation", EditorStyles.boldLabel);
		
		Navigation.SeekerIterationCap = EditorGUILayout.IntField ("Seeker iterations", Navigation.SeekerIterationCap);
		
		EditorGUILayout.Space ();
		
		GUILayout.Label ("Autoconnect", EditorStyles.boldLabel);
		
		s_AutoConnectMaxWidth = EditorGUILayout.FloatField ("Max test width", s_AutoConnectMaxWidth);
		
		GUILayout.BeginHorizontal ();
			GUILayout.Space (103);
			if (GUILayout.Button ("Autoconnect", EditorStyles.miniButton))
			{
				Autoconnect ();
			}
		GUILayout.EndHorizontal ();
		
		EditorGUILayout.Space ();
		
		GUILayout.Label ("Waypoints", EditorStyles.boldLabel);
		
		int newWaypointIndex = EditorGUILayout.Popup (s_WaypointDropDownIndex, s_WaypointSelectionNames);
		if (s_WaypointDropDownIndex != newWaypointIndex && newWaypointIndex > 1)
		{
			s_WaypointDropDownIndex = newWaypointIndex;
			SelectWaypoint (s_Waypoints[s_WaypointDropDownIndex - 2]);
		}
		
		GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+", EditorStyles.miniButtonLeft))
			{
				Navigation.RegisterWaypoint (CreateWaypoint ());
				EditorUtility.SetDirty (Navigation.Instance);
			}
			GUI.enabled = s_Waypoints.Count != 0;
			if (GUILayout.Button ("-", EditorStyles.miniButtonRight))
			{
				Navigation.UnregisterWaypoint (s_Waypoints[s_WaypointDropDownIndex - 2]);
				DestroyImmediate (s_Waypoints[s_WaypointDropDownIndex - 2].gameObject);
				s_WaypointDropDownIndex = 0;
				EditorUtility.SetDirty (Navigation.Instance);
			}
			GUI.enabled = true;
		GUILayout.EndHorizontal ();
	}
	
	
	public static void OnWaypointGUI (Waypoint waypoint)
	{
		GUILayout.Label ("Waypoint", EditorStyles.boldLabel);

		waypoint.Radius = EditorGUILayout.FloatField ("Radius", waypoint.Radius);
		waypoint.Tag = EditorGUILayout.TagField ("Tag", waypoint.Tag);

		EditorGUILayout.Space ();

		if (s_ConnectionNames.Length != 0)
		{
			GUILayout.Label ("Connections", EditorStyles.boldLabel);
			s_ConnectionDropDownIndex = EditorGUILayout.Popup (s_ConnectionDropDownIndex, s_ConnectionNames);
			waypoint.Connections[s_ConnectionDropDownIndex].Width = EditorGUILayout.FloatField ("Width", waypoint.Connections[s_ConnectionDropDownIndex].Width);
			waypoint.Connections[s_ConnectionDropDownIndex].Tag = EditorGUILayout.TagField ("Tag", waypoint.Connections[s_ConnectionDropDownIndex].Tag);
		}

		EditorGUILayout.Space ();

		if (s_OtherWaypointNames.Length != 0)
		{
			GUILayout.Label ("New connection", EditorStyles.boldLabel);
			s_ConnectionFormingWaypointIndex = EditorGUILayout.Popup (s_ConnectionFormingWaypointIndex, s_OtherWaypointNames);
	
			if (GUILayout.Button ("Connect", EditorStyles.miniButton))
			{
				new Connection (waypoint, s_Waypoints[s_ConnectionFormingWaypointIndex]);
				EditorUtility.SetDirty (Navigation.Instance);
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty (waypoint);
		}
	}
	
	
	public static Waypoint CreateWaypoint ()
	{
		string name = "Waypoint ";
		int index = 0;
		
		while (GameObject.Find (name + ++index) != null);
		
		GameObject go = new GameObject (name + index);
		go.hideFlags = HideFlags.HideInHierarchy;
		
		return go.AddComponent<Waypoint> ();
	}
	
	
	public static void SelectWaypoint (Waypoint waypoint)
	{
		Selection.activeObject = waypoint.gameObject;
	}
	
	
	public static void Autoconnect ()
	{
		
	}
}

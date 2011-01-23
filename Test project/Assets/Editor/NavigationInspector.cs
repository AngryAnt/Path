using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof (Navigation))]
public class NavigationInspector : Editor
{
	private Vector2 m_Scroll = Vector2.zero;
	private Waypoint m_SelectedWaypoint;
	
	const float kWaypointListHeight = 200.0f;
	
	
	public override void OnInspectorGUI ()
	{
		Navigation.SeekerIterationCap = EditorGUILayout.IntField ("Seeker iterations", Navigation.SeekerIterationCap);
		
		GUILayout.Label ("Waypoints", EditorStyles.toolbar, GUILayout.ExpandWidth (true));
		
		m_Scroll = GUILayout.BeginScrollView (m_Scroll, GUILayout.Height (kWaypointListHeight));
			foreach (Waypoint waypoint in Navigation.Waypoints)
			{
				if (GUILayout.Button (waypoint.ToString (), m_SelectedWaypoint == waypoint ? EditorStyles.whiteLabel : EditorStyles.label))
				{
					m_SelectedWaypoint = waypoint;
					if (Event.current.clickCount == 2)
					{
						SelectWaypoint (waypoint);
					}
				}
			}
		GUILayout.EndScrollView ();
		
		GUILayout.BeginHorizontal (EditorStyles.toolbar);
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("+", EditorStyles.toolbarButton))
			{
				Navigation.RegisterWaypoint (CreateWaypoint ());
				EditorUtility.SetDirty (Navigation.Instance);
			}
			GUI.enabled = m_SelectedWaypoint != null;
			if (GUILayout.Button ("-", EditorStyles.toolbarButton))
			{
				Navigation.UnregisterWaypoint (m_SelectedWaypoint);
				DestroyImmediate (m_SelectedWaypoint.gameObject);
				EditorUtility.SetDirty (Navigation.Instance);
			}
			GUI.enabled = true;
		GUILayout.EndHorizontal ();
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
}

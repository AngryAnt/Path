using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof (Waypoint))]
public class WaypointInspector : Editor
{
	private List<Waypoint> m_Waypoints = new List<Waypoint> ();
	private string[] m_WaypointNames = new string[0];
	private int m_WaypointDropDownIndex = 0;
	
	private string[] m_ConnectionNames = new string[0];
	private int m_ConnectionDropDownIndex = 0;
	
	const float kConnectionListHeight = 200.0f;
	
	
	void OnEnable ()
	{
		m_Waypoints = new List<Waypoint> (Navigation.Waypoints);
		m_Waypoints.Remove (target as Waypoint);
		m_WaypointNames = m_Waypoints.ConvertAll (new System.Converter<Waypoint, string> (WaypointToString)).ToArray ();
		
		m_ConnectionNames = System.Array.ConvertAll (((Waypoint)target).Connections, new System.Converter<Connection, string> (ConnectionToString));
	}
	
	
	string WaypointToString (Waypoint waypoint)
	{
		return waypoint.ToString ();
	}
	
	
	string ConnectionToString (Connection connection)
	{
		return connection.ToString ();
	}
	
	
	public override void OnInspectorGUI ()
	{
		Waypoint waypoint = target as Waypoint;
		
		if (target == null)
		{
			return;
		}
		
		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Nav", EditorStyles.miniButton, GUILayout.Width (35)))
			{
				Selection.activeObject = Navigation.Instance.gameObject;
			}
		GUILayout.EndHorizontal ();
		
		EditorGUILayout.Space ();
		
		waypoint.Radius = EditorGUILayout.FloatField ("Radius", waypoint.Radius);
		waypoint.Tag = EditorGUILayout.TagField ("Tag", waypoint.Tag);
		
		EditorGUILayout.Space ();
		
		if (m_ConnectionNames.Length != 0)
		{
			GUILayout.Label ("Connections", EditorStyles.boldLabel);
			m_ConnectionDropDownIndex = EditorGUILayout.Popup (m_ConnectionDropDownIndex, m_ConnectionNames);
			waypoint.Connections[m_ConnectionDropDownIndex].Width = EditorGUILayout.FloatField ("Width", waypoint.Connections[m_ConnectionDropDownIndex].Width);
			waypoint.Connections[m_ConnectionDropDownIndex].Tag = EditorGUILayout.TagField ("Tag", waypoint.Connections[m_ConnectionDropDownIndex].Tag);
		}
		
		EditorGUILayout.Space ();
		
		if (m_Waypoints.Count != 0)
		{
			GUILayout.Label ("New connection", EditorStyles.boldLabel);
			m_WaypointDropDownIndex = EditorGUILayout.Popup (m_WaypointDropDownIndex, m_WaypointNames);
			
			if (GUILayout.Button ("Connect", EditorStyles.miniButton))
			{
				new Connection (waypoint, m_Waypoints[m_WaypointDropDownIndex]);
				EditorUtility.SetDirty (Navigation.Instance);
			}
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty (waypoint);
		}
	}
}

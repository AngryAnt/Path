#if EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class PathAbout : EditorWindow
{
	Vector3 m_Scroll = Vector3.zero;
	
	
	public PathAbout ()
	{
		title = "About Path";
		position = new Rect ((Screen.width - 500.0f) / 2.0f, (Screen.height - 400.0f) / 2.0f, 500.0f, 400.0f);
		minSize = new Vector2 (500.0f, 400.0f);
		maxSize = new Vector2 (500.0f, 400.0f);
	}
	
	
	public void OnGUI ()
	{
		GUILayout.BeginHorizontal ();
			GUILayout.Label (Resources.LogoShadow);
			GUILayout.BeginVertical ();
				GUILayout.BeginVertical (GUILayout.Height (70.0f));
					GUILayout.FlexibleSpace ();
					GUILayout.Label (Resources.PathLogo);
					GUILayout.Label (Resources.Version);
				GUILayout.EndVertical ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		
		GUILayout.Space (20.0f);
		
		GUILayout.BeginHorizontal ();
			GUILayout.Space (5.0f);
			GUILayout.BeginVertical ();
				m_Scroll = GUILayout.BeginScrollView (m_Scroll, GUI.skin.GetStyle ("Box"));
					GUILayout.Label (Resources.License, Resources.WrappedMiniLabelStyle);
				GUILayout.EndScrollView ();
			GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
		
		GUILayout.FlexibleSpace ();
		GUILayout.Space (20.0f);
				
		GUILayout.BeginHorizontal ();
			GUILayout.Space (5.0f);
			GUILayout.BeginVertical ();
				GUILayout.Label ("Thanks to " + Resources.Thanks + ".", Resources.WrappedLabelStyle);
				GUILayout.Space (10.0f);
				GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					GUILayout.Label (Resources.Copyright);
				GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.Space (5.0f);
		GUILayout.EndHorizontal ();
	}
}
#endif

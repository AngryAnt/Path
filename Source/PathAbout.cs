/*
Path - distribution of the 'Path' pathfinding system
version 2.0.1b1, April, 2011

Copyright (C) 2011 by AngryAnt, Emil Johansen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#if EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using PathRuntime;
using Resources = PathRuntime.Resources;


namespace PathEditor
{
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
}
#endif

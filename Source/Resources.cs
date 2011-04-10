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

using System;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace PathRuntime
{
	public class Resources
	{
		public static void Assert (bool assertion, string message = "")
		{
			if (!assertion)
			{
				throw new ApplicationException (string.IsNullOrEmpty (message) ? "Assert failed" : message);
			}
		}


		public static string Version
	    {
	    	get
	    	{
	    		#if DEBUG
	        		return "2.0.1b1 DEBUG";
	        	#else
	        		return "2.0.1b1";
	        	#endif
	    	}
	    }


		public static string License
		{
			get
			{
				#if DEBUG
					return "Path - distribution of the 'Path' pathfinding system\nversion 2.0.1b1 DEBUG, April, 2011\n\nCopyright (C) 2011 by AngryAnt, Emil Johansen\n\nPermission is hereby granted, free of charge, to any person obtaining a copy\nof this software and associated documentation files (the \"Software\"), to deal\nin the Software without restriction, including without limitation the rights\nto use, copy, modify, merge, publish, distribute, sublicense, and/or sell\ncopies of the Software, and to permit persons to whom the Software is\nfurnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in\nall copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\nFITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\nAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\nLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\nOUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN\nTHE SOFTWARE.";
				#else
					return "Path - distribution of the 'Path' pathfinding system\nversion 2.0.1b1, April, 2011\n\nCopyright (C) 2011 by AngryAnt, Emil Johansen\n\nPermission is hereby granted, free of charge, to any person obtaining a copy\nof this software and associated documentation files (the \"Software\"), to deal\nin the Software without restriction, including without limitation the rights\nto use, copy, modify, merge, publish, distribute, sublicense, and/or sell\ncopies of the Software, and to permit persons to whom the Software is\nfurnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in\nall copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\nIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\nFITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\nAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\nLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\nOUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN\nTHE SOFTWARE.";
				#endif
			}
		}


		public static string Thanks
		{
			get
			{
				return "Ann-Louise Salomonsen, family and friends, Unity Technologies, aiGameDev.com, Ricardo J. Mendez and the Unity community and the creatures of the #unity3d Freenode IRC channel";
			}
		}


		public static string Copyright
		{
			get
			{
				return "Copyright (C) Emil Johansen - AngryAnt 2011";
			}
		}


		private static Texture2D
			s_Logo,
			s_LogoShadow,
			s_PathLogo;


		public static Texture2D Logo
		{
			get
			{
				if (s_Logo == null)
				{
					s_Logo = GetTextureResource ("Logo.png");
				}

				return s_Logo;
			}
		}


		public static Texture2D LogoShadow
		{
			get
			{
				if (s_LogoShadow == null)
				{
					s_LogoShadow = GetTextureResource ("LogoShadow.png");
				}

				return s_LogoShadow;
			}
		}


		public static Texture2D PathLogo
		{
			get
			{
				if (s_PathLogo == null)
				{
					s_PathLogo = GetTextureResource ("PathLogo.png");
				}

				return s_PathLogo;
			}
		}


		private static GUIStyle s_WrappedLabel, s_WrappedMiniLabel;


		internal static GUIStyle WrappedLabelStyle
		{
			get
			{
				if (s_WrappedLabel == null)
				{
					s_WrappedLabel = new GUIStyle (GUI.skin.GetStyle ("Label"));
					s_WrappedLabel.margin = new RectOffset ();
					s_WrappedLabel.wordWrap = true;
				}
				return s_WrappedLabel;
			}
		}


		internal static GUIStyle WrappedMiniLabelStyle
		{
			get
			{
				if (s_WrappedMiniLabel == null)
				{
					s_WrappedMiniLabel = new GUIStyle (GUI.skin.GetStyle ("MiniLabel"));
					s_WrappedMiniLabel.wordWrap = true;
				}
				return s_WrappedMiniLabel;
			}
		}


		// Resource loading //


		internal static Stream GetResourceStream (string resourceName, Assembly assembly)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetExecutingAssembly ();
			}

			return assembly.GetManifestResourceStream (resourceName);
		}


		internal static Stream GetResourceStream (string resourceName)
		{
			return GetResourceStream (resourceName, null);
		}


		internal static byte[] GetByteResource (string resourceName, Assembly assembly)
		{
			Stream byteStream;
			byte[] buffer;

			byteStream = GetResourceStream (resourceName, assembly);
			buffer = new byte[byteStream.Length];
			byteStream.Read (buffer, 0, (int)byteStream.Length);
			byteStream.Close ();

			return buffer;
		}


		internal static byte[] GetByteResource (string resourceName)
		{
			return GetByteResource (resourceName, null);
		}


		internal static Texture2D GetTextureResource (string resourceName, Assembly assembly)
		{
			Texture2D texture;

			texture = new Texture2D (4, 4);
			texture.LoadImage (GetByteResource (resourceName, assembly));

			return texture;
		}


		internal static Texture2D GetTextureResource (string resourceName)
		{
			return GetTextureResource (resourceName, typeof (Resources).Assembly);
		}
	}
}

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
	        		return "2.0.0f2 DEBUG";
	        	#else
	        		return "2.0.0f2";
	        	#endif
	    	}
	    }


		public static string License
		{
			get
			{
				#if DEBUG
					return "Path - distribution of the 'Path' pathfinding system\nversion 2.0.0f2 DEBUG, February, 2011\n\nCopyright (C) AngryAnt, Emil Johansen\n\nThis software is provided 'as-is', without any express or implied\nwarranty.  In no event will the author be held liable for any damages\narising from the use of this software.\n\nPermission is granted to anyone to use this software for any purpose\nand redistribute it freely, subject to the following restrictions:\n\n1. The origin of this software must not be misrepresented; you must not\n   claim that you wrote the original software. If you use this software\n   in a product, an acknowledgment is required as stated in point 2 of\n   this notice.\n2. If you use this software in a product, you are required to acknowledge\n   its original creator by displaying at least one unaltered version of the\n   provided software logo images in your product splash screen for no less\n   than four full seconds. If your product has an \"About\" or credits view,\n   this same logo image must also be displayed there.\n3. The original software author may, in any customer reference list or in\n   any press release, use the name of products, individuals and companies\n   using this software.\n4. This Agreement will be governed by the laws of the State of Denmark as\n   they are applied to agreements between Denmark residents entered into\n   and to be performed entirely within Denmark. The United Nations\n   Convention on Contracts for the International Sale of Goods is\n   specifically disclaimed.\n5. Whether you are licensing the Software as an individual or on behalf of\n   an entity, you may not: (a) reverse engineer, decompile, or disassemble\n   the Software or attempt to discover the source code; (b) modify the\n   Software runtime in whole or in part without the express written consent\n   of the original software creator; (c) remove any proprietary notices or\n   labels on the Software; (d) resell, lease, rent, transfer, sublicense,\n   or otherwise transfer rights to the Software.\n6. You agree that this is the entire agreement between you and the original\n   software creator, which supersedes any prior agreement, whether written\n   or oral, and all other communications between the original software\n   creator and you relating to the subject matter of this Agreement.\n7. This notice and the provided logo image files may not be removed or\n   altered from any source distribution.\n\nAny of these requirement may be waived for an additional fee. Contact the\noriginal software author for details.\n\nAngryAnt, Emil Johansen - emil@eej.dk";
				#else
					return "Path - distribution of the 'Path' pathfinding system\nversion 2.0.0f2, February, 2011\n\nCopyright (C) AngryAnt, Emil Johansen\n\nThis software is provided 'as-is', without any express or implied\nwarranty.  In no event will the author be held liable for any damages\narising from the use of this software.\n\nPermission is granted to anyone to use this software for any purpose\nand redistribute it freely, subject to the following restrictions:\n\n1. The origin of this software must not be misrepresented; you must not\n   claim that you wrote the original software. If you use this software\n   in a product, an acknowledgment is required as stated in point 2 of\n   this notice.\n2. If you use this software in a product, you are required to acknowledge\n   its original creator by displaying at least one unaltered version of the\n   provided software logo images in your product splash screen for no less\n   than four full seconds. If your product has an \"About\" or credits view,\n   this same logo image must also be displayed there.\n3. The original software author may, in any customer reference list or in\n   any press release, use the name of products, individuals and companies\n   using this software.\n4. This Agreement will be governed by the laws of the State of Denmark as\n   they are applied to agreements between Denmark residents entered into\n   and to be performed entirely within Denmark. The United Nations\n   Convention on Contracts for the International Sale of Goods is\n   specifically disclaimed.\n5. Whether you are licensing the Software as an individual or on behalf of\n   an entity, you may not: (a) reverse engineer, decompile, or disassemble\n   the Software or attempt to discover the source code; (b) modify the\n   Software runtime in whole or in part without the express written consent\n   of the original software creator; (c) remove any proprietary notices or\n   labels on the Software; (d) resell, lease, rent, transfer, sublicense,\n   or otherwise transfer rights to the Software.\n6. You agree that this is the entire agreement between you and the original\n   software creator, which supersedes any prior agreement, whether written\n   or oral, and all other communications between the original software\n   creator and you relating to the subject matter of this Agreement.\n7. This notice and the provided logo image files may not be removed or\n   altered from any source distribution.\n\nAny of these requirement may be waived for an additional fee. Contact the\noriginal software author for details.\n\nAngryAnt, Emil Johansen - emil@eej.dk";
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

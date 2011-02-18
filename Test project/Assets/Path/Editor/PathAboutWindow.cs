using UnityEditor;
using PathEditor;

public class PathAboutWindow : PathAbout
{
	[MenuItem ("Help/About Path...")]
	static void Launch ()
	{
		GetWindow (typeof (PathAboutWindow));
	}
}

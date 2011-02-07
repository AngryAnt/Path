using UnityEditor;

public class PathAboutWindow : PathAbout
{
	[MenuItem ("Help/About Path...")]
	static void Launch ()
	{
		GetWindow (typeof (PathAboutWindow));
	}
}

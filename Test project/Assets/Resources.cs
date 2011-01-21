public class Resources
{
	public static void Assert (bool assertion, string message = "")
	{
		if (!assertion)
		{
			throw new System.ApplicationException (string.IsNullOrEmpty (message) ? "Assert failed" : message);
		}
	}
}

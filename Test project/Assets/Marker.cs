using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour
{
	public Color m_Color = Color.white;
	public float m_Radius = 1.0f;

	void OnDrawGizmos ()
	{
		Gizmos.color = m_Color;
		Gizmos.DrawWireSphere (transform.position, m_Radius);
	}
}

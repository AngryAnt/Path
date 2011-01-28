using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class SeekerData : System.IComparable
{
	private List<Connection> m_Path = new List<Connection> ();
	private float m_GScore, m_HScore;
	
	
	public SeekerData (Connection connection, float gScore, float hScore)
	{
		m_Path.Add (connection);
		m_GScore = gScore;
		m_HScore = hScore;
	}
	
	
	public SeekerData (SeekerData original, Connection connection, float gScore, float hScore)
	{
		m_Path = new List<Connection> (original.Path);
		m_Path.Add (connection);
		m_GScore = original.GScore + gScore;
		m_HScore = original.HScore + hScore;
	}
	
	
	public IList<Connection> Path
	{
		get
		{
			return m_Path.AsReadOnly ();
		}
	}
	
	
	public float GScore
	{
		get
		{
			return m_GScore;
		}
	}
	
	
	public float HScore
	{
		get
		{
			return m_HScore;
		}
	}
	
	
	public float FScore
	{
		get
		{
			return m_GScore + m_HScore;
		}
	}
	
	
	public Connection LastSegment
	{
		get
		{
			return m_Path[m_Path.Count - 1];
		}
	}
	
	
	public Waypoint Destination
	{
		get
		{
			return LastSegment.To;
		}
	}
	
	
	public List<Connection> Options
	{
		get
		{
			List<Connection> connections = new List<Connection> (Destination.Connections);
			
			if (Path.Count > 1)
			{
				connections.RemoveAll (x => x.To != LastSegment.From);
			}
			
			return connections;
		}
	}
	
	
	public int CompareTo (object other)
	{
		SeekerData seeker = other as SeekerData;
		
		if (seeker == null)
		{
			throw new System.ApplicationException ("Invalid SeekerData provided for comparison");
		}
		
		return FScore.CompareTo (seeker.FScore);
	}
}

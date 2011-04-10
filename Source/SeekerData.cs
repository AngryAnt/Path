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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace PathRuntime
{
	internal class SeekerData : System.IComparable
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
					connections.RemoveAll (x => x.To == LastSegment.From);
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


		public override string ToString ()
		{
			string value = "Path from " + Path[0].From;

			foreach (Connection connection in Path)
			{
				value += " to " + connection.To;
			}

			return value + ".";
		}
	}
}

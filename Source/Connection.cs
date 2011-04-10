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


namespace PathRuntime
{
	/// A connection between two Path nodes.
	[System.Serializable]
	public class Connection
	{
		[SerializeField]
		private Waypoint m_From, m_To;
		[SerializeField]
		private float m_Width = 1.0f;
		[SerializeField]
		private float m_Weight = 1.0f;
		[SerializeField]
		private string m_Tag = "Untagged";
		[SerializeField]
		private bool m_Enabled = true;


		/// Forms a new connection between two nodes. The connection lists itself with the from node.
		public Connection (Waypoint from, Waypoint to)
		{
			Resources.Assert (from != null && to != null && from != to);

			m_From = from;
			m_To = to;

			if (!m_From.ConnectsTo (m_To))
			{
				m_From.AddConnection (this);
			}
		}


		/// The origin of the connection.
		public Waypoint From
		{
			get
			{
				return m_From;
			}
		}


		/// The target of the connection.
		public Waypoint To
		{
			get
			{
				return m_To;
			}
		}


		/// The width of the connection. Connections narrower than the pathfinding Navigator will not be picked.
		public float Width
		{
			get
			{
				return m_Width;
			}
			set
			{
				m_Width = value > 0 ? value : m_Width;
			}
		}
		
		
		/// The weight of the connection. A factor which increases or decreases the cost of the connection when
		/// pathfinding. Default is 1.
		public float Weight
		{
			get
			{
				return m_Weight;
			}
			set
			{
				m_Weight = value != 0 ? value : m_Weight;
			}
		}


		/// The Connection Tag. This is used to weigh the Connection when pathfinding, assuming the pathfinding
		/// Navigator has registered any weight handlers with the tag.
		public string Tag
		{
			get
			{
				return m_Tag;
			}
			set
			{
				m_Tag = value;
			}
		}


		/// The Connection Enabled flag. If set to false, pathfinding will ignore the Connection and any already
		/// found pathes going via this Connection will be invalidated.
		public bool Enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				m_Enabled = value;
				if (!m_Enabled)
				{
					Navigation.OnDisable (this);
				}
			}
		}


		/// The Connection cost. This is the pathfinding cost of the connection, before any tag-based weighting.
		public float Cost
		{
			get
			{
				return (To.Position - From.Position).sqrMagnitude;
			}
		}


		/// Overridden for easy debugging.
		public override string ToString ()
		{
			return "Connection to " + To.ToString ();
		}
	}
}

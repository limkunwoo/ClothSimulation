using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class Edge
    {
        public Vertex p1, p2;
        public int index;
        public List<Triangle> triangles = new List<Triangle>();

        public List<Vertex> otherV = new List<Vertex>();

        public Edge(Vertex p1, Vertex p2, int index)
        {
            this.p1 = p1; this.p2 = p2;
            this.index = index;
        }

        public void DebugLine()
        {
            Debug.DrawLine(p1.Pos, p2.Pos, Color.red);
        }

        public void AddSharedTriangle()
        {
            foreach(Triangle t in p1.nb_faces)
            {
                if(t.IsContainVertex(p1) && t.IsContainVertex(p2))
                {
                    triangles.Add(t);
                    otherV.Add(t.GetOtherVertex(this));
                }
            }
        }
    }
}

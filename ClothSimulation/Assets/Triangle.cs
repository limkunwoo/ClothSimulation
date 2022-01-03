using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class Triangle
    {
        public Vertex p1, p2, p3;
        List<Vertex> pos = new List<Vertex>();
        List<Edge> nb_edges = new List<Edge>();
        int flag;
        public Triangle(Vertex p1, Vertex p2, Vertex p3)
        {
            this.p1 = p1; this.p2 = p2; this.p3 = p3;
            pos.Add(p1);
            pos.Add(p2);
            pos.Add(p3);
        }

        public bool IsContainVertex(Vertex p)
        {
            if (p1 == p)
                return true;
            if (p2 == p)
                return true;
            if (p3 == p)
                return true;
            return false;
        }
        public int GetIndex(Vertex p)
        {
            if (p1 == p)
                return 0;
            else if (p2 == p)
                return 1;
            else
                return 2;
        }
        public Vertex GetOtherVertex(Edge e)
        {

            if (e.p1 != p1 && e.p2 != p1)
                return p1;
            if (e.p1 != p2 && e.p2 != p2)
                return p2;
            if (e.p1 != p3 && e.p2 != p3)
                return p3;
            return null;
        }
    }
}

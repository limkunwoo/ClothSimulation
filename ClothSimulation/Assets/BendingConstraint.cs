using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class BendingConstraint
    {
        Vertex b0, b1, v;

        float rest_h;
        float total_weight;

        float stiffness;
        float bending_parameter;

        public BendingConstraint(Vertex b0, Vertex b1, Vertex v)
        {
            this.b0 = b0;
            this.b1 = b1;
            this.v = v;

            Vector3 c = (b0.Pos + b1.Pos + v.Pos) / 3;
            rest_h = (v.Pos - c).magnitude;
            //Debug.Log("rest_h : " + rest_h);

            total_weight = b0.mass + b1.mass + 2 * v.mass;

            stiffness = 0;
            bending_parameter = 0;
        }
        public void satisfyConstraint()
        {
            Vector3 c = (b0.Pos + b1.Pos + v.Pos) / 3;
            Vector3 vec = v.Pos - c;

            Vector3 d_b0 = new Vector3(0,0,0);
            Vector3 d_b1 = new Vector3(0, 0, 0);
            Vector3 d_v = new Vector3(0, 0, 0);
            if (vec.magnitude != 0)
            {
                d_b0 = vec * (2 * b0.mass / total_weight) * (1 - (bending_parameter + rest_h) / vec.magnitude);
                d_b1 = vec * (2 * b1.mass / total_weight) * (1 - (bending_parameter + rest_h) / vec.magnitude);
                d_v = -vec * (4 * v.mass / total_weight) * (1 - (bending_parameter + rest_h) / vec.magnitude);
            }

            b0.MovePosition(d_b0);
            b1.MovePosition(d_b1);
            v.MovePosition(d_v);
        }
        public void DebugLine()
        {
            Vector3 c = (b0.Pos + b1.Pos + v.Pos) / 3;
            Vector3 vec = v.Pos - c;
        }
    }
}

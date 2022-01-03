using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class Constraint
    {
        float rest_distance;
        Vertex p1, p2;

        public Constraint(Vertex p1, Vertex p2)
        {
            this.p1 = p1;
            this.p2 = p2;

            rest_distance = (p1.Pos - p2.Pos).magnitude;
        }

        public void SatisfyConstraint()
        {
            Vector3 vec = p2.Pos - p1.Pos;
            float current_length = vec.magnitude;

            Vector3 delta_p = vec.normalized * p1.mass / (p1.mass + p2.mass) * (current_length - rest_distance);
            p1.MovePosition(delta_p);
            p2.MovePosition(-delta_p);
        }
    }
}

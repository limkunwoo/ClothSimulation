using UnityEngine;

namespace Assets
{
    class DehegralConstraint
    {
        Vertex p1, p2, p3, p4;
        float rest_halfSinTheta = 0.5f;

        const float k_d = 0.1f;
        const float k_e = 0.8f;

        public DehegralConstraint(Vertex p1, Vertex p2, Vertex p3, Vertex p4)
        {
            this.p1 = p1; this.p2 = p2; this.p3 = p3; this.p4 = p4;

            Vector3 N1 = Vector3.Cross(p1.Pos - p3.Pos, p1.Pos - p2.Pos);
            Vector3 N2 = Vector3.Cross(p4.Pos - p2.Pos, p4.Pos - p3.Pos);
            Vector3 E = p2.Pos - p3.Pos;

            rest_halfSinTheta = Vector3.Dot(Vector3.Cross(N1.normalized, N2.normalized), E.normalized);

        }
        
        public void satisfyConstraint()
        {
            Vector3 N1 = Vector3.Cross(p1.Pos - p3.Pos, p1.Pos - p2.Pos);
            Vector3 N2 = Vector3.Cross(p4.Pos - p2.Pos, p4.Pos - p3.Pos);
            float N1_norm = N1.magnitude;
            float N2_norm = N2.magnitude;
            Vector3 N1_mode = N1 / (N1_norm * N1_norm);
            Vector3 N2_mode = N2 / (N2_norm * N2_norm);

            Vector3 E = p2.Pos - p3.Pos;
            float E_norm = E.magnitude;

            Vector3 n1 = N1.normalized;
            Vector3 n2 = N2.normalized;
            Vector3 e = E.normalized;

            Vector3 u1 = E_norm * N1_mode;
            Vector3 u2 = E_norm * N2_mode;
            Vector3 u3 = Vector3.Dot(p1.Pos - p2.Pos, E) / E_norm * N1_mode + Vector3.Dot(p4.Pos - p2.Pos, E) / E_norm * N2_mode;
            Vector3 u4 = -Vector3.Dot(p1.Pos - p3.Pos, E) / E_norm * N1_mode - Vector3.Dot(p4.Pos - p3.Pos, E) / E_norm * N2_mode;

            float sin_halfTheta = Vector3.Dot(Vector3.Cross(n1, n2), e);
            Vector3 Fe_1 = k_e * E_norm * E_norm / (N1_norm + N2_norm) * (sin_halfTheta - rest_halfSinTheta) * u1;
            Vector3 Fe_2 = k_e * E_norm * E_norm / (N1_norm + N2_norm) * sin_halfTheta * u2;
            Vector3 Fe_3 = k_e * E_norm * E_norm / (N1_norm + N2_norm) * sin_halfTheta * u3;
            Vector3 Fe_4 = k_e * E_norm * E_norm / (N1_norm + N2_norm) * sin_halfTheta * u4;

            if (p1.index == 150)
            {
                //Debug.DrawLine(p1.Pos, p1.Pos + Fe_1);
                //Debug.Log(Fe_1);
            }
            float delta = Vector3.Dot(u1, p1.Vel) + Vector3.Dot(u2, p2.Vel) + Vector3.Dot(u3, p3.Vel) + Vector3.Dot(u4, p4.Vel);
            Vector3 Fd_1 = -k_d * E_norm * delta * u1;
            Vector3 Fd_2 = -k_d * E_norm * delta * u2;
            Vector3 Fd_3 = -k_d * E_norm * delta * u3;
            Vector3 Fd_4 = -k_d * E_norm * delta * u4;

            if (p1.index == 150)
            {
                //Debug.DrawLine(p1.Pos, p1.Pos + Fd_1, Color.blue);
                //Debug.Log(Fd_1);
            }
            p1.AddForce(Fe_1);
            p2.AddForce(Fe_2);
            p3.AddForce(Fe_3);
            p4.AddForce(Fe_4);

            p1.AddForce(Fd_1);
            p2.AddForce(Fd_2);
            p3.AddForce(Fd_3);
            p4.AddForce(Fd_4);

        }
    }
}

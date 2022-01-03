using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class Vertex
    {
        const float RELATION_TIME = 0.01f;
        const float DAMPING = 0.99f;
        const float FRICTION = 0.7f;
        bool movable;
        public bool flag = false;

        Vector3 prevPos;
        public Vector3 Vel { get; set; }
        Vector3 acc;

        public Vector3 Pos;
        public float mass;

        public List<int> cons;
        public List<Vertex> nb_vertices;
        public List<Triangle> nb_faces;

        public int index;
        GameObject ball;

        public Vertex(Vector3 position, int index)
        {
            this.index = index;
            Pos = position;
            prevPos = position;
            acc = new Vector3();
            Vel = new Vector3(0, 0, 0);
            movable = true;
            mass = 1.0f;
            nb_vertices = new List<Vertex>();
            nb_faces = new List<Triangle>();
            cons = new List<int>();
            ball = GameObject.FindGameObjectWithTag("ball");
        }

        public void AddForce(Vector3 f)
        {
            acc += f / mass;
        }
        public void Intergrate()
        {
            if (movable)
            {

                Vector3 ballPosition = ball.transform.position;
                Vector3 v = BallScript.GetVelocity();
                Vector3 vp = (Pos - prevPos) / Time.deltaTime;
                Vector3 N = (Pos - ballPosition).normalized;
                //square equation = x^2 + y^2 + z^2 = r^2
                float level_set_value = (Pos - ballPosition).magnitude - 2.0f;
                float new_lv_value = level_set_value + Vector3.Dot((vp - v), N) * Time.deltaTime;

                if (new_lv_value < 0)
                {
                    float vN = Vector3.Dot(v, N);
                    Vector3 vT = v - vN * N;

                    float vpN = Vector3.Dot(vp, N);
                    Vector3 vpT = vp - vpN * N;

                    float new_vpN = vN - level_set_value / RELATION_TIME;

                    Vector3 rel_vT = vpT - vT;
                    Vector3 new_rel_vT = rel_vT * Mathf.Max(0, 1 - FRICTION * ((new_vpN - vpN) / rel_vT.magnitude));

                    Vector3 new_vpT = vT + new_rel_vT;

                    Vel = new_vpN * N + new_vpT;

                    Vector3 temp = Pos;
                    Pos = Pos + Vel * Time.deltaTime * DAMPING + acc * Time.deltaTime * Time.deltaTime * 0.5f;
                    prevPos = temp;
                }
                else
                {
                    Vel = Pos - prevPos;
                    Vector3 temp = Pos;
                    Pos = Pos + Vel * DAMPING + acc * Time.deltaTime * Time.deltaTime;
                    prevPos = temp;
                }
                ResetAcc();
            }
        }
        public void FixParticle()
        {
            movable = false;
        }
        public void MovePosition(Vector3 offset)
        {
            if (movable)
                Pos += offset;
        }
        void ResetAcc()
        {
            acc.x = 0;
            acc.y = 0;
            acc.z = 0;
        }
    }
}

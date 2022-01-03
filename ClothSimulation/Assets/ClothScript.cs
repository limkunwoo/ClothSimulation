using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using System;

public class ClothScript : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;

    public int resolution = 20;
    public int size = 10;
    public bool shouldApplyBendingConstraint;
    public bool isFlat;
    
    
    List<Vertex> vertices = new List<Vertex>();
    List<Constraint> constraints = new List<Constraint>();
    List<BendingConstraint> bendingConstraints = new List<BendingConstraint>();
    List<DehegralConstraint> dehegralConstraints = new List<DehegralConstraint>();
    // Start is called before the first frame update
    
    void Start()
    {
        meshFilter = transform.GetComponent<MeshFilter>();
        GenerateMesh();
        
        //양쪽 끝 고정
        for(int i = 0; i < 3; i++)
        {
            GetParticle(0 + i, 0).FixParticle();
            GetParticle(resolution - i - 1, 0).FixParticle();
        }
        
        //제약조건 형성
        for (int y = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                if( x < resolution - 1)
                    SetConstraint(GetParticle(x, y), GetParticle(x+1, y));
                if (y < resolution - 1)
                    SetConstraint(GetParticle(x, y), GetParticle(x, y + 1));

                //x모양
                
                if (x < resolution - 1 && y < resolution - 1)
                {
                    SetConstraint(GetParticle(x, y), GetParticle(x + 1, y + 1));
                    SetConstraint(GetParticle(x + 1, y), GetParticle(x, y + 1));

                }
                
                //bendig Constraint 계산
                Vertex v = GetParticle(x, y);
                foreach (Vertex vi in v.nb_vertices)
                {
                    float cos_best = 1;
                    Vertex v_best = vi;
                    foreach(Vertex vj in v.nb_vertices)
                    {
                        float cos = Vector3.Dot((vi.Pos - v.Pos).normalized, (vj.Pos - v.Pos).normalized);
                        if(cos <= cos_best)
                        {
                            cos_best = cos;
                            v_best = vj;
                        }
                    }
                    if(!v_best.cons.Contains(vi.index))
                    {
                        vi.cons.Add(v_best.index);
                        v_best.cons.Add(vi.index);
                        SetBendingConstraint(vi, v_best, v);
                    }
                }
                if ((x < (resolution - 1)) && (y > 0) && (y < resolution - 1)) 
                    dehegralConstraints.Add(new DehegralConstraint(GetParticle(x + 1, y-1), GetParticle(x + 1, y), GetParticle(x, y), GetParticle(x, y + 1)));
                if ((y < (resolution - 1)) && (x > 0) && (x < resolution - 1)) 
                    dehegralConstraints.Add(new DehegralConstraint(GetParticle(x - 1, y + 1), GetParticle(x , y), GetParticle(x, y + 1), GetParticle(x + 1, y)));
                if ((x < resolution - 1)&&(y < resolution - 1)) 
                    dehegralConstraints.Add(new DehegralConstraint(GetParticle(x, y), GetParticle(x + 1, y), GetParticle(x, y + 1), GetParticle(x + 1, y + 1)));
            }
            
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //중력
        AddForce(new Vector3(0, -9.8f, 0));
        Intergrate();

        ReMesh();
    }
    void SetBendingConstraint(Vertex b0, Vertex b1, Vertex v)
    {
        bendingConstraints.Add(new BendingConstraint(b0, b1, v));
    }
    //적분기
    public void Intergrate()
    {

        for (int i = 0; i < 5; i++)
        {
            
            for (int j = 0; j < constraints.Count; j++)
            {
               constraints[j].SatisfyConstraint();
            }
            for (int j = 0; j < dehegralConstraints.Count; j++)
            {
                //dehegralConstraints[j].satisfyConstraint();
            }
            if (shouldApplyBendingConstraint)
            {
                for (int j = 0; j < bendingConstraints.Count; j++)
                {
                    bendingConstraints[j].satisfyConstraint();
                }
            }
        }
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].Intergrate();
        }
    }
    //x,y 좌표의 particle 가져오기
    Vertex GetParticle(int x, int y)
    {
        return vertices[resolution * y + x];
    }

    void SetConstraint(Vertex p1, Vertex p2)
    {
        constraints.Add(new Constraint(p1, p2));
    }
    //Cloth의 메쉬 구성
    public void GenerateMesh()
    {
        //List<Vector3> vertices = new List<Vector3>();
        
        List<int> triangles = new List<int>();
        List<Vector2> uvPos = new List<Vector2>();

        //적당한 해상도로 메쉬 재구성
        for(int y = 0; y < resolution; y ++)
        {
            for(int x = 0; x < resolution; x++)
            {
                Vector3 currentPosition = new Vector3();
                Vector2 currentUV = new Vector2();

                currentPosition.x = (x/(float)(resolution - 1)) * size;
                currentPosition.y = -(y/(float)(resolution - 1)) * size;
                currentPosition.z = transform.position.z;
                //Cloth 모양을 지그재그로 만듬 (Bending Constraint를 잘 확인하기 위해서)
                if(y%2 == 0 && !isFlat)
                {
                    currentPosition.z =  - 0.5f;
                }
                currentPosition += (transform.position - new Vector3(size / 2, -size / 2, transform.position.z));
                vertices.Add(new Vertex(currentPosition, y * resolution + x));

                currentUV.x = x / (float)(resolution - 1);
                currentUV.y = y / (float)(resolution - 1);
                uvPos.Add(currentUV);
                if (x < resolution - 1 &&  y < resolution - 1)
                {
                    triangles.Add(y * resolution + x);
                    triangles.Add(y * resolution + x + 1);
                    triangles.Add((y + 1) * resolution + x);

                    triangles.Add((y + 1) * resolution + (x + 1));
                    triangles.Add((y + 1) * resolution + x);
                    triangles.Add(y * resolution + (x + 1));
                }
            }
        }
        //이웃 버텍스 설정
        
        for(int i = 0; i <triangles.Count; i += 3)
        {
            int v1_index = triangles[i];
            int v2_index = triangles[i + 1];
            int v3_index = triangles[i + 2];

            Vertex v1 = vertices[v1_index];
            Vertex v2 = vertices[v2_index];
            Vertex v3 = vertices[v3_index];

            if (!v1.nb_vertices.Contains(v2)) v1.nb_vertices.Add(v2);
            if (!v1.nb_vertices.Contains(v3)) v1.nb_vertices.Add(v3);

            if (!v2.nb_vertices.Contains(v1)) v2.nb_vertices.Add(v1);
            if (!v2.nb_vertices.Contains(v3)) v2.nb_vertices.Add(v3);

            if (!v3.nb_vertices.Contains(v1)) v3.nb_vertices.Add(v1);
            if (!v3.nb_vertices.Contains(v2)) v3.nb_vertices.Add(v2);
        }

        Vector3[] meshVertices = new Vector3[resolution * resolution];
        for(int i = 0; i < vertices.Count; i++)
        {
            meshVertices[i] = vertices[i].Pos;
        }

        mesh = new Mesh();
        mesh.vertices = meshVertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvPos.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;
        meshFilter.mesh.name = "Cloth";
    }
    public void AddForce(Vector3 force)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].AddForce(force);
        }
    }
    
    public void ReMesh()
    {
        Vector3[] new_vertices = transform.GetComponent<MeshFilter>().mesh.vertices;
        for (int i = 0; i < new_vertices.Length; i++)
        {
            new_vertices[i] = vertices[i].Pos;
        }
        transform.GetComponent<MeshFilter>().mesh.vertices = new_vertices;
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }
}

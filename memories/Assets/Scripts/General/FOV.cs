using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FOV : MonoBehaviour {

    [Range(0, 360)]
    public float viewAngle;
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    [Range(0, 20)]
    public float rad;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    void Start() {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh; 
    }

    private void LateUpdate() {
        drawFieldOfView();
    }

    void drawFieldOfView() {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++) {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * rad, Color.red); //DEBUG-SCRIPT
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0) {
                //bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst) > edgeDstThreshold; //WRONG
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero) {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero) {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertCount = viewPoints.Count + 1;
        Vector3[] verts = new Vector3[vertCount];
        int[] tris = new int[(vertCount - 2) * 3];
        verts[0] = Vector3.zero;

        for (int i = 0; i < vertCount - 1; i++) {
            verts[i + 1] = transform.InverseTransformPoint(viewPoints [i]);

            if (i < vertCount - 2) {
                tris[i * 3] = 0;
                tris[i * 3 + 2] = i + 1;
                tris[i * 3 + 1] = i + 2;
            }
        }
        //Debug.Log("updated mesh with " + tris.Length + " tris" );

        viewMesh.Clear();
        viewMesh.vertices = verts;
        viewMesh.triangles = tris;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            //bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst) > edgeDstThreshold; //WRONG
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle) {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, dir, rad, obstacleMask);
        //Debug.DrawLine(transform.position, transform.position + dir * rad); //TODO delete
        if (hit.collider != null) {
            Debug.DrawLine(transform.position, hit.point); //DEBUG-CODE! Delete this before shipping!
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            Debug.DrawLine(transform.position, transform.position + dir * rad);  //DEBUG-CODE! Delete this before shipping!
            return new ViewCastInfo(false, transform.position + dir * rad, rad, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle) {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }

    }

    public struct EdgeInfo {
        public Vector3 PointA;
        public Vector3 PointB;
        public EdgeInfo(Vector3 _PointA, Vector3 _PointB) {
            PointA = _PointA;
            PointB = _PointB;
        }
    }
    
}

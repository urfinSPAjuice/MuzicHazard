using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(Enemie))]
public class EnemieEditor : Editor {

    private const string CHILD_NAME = "MeshColliderObject";

    //Enemie enemie;
    GameObject thisObject, childObject;

    public override void OnInspectorGUI() {
        //if (enemie == null) enemie = (Enemie)target;
        if (thisObject == null) thisObject = ((Enemie)target).gameObject;

        if (GUILayout.Button("Generate Mesh & Collider"))
        {

            bool polygonColliderWasOnObject = true;
            PolygonCollider2D colPoly = thisObject.GetComponent<PolygonCollider2D>();
            if (colPoly == null)
            {
                polygonColliderWasOnObject = false;
                colPoly = thisObject.AddComponent<PolygonCollider2D>();
            }

            Transform t = thisObject.transform.FindChild(CHILD_NAME);
            if (t != null) childObject = t.gameObject;
            if (childObject == null)
            {
                childObject = new GameObject();
                childObject.name = CHILD_NAME;
                childObject.transform.parent = thisObject.transform;
                childObject.transform.localPosition = Vector3.zero;
                childObject.transform.localRotation = Quaternion.identity;
                childObject.transform.localScale = Vector3.one;
            }

            Mesh mesh = new Mesh();
            mesh.name = thisObject.GetComponent<SpriteRenderer>().sprite.name + "_mesh";
            #region ConfigureMesh

            Vector2[] path = colPoly.GetPath(0);
            if (!polygonColliderWasOnObject) DestroyImmediate(colPoly);
            Vector3[] vertices = new Vector3[path.Length];

            for (int i = 0; i < path.Length; i++)
            {
                Debug.Log(path[i]);
                vertices[i] = new Vector3(path[i].x, path[i].y);
            }

            int[] tris = new int[(vertices.Length - 2) * 3];

            bool toSide = false;
            int curTrisIndex = 0;
            for (int i = 0; i < vertices.Length - 2; i++)
            {
                if (toSide)
                {
                    tris[curTrisIndex] = i;
                    tris[curTrisIndex + 1] = i + 1;
                    tris[curTrisIndex + 2] = i + 2;
                }
                else
                {
                    tris[curTrisIndex] = i + 2;
                    tris[curTrisIndex + 1] = i + 1;
                    tris[curTrisIndex + 2] = i;
                }
                toSide = !toSide;

                curTrisIndex += 3;
            }

            Vector3[] normals = new Vector3[vertices.Length];

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -Vector3.forward;
            }

            /*
            Vector2[] uv = new Vector2[vertices.Length];

            for (int i = 0; i < uv.Length; i += 2)
            {
                uv[i] = new Vector2((float)i / (uv.Length - 2) * TextureSize, (verticles[i].y / Height) * ((float)Height / Width) * TextureSize);
                uv[i + 1] = new Vector2((float)i / (uv.Length - 2) * TextureSize, (verticles[i + 1].y / Height) * ((float)Height / Width) * TextureSize);
            }//*/

            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.normals = normals;
            //mesh.uv = uv;
            mesh.RecalculateBounds();
            #endregion

            MeshFilter mFilter = childObject.GetComponent<MeshFilter>();
            if (mFilter == null) mFilter = childObject.AddComponent<MeshFilter>();
            mFilter.sharedMesh = mesh;

            MeshRenderer mRenderer = childObject.GetComponent<MeshRenderer>();
            if (mRenderer == null) mRenderer = childObject.AddComponent<MeshRenderer>();
            MeshCollider mCollider = childObject.GetComponent<MeshCollider>();
            if (mCollider != null)
            {
                DestroyImmediate(mCollider);
                childObject.AddComponent<MeshCollider>();
            }
        }
    }	
    
}

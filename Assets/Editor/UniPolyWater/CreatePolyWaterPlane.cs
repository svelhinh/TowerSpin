using UnityEngine;
using UnityEditor;
using System.Collections;


public class CreateUniPolyWaterPlane : ScriptableWizard
{
    public enum AnchorPoint
    {
        TopLeft,
        TopHalf,
        TopRight,
        RightHalf,
        BottomRight,
        BottomHalf,
        BottomLeft,
        LeftHalf,
        Center
    }

    public int widthSegments = 1;
    public int lengthSegments = 1;
    float width = 1.0f;
    float length = 1.0f;
    public AnchorPoint anchor = AnchorPoint.Center;
    public bool addCollider = false;
    public bool createAtOrigin = true;
    public string optionalName;

    static Camera cam;
    static Camera lastUsedCam;


    [MenuItem("GameObject/Create Other/UniPolyWater Plane..")]
    static void CreateWizard()
    {
        cam = Camera.current;
        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam)
            cam = lastUsedCam;
        else
            lastUsedCam = cam;
        ScriptableWizard.DisplayWizard("Create PolyWater Plane", typeof(CreateUniPolyWaterPlane));
    }


    void OnWizardUpdate()
    {
        widthSegments = Mathf.Clamp(widthSegments, (int)width, 100);
        lengthSegments = Mathf.Clamp(lengthSegments, (int)length, 100);
		width = widthSegments * 1.0f;
		length = lengthSegments * 1.0f;
    }


    void OnWizardCreate()
    {
        GameObject plane = new GameObject();

        if (!string.IsNullOrEmpty(optionalName))
            plane.name = optionalName;
        else
            plane.name = "UniPolyWaterPlane";

        if (!createAtOrigin && cam)
            plane.transform.position = cam.transform.position + cam.transform.forward * 5.0f;
        else
            plane.transform.position = Vector3.zero;

        Vector2 anchorOffset;
        string anchorId;
        switch (anchor)
        {
            case AnchorPoint.TopLeft:
                anchorOffset = new Vector2(-width / 2.0f, length / 2.0f);
                anchorId = "TL";
                break;
            case AnchorPoint.TopHalf:
                anchorOffset = new Vector2(0.0f, length / 2.0f);
                anchorId = "TH";
                break;
            case AnchorPoint.TopRight:
                anchorOffset = new Vector2(width / 2.0f, length / 2.0f);
                anchorId = "TR";
                break;
            case AnchorPoint.RightHalf:
                anchorOffset = new Vector2(width / 2.0f, 0.0f);
                anchorId = "RH";
                break;
            case AnchorPoint.BottomRight:
                anchorOffset = new Vector2(width / 2.0f, -length / 2.0f);
                anchorId = "BR";
                break;
            case AnchorPoint.BottomHalf:
                anchorOffset = new Vector2(0.0f, -length / 2.0f);
                anchorId = "BH";
                break;
            case AnchorPoint.BottomLeft:
                anchorOffset = new Vector2(-width / 2.0f, -length / 2.0f);
                anchorId = "BL";
                break;
            case AnchorPoint.LeftHalf:
                anchorOffset = new Vector2(-width / 2.0f, 0.0f);
                anchorId = "LH";
                break;
            case AnchorPoint.Center:
            default:
                anchorOffset = Vector2.zero;
                anchorId = "C";
                break;
        }

        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        plane.AddComponent(typeof(MeshRenderer));

        string planeAssetName = plane.name + widthSegments + "x" + lengthSegments + anchorId + ".asset";
        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + planeAssetName, typeof(Mesh));

        if (m == null)
        {
            m = new Mesh();
            m.name = plane.name;

            int hCount = widthSegments + 1;
            int vCount = lengthSegments + 1;
            int numTriangles = widthSegments * lengthSegments * 6;
            int numVertices = hCount * vCount;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numTriangles];

            int index = 0;
            float scaleX = 1.0f;
            float scaleY = 1.0f;
            for (float y = 0.0f; y < vCount; y++)
            {
                for (float x = 0.0f; x < hCount; x++)
                {
                    vertices[index++] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, 0.0f, y * scaleY - length / 2f - anchorOffset.y);
                }
            }

            index = 0;
            for (int y = 0; y < lengthSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index] = (y * hCount) + x;
                    triangles[index + 1] = ((y + 1) * hCount) + x;
                    triangles[index + 2] = (y * hCount) + x + 1;

                    triangles[index + 3] = ((y + 1) * hCount) + x;
                    triangles[index + 4] = ((y + 1) * hCount) + x + 1;
                    triangles[index + 5] = (y * hCount) + x + 1;
                    index += 6;
                }
            }

            Vector3[] reVertices = new Vector3[numTriangles];
            Vector4[] tangents = new Vector4[numTriangles];
            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
            Vector2[] uvs1 = new Vector2[numTriangles];
            Vector2[] uvs = new Vector2[numTriangles];

            for (int i = 0; i < triangles.Length;)
            {
                reVertices[i] = vertices[triangles[i]];
                reVertices[i + 1] = vertices[triangles[i + 1]];
                reVertices[i + 2] = vertices[triangles[i + 2]];
				reVertices[i + 3] = vertices[triangles[i + 3]];
                reVertices[i + 4] = vertices[triangles[i + 4]];
                reVertices[i + 5] = vertices[triangles[i + 5]];

                triangles[i] = i;
                triangles[i + 1] = i + 1;
                triangles[i + 2] = i + 2;
				triangles[i + 3] = i + 3;
				triangles[i + 4] = i + 4;
				triangles[i + 5] = i + 5;
                
                tangents[i] = tangent;
                tangents[i + 1] = tangent;
                tangents[i + 2] = tangent;
				tangents[i + 3] = tangent;
				tangents[i + 4] = tangent;
				tangents[i + 5] = tangent;

				uvs[i] = new Vector2((reVertices[i].x + (width / 2f)) / width, (reVertices[i].z + (length / 2f)) / length);
                uvs1[i] = new Vector2(0, 0);
                            
				uvs[i + 1] = new Vector2((reVertices[i + 1].x + (width / 2f)) / width, (reVertices[i + 1].z + (length / 2f)) / length);
                uvs1[i + 1] = new Vector2(1, 0);
                            
				uvs[i + 2] = new Vector2((reVertices[i + 2].x + (width / 2f)) / width, (reVertices[i + 2].z + (length / 2f)) / length);
                uvs1[i + 2] = new Vector2(2, 0);
                
                uvs[i + 3] = new Vector2((reVertices[i + 3].x + (width / 2f)) / width, (reVertices[i + 3].z + (length / 2f)) / length);
                uvs1[i + 3] = new Vector2(3, 0);

                uvs[i + 4] = new Vector2((reVertices[i + 4].x + (width / 2f)) / width, (reVertices[i + 4].z + (length / 2f)) / length);
                uvs1[i + 4] = new Vector2(4, 0);

                uvs[i + 5] = new Vector2((reVertices[i + 5].x + (width / 2f)) / width, (reVertices[i + 5].z + (length / 2f)) / length);
                uvs1[i + 5] = new Vector2(5, 0);
                            
                i += 6;
            }

            m.vertices = reVertices;

            m.uv = uvs;
            m.uv3 = uvs1;

            m.triangles = triangles;
            m.tangents = tangents;
            m.RecalculateNormals();

            AssetDatabase.CreateAsset(m, "Assets/UniPolyWater/Models/" + planeAssetName);
            AssetDatabase.SaveAssets();
        }

        meshFilter.sharedMesh = m;
        m.RecalculateBounds();

        if (addCollider)
            plane.AddComponent(typeof(BoxCollider));

        Selection.activeObject = plane;
    }
}
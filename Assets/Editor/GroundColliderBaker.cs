using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class GroundColliderBaker
    {
        [MenuItem("Tools/Bake Ground Collision Mesh")]
        static void BakeGroundCollision()
        {
            GameObject groundParent = GameObject.Find("Ground Tiles");
            MeshFilter[] filters = groundParent.GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[filters.Length];
            for(int i = 0; i < filters.Length; i++)
            {
                combine[i].mesh = filters[i].sharedMesh;
                combine[i].transform = filters[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combine, true, true);

            AssetDatabase.CreateAsset(combinedMesh, "Assets/Physics/GroundCollisionMesh.asset");
            AssetDatabase.SaveAssets();

            GameObject colliderObj = new GameObject("Ground_Collision");
            colliderObj.isStatic = true;

            MeshCollider mc = colliderObj.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
            mc.convex = false;

            Debug.Log("Ground collision baked: " + filters.Length + " tiles combined.");
        }
    }
}

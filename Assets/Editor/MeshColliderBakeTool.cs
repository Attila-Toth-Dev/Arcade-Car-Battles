using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Editor
{
    public class MeshColliderBakeTool
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
            combinedMesh.indexFormat = IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combine, true, true);

            AssetDatabase.CreateAsset(combinedMesh, "Assets/Art/Models/GroundCollisionMesh.asset");
            AssetDatabase.SaveAssets();

            GameObject colliderObj = new GameObject("Ground_Collision");
            colliderObj.isStatic = true;

            MeshCollider mc = colliderObj.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
            mc.convex = false;

            Debug.Log("Ground collision baked: " + filters.Length + " tiles combined.");
        }

        [MenuItem("Tools/Bake Building Collision Mesh")]
        static void BakeBuildingCollision()
        {
            GameObject buildingParent = GameObject.Find("Buildings");
            MeshFilter[] filters = buildingParent.GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[filters.Length];
            for(int i = 0; i < filters.Length; i++)
            {
                combine[i].mesh = filters[i].sharedMesh;
                combine[i].transform = filters[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.indexFormat = IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combine, true, true);

            AssetDatabase.CreateAsset(combinedMesh, "Assets/Art/Models/BuildingCollisionMesh.asset");
            AssetDatabase.SaveAssets();

            GameObject colliderObj = new GameObject("Building_Collision");
            colliderObj.isStatic = true;

            MeshCollider mc = colliderObj.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
            mc.convex = false;
            
            Debug.Log("Building collision baked: " + filters.Length + " buildings combined.");
        }
    }
}

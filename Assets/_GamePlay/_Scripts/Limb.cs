using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RaycastPro;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Limb : MonoBehaviour, IHitable
{
    public SkinnedMeshRenderer referenceSkin;
    
    [HideInInspector] public Giant_Core giantCore;

    public Vector3 rotationOffset;

    public bool isLeg = false;
    
    public float health = 100f;
    public float maxHealth = 100f;
    public float explosionForce = 150f;
    public float explosionRadius = 15f;
    
    public LayerMask bakeLayer;
    public float scaleFactor = 1f;
    public float rigidBodyMass = 12f;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            health = Mathf.Clamp(health, 0f, maxHealth);

            if (health <= 0)
            {
                Dismember();
            }
        }
    }
    /// <summary>
    /// For parts like foot and head
    /// </summary>
    public bool giantFinisher;
    
    /// <summary>
    /// This will stop run dismember Method
    /// </summary>
    public bool unbreakable;
    public Transform boneRoot;

    void Start()
    {
        health = maxHealth;
    }
    
    

    public void TakeDamage(float amount)
    {
        // if (UserManager.Instance.Data.Level == 1)
        // {
        //     amount = 1f;
        // }
        Health -= amount;
        giantCore.TakeDamage(amount);
    }

    private Transform dismember;
    [Button("Dismember")]
    public void Dismember(bool instantiateParticle = true, bool instantiateMember = true)
    {
        if (unbreakable) return;

        if (isLeg && giantCore.currentLegCount <= 1) return;
        
        if(isLeg)
            giantCore.currentLegCount--;
        
        if (instantiateMember) // Part instantiate
        {
            if (referenceSkin)
            {
                BakeMesh();
            }
            else
            {
                dismember = Instantiate(transform, transform);
                dismember.gameObject.layer = LayerMask.NameToLayer("Ragdolls");
                dismember.localScale = Vector3.one;
                dismember.localEulerAngles = rotationOffset;
                dismember.localPosition = Vector3.zero;
                dismember.parent = null;
                var rb = dismember.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = dismember.AddComponent<Rigidbody>();
                }
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                rb.mass = rigidBodyMass;
                //Destroy(dismember.gameObject, 10f);
                GameManager.Instance.Monster.detachedLimbs.Add(dismember.gameObject);
            }
        }
        
        if (boneRoot)
        {
            boneRoot.localScale = Vector3.zero;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }

        gameObject.SetActive(false);
        if (instantiateParticle && LimbManager.DismemberEffect)
        {
            LimbManager.ExplodeEffect(dismember);
        }

        
        // if (giantFinisher)
        // {
        //     giantCore.SetHealth(0);
        // }

        unbreakable = true;
    }
    

    public void OnHit(CarCore core, float damage)
    {
        TakeDamage(damage);
    }
    
    private List<int> boneIndex = new List<int>();

    private Rigidbody rb;

    [Button("Bake Mesh")]
    public void BakeMesh()
    {
        boneIndex.Clear();

        foreach (var child in transform.GetAllChildren())
        {
            boneIndex.Add(System.Array.IndexOf(referenceSkin.bones, child));
        }

        var originalMesh = referenceSkin.sharedMesh;
        
        // Create a new mesh
        var bakedMesh = new Mesh
        {
            // Copy necessary data from the original mesh
            vertices = originalMesh.vertices,
            normals = originalMesh.normals,
            uv = originalMesh.uv,
            triangles = originalMesh.triangles
        };
        
        referenceSkin.BakeMesh(bakedMesh, false);
        
        // Iterate through the vertices and check if each vertex is influenced by the specific bone
        var boneWeights = originalMesh.boneWeights;
        var vertices = bakedMesh.vertices;
        var boneIndices = boneWeights.Select(bw => bw.boneIndex0).ToArray();

        var root = boneRoot ? boneRoot : referenceSkin.transform;
        var offset = root.transform.InverseTransformDirection(transform.position - root.position);
        offset.x /= scaleFactor;

        
        for (int i = 0; i < vertices.Length; i++)
        {
            if (boneIndex.Contains(boneIndices[i]))
            {
                vertices[i] -= offset;
            }
            else
            {
                vertices[i] = Vector3.zero;
            }
        }

        var rO = Quaternion.Euler(rotationOffset);
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rO * vertices[i];
        }
        
        bakedMesh.vertices = vertices;
        
        bakedMesh.Optimize();
        bakedMesh.OptimizeIndexBuffers();
        bakedMesh.OptimizeReorderVertexBuffer();

        bakedMesh.RecalculateBounds();
        bakedMesh.RecalculateNormals();


        dismember = new GameObject($"{name} Limb").transform;

//        createdMesh.layer = LayerMask.NameToLayer("Ragdolls");
        
        dismember.position = transform.position;
        //createdMesh.transform.rotation = transform.rotation;
        dismember.localScale = Vector3.one * scaleFactor;
        
        dismember.AddComponent<MeshRenderer>();
        
        var targetMeshFilter = dismember.AddComponent<MeshFilter>();
        targetMeshFilter.sharedMesh = bakedMesh;
        targetMeshFilter.sharedMesh.vertices = bakedMesh.vertices;
        targetMeshFilter.GetComponent<MeshRenderer>().sharedMaterials = referenceSkin.sharedMaterials;
        
        var col = dismember.AddComponent<MeshCollider>();
        col.convex = true;

        rb = dismember.AddComponent<Rigidbody>();
        if (explosionForce > 0)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
        rb.mass = rigidBodyMass;
        //Destroy(dismember.gameObject, 10f);
        GameManager.Instance.Monster.detachedLimbs.Add(dismember.gameObject);
    }
}

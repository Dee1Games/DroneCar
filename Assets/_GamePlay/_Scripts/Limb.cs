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
    private LimbSmoke smoke;

    public bool IsHead = false;
    
    public SkinnedMeshRenderer referenceSkin;
    
    [HideInInspector] public Giant_Core giantCore;

    public Vector3 rotationOffset;

    public bool isLeg = false;
    
    private float _h;
    public float maxHealth = 100f;
    public float explosionForce = 150f;
    public float explosionRadius = 15f;
    
    public LayerMask bakeLayer;
    public float scaleFactor = 1f;
    public float rigidBodyMass = 12f;

    public float Health
    {
        get => _h;
        set
        {
            _h = value;
            _h = Mathf.Clamp(_h, 0f, maxHealth);

            if (_h <= 0)
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

    public void Reset()
    {
        _h = maxHealth;
    }
    
    

    public float TakeDamage(Vector3 pos, float amount, bool isCar = false)
    {
        // if (UserManager.Instance.Data.Level == 1)
        // {
        //     amount = 1f;
        // }
        if (IsHead && isCar)
        {
            amount = (Health) + 1;
        }
        
        Health -= amount;
        giantCore.TakeDamage(pos, amount);
        
        bool smokeChanged = false;
        float healthPercent = Health / maxHealth;
        int damageLevel = 0;
        if (healthPercent > 0.66f)
        {
            damageLevel = 1;
        } else if (healthPercent > 0.33f)
        {
            damageLevel = 2;
        } else
        {
            damageLevel = 3;
        }

        if (smoke == null || damageLevel != smoke.level)
        {
            LimbSmoke newSmoke = LimbSmokePool.Instance.Spawn(transform.position,  transform.up, damageLevel);
            if (newSmoke != null)
            {
                if (smoke != null)
                {
                    smoke.ReturnToPool();
                }
                newSmoke.transform.parent = transform.parent;
                smoke = newSmoke;
            }
        }

        return amount;
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
                Target t = dismember.transform.GetComponentInChildren<Target>();
                if (t != null)
                {
                    t.enabled = false;
                }
                dismember.gameObject.layer = LayerMask.NameToLayer("Ragdolls");
                dismember.localScale = Vector3.one;
                dismember.localEulerAngles = rotationOffset;
                dismember.localPosition = Vector3.zero;
                dismember.parent = null;
                Transform fire = dismember.transform.Find("Godzilla_Fire");
                if (fire != null)
                {
                    fire.gameObject.SetActive(false);
                }
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
    

    public float OnHit(CarCore core, Vector3 pos, float damage, bool isCar)
    {
        return TakeDamage(pos, damage, isCar);
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
        
        Target t = dismember.transform.GetComponentInChildren<Target>();
        if (t != null)
        {
            t.enabled = false;
        }

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
        
        Transform fire = dismember.transform.Find("Godzilla_Fire");
        if (fire != null)
        {
            fire.gameObject.SetActive(false);
        }

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

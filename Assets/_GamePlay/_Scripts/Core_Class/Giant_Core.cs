using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

public class Giant_Core : MonoBehaviour, IHitable
{
    public Monster monster;
    public int legCount;
    public int currentLegCount;
    protected AI_Core aiCore;
    protected Animator animator;
    public FullBodyBipedIK fullBodyBipedIK;

    [Title("Options", titleAlignment: TitleAlignments.Centered)]


    [Title("UI")]
    public Sprite giantIcon;
    
    [FoldoutGroup("Ragdoll Setup")]
    public bool hasRagdoll;
    [FoldoutGroup("Ragdoll Setup")]
    public List<Rigidbody> ragdollParts;
    [FoldoutGroup("Ragdoll Setup")]
    public Limb[] limbs;
    [FoldoutGroup("Ragdoll Setup")]
    public float mass = 15;
    [FoldoutGroup("Ragdoll Setup")]
    public float drag = 3;
    [FoldoutGroup("Ragdoll Setup")]
    public float angularDrag = 3;

    private void Awake()
    {
        currentLegCount = legCount;
        aiCore = GetComponent<AI_Core>();
        monster = GetComponentInParent<Monster>();
        animator = GetComponentInChildren<Animator>();
        fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        
        // Get All Ragdolls via ignore self;
        ragdollParts = GetComponentsInChildren<Rigidbody>().ToList();

        limbs = GetComponentsInChildren<Limb>();
        foreach (var limb in limbs)
        {
            limb.giantCore = this;
        }

        if (UI_Core._)
        {
            UI_Core._.giantIcon.sprite = giantIcon;
        }
    }

    public Transform GetRandomMember()
    {
        var random = Random.Range(0, 5);

        switch (random)
        {
            case 0: return fullBodyBipedIK.references.head;
            case 1: return fullBodyBipedIK.references.leftUpperArm;
            case 2: return fullBodyBipedIK.references.rightUpperArm;
            case 3: return fullBodyBipedIK.references.leftThigh;
            case 4: return fullBodyBipedIK.references.rightThigh;
        }
        return fullBodyBipedIK.references.head;
    }

    [FoldoutGroup("Ragdoll Setup")]
    [Button("Set Ragdoll")]
    private void SetRagdoll()
    {
        ragdollParts = GetComponentsInChildren<Rigidbody>().ToList();
        foreach (var ragdollPart in ragdollParts)
        {
            ragdollPart.mass = mass;
            ragdollPart.drag = drag;
            ragdollPart.angularDrag = angularDrag;
        }
    }
    [Button("Remove Ragdoll")]
    private void RemoveRagdoll()
    {
        for (var index = 0; index < ragdollParts.Count; index++)
        {
            if (!ragdollParts[index]) continue;

            if (ragdollParts[index].TryGetComponent(out Joint joint))
            {
                DestroyImmediate(joint);
            }
            if (ragdollParts[index].TryGetComponent(out Collider col))
            {
                DestroyImmediate(col);
            }
            DestroyImmediate(ragdollParts[index]);
        }
    }
    [Button("Remove Mesh Colliders")]
    [FoldoutGroup("Ragdoll Setup")]
    public void RemoveMeshColliders()
    {
        var children = GetComponentsInChildren<MeshCollider>();
        for (var index = 0; index < children.Length; index++)
        {
            var child = children[index];
            DestroyImmediate(child);
        }
    }
    [Button("Find Limbs")]
    [FoldoutGroup("Ragdoll Setup")]
    private void RefreshLimbs()
    {
        limbs = GetComponentsInChildren<Limb>();
        var skin =  GetComponentInChildren<SkinnedMeshRenderer>();
        for (var i = 0; i < limbs.Length; i++)
        {
            limbs[i].referenceSkin = skin;
        }
    }


    private static readonly int Die = Animator.StringToHash("die");
    
    private bool isDead;

    public bool IsDead => isDead;
    
    /// <summary>
    /// False = die
    /// </summary>
    /// <param name="phase"></param>
    public void OnDie()
    {
        if (isDead)
            return;
        
        isDead = true;
        aiCore.Active(false);

        if (hasRagdoll)
        {
            animator.enabled = false;
            if (fullBodyBipedIK) fullBodyBipedIK.enabled = false;
            foreach (var ragdollPart in ragdollParts)
            {
                if (ragdollPart)
                {
                    ragdollPart.isKinematic = false;
                    ragdollPart.useGravity = true;
                }
            }
        }
        else
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
            animator.SetTrigger(Die);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;
        
        if (GameManager.Instance.Player == null || GameManager.Instance.Player.Core.hitedMonster)
        {
            return;
        }
        
        GameManager.Instance.CurrentRunDamage += damage;
            

        monster.Health -= damage;
        
        aiCore.OnPlayerFound(CarCore._);
        
        if (monster.Health <= 0)
        {
            OnDie();
            if (GameManager.Instance.Player.IsActive)
            {
                GameManager.Instance.Player.Core.End(false, false);
            }
            UserManager.Instance.NextLevel();
            GameManager.Instance.Player.Core.CameraZoomOut();
            //UIManager.Instance.ShowScreen(UIScreenID.EndLevel);
        }

    }

    public void SetHealth(float amount)
    {
        monster.Health = amount;
        /*if (amount <= 0)
        {
            OnDie();
        }*/
    }
    public void OnHit(CarCore core, float damage) => TakeDamage(damage);
}
public static class CoreExtension
{
    public static Transform[] GetAllChildren(this Transform parent)
    {
        var children = new List<Transform>();
        
        children.Add(parent);
        
        GetAllChildrenRecursive(parent, children);

        return children.ToArray();
    }

    private static void GetAllChildrenRecursive(Transform parent, List<Transform> childrenList)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            childrenList.Add(child);
            GetAllChildrenRecursive(child, childrenList);
        }
    }
}
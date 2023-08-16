using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance;
    
    [SerializeField] private List<ProjectileMoveScript> projectilePrefabs;
    [SerializeField] private int poolInitCount;

    private List<Queue<ProjectileMoveScript>> projectilePools;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        projectilePools = new List<Queue<ProjectileMoveScript>>();

        int n = 0;
        foreach (ProjectileMoveScript prefab in projectilePrefabs)
        {
            Queue<ProjectileMoveScript> objectPool = new Queue<ProjectileMoveScript>();

            for (int i = 0; i < poolInitCount; i++)
            {
                ProjectileMoveScript projectile = InstantiateProjectile(n);
                projectile.gameObject.SetActive(false);
                objectPool.Enqueue(projectile);
            }

            projectilePools.Add(objectPool);
            n++;
        }
    }

    private ProjectileMoveScript InstantiateProjectile(int n)
    {
        ProjectileMoveScript projectile = Instantiate(projectilePrefabs[n]).GetComponent<ProjectileMoveScript>();
        projectile.name = n.ToString();
        projectile.transform.parent = transform;
        return projectile;
    }

    public ProjectileMoveScript GetProjectile(int n)
    {
        if (projectilePools[n].Count == 0)
        {
            ProjectileMoveScript newProjectile = InstantiateProjectile(n);
            newProjectile.gameObject.SetActive(false);
            projectilePools[n].Enqueue(newProjectile);
        }
        
        ProjectileMoveScript projectile = projectilePools[n].Dequeue();
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void ReturnProjectile(ProjectileMoveScript projectile)
    {
        projectile.gameObject.SetActive(false);
        projectilePools[int.Parse( projectile.name)].Enqueue( projectile);
    }
}

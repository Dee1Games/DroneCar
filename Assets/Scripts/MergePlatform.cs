using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergePlatform : MonoBehaviour
{
    public static MergePlatform Instance;
    
    [SerializeField] private Camera camera;
    [SerializeField] private MergeCell[] cells;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveOffset;

    private MergeItem currentSelectedItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (MergeCell cell in cells)
        {
            cell.Init();
        }

        currentSelectedItem = null;
    }

    private void Update()
    {
        if (currentSelectedItem != null)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition + moveOffset);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 999f, LayerMask.GetMask("MergePlatform")))
                {
                    Vector3 point = hit.point;
                    currentSelectedItem.transform.position = point;
                }
            }
            else
            {
                MergeCell closestCell = GetClosestCell(currentSelectedItem.transform.position);
                if (closestCell == null)
                    closestCell = currentSelectedItem.CurrentCell;
                closestCell.SetItem(currentSelectedItem);
                currentSelectedItem.MoveToCell(closestCell, moveSpeed);
                currentSelectedItem = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnItemInCell(GetRandomEmptyCell());
        }
    }

    public void ItemSelected(MergeItem item)
    {
        currentSelectedItem = item;
    }

    private void SpawnItemInCell(MergeCell cell)
    {
        MergeItem item = Instantiate(itemPrefab, cell.transform).GetComponent<MergeItem>();
        cell.SetItem(item);
    }

    private MergeCell GetRandomEmptyCell()
    {
        List<MergeCell> emptyCells = new List<MergeCell>();
        foreach (MergeCell cell in cells)
        {
            if(!cell.IsFull)
                emptyCells.Add(cell);
        }
        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    private MergeCell GetClosestCell(Vector3 pos)
    {
        Collider[] nearCellColliders = Physics.OverlapSphere(pos, 1f, LayerMask.GetMask("MergeCell"));
            
        float minDistance = 9999f;
        MergeCell closestCell = null;

        foreach (Collider cellCollider in nearCellColliders)
        {
            MergeCell cell = cellCollider.GetComponent<MergeCell>();
            if(cell!=null && cell.IsFull)
                continue;
            
            pos.y = cell.transform.position.y;
            float distance = Vector3.Distance(pos, cell.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCell = cell;
            }
        }

        return closestCell;
    }
}

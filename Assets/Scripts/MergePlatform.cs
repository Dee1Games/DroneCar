using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergePlatform : MonoBehaviour
{
    public static MergePlatform Instance;
    
    [SerializeField] private Camera camera;
    [SerializeField] private MergeCell[] cells;
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
                MergeCell closestCell = GetClosestCell(currentSelectedItem);
                if (closestCell == null)
                    closestCell = currentSelectedItem.CurrentCell;
                MergeItem currentItem = currentSelectedItem;
                currentSelectedItem.MoveToCell(closestCell, moveSpeed, () =>
                {
                    closestCell.SetItem(currentItem);
                });
                currentSelectedItem = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnInRandomCell();
        }
    }

    public void ItemSelected(MergeItem item)
    {
        currentSelectedItem = item;
    }

    private void SpawnInRandomCell()
    {
        SpawnItemInCell(GetRandomEmptyCell());
    }

    private void SpawnItemInCell(MergeCell cell)
    {
        Item randomItem = GetRandomItem();
        MergeItem item = Instantiate(randomItem.MergePrefab, cell.transform).GetComponent<MergeItem>();
        item.Init(randomItem.Level, randomItem.Type);
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
    
    private Item GetRandomItem()
    {
        List<UpgradeType> probabilities = new List<UpgradeType>();
        foreach(UpgradeType upgradeType in UpgradeType.GetValues(typeof(UpgradeType)))
        {
            int p = PlayerVehicle.Instance.Config.GetProbability(upgradeType);
            for (int i = 0; i < p; i++)
            {
                probabilities.Add(upgradeType);
            }
        }

        UpgradeType randomType = probabilities[Random.Range(0, probabilities.Count)];
        return PlayerVehicle.Instance.Config.GetItem(randomType, 1);
    }

    private MergeCell GetClosestCell(MergeItem item)
    {
        Vector3 pos = item.transform.position;
        Collider[] nearCellColliders = Physics.OverlapSphere(pos, 1f, LayerMask.GetMask("MergeCell"));
            
        float minDistance = 9999f;
        MergeCell closestCell = null;

        foreach (Collider cellCollider in nearCellColliders)
        {
            MergeCell cell = cellCollider.GetComponent<MergeCell>();
            if (cell==null)
                continue;
            if (cell.IsFull)
                if (cell.Item.Type!=item.Type || cell.Item.Level!=item.Level)
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

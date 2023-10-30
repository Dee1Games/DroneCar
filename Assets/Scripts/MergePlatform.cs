using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergePlatform : MonoBehaviour
{
    public static MergePlatform Instance;
    
    [SerializeField] private Camera camera;
    [SerializeField] private Transform content;
    [SerializeField] private MergeCell[] cells;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveOffset;
    [SerializeField] private Transform vehiclePos;
    [SerializeField] private float mergeToVehicleDistance;
    [SerializeField] private int firstUpgradePrice;
    [SerializeField] private float upgradePriceMultiplier;
    [SerializeField] private ParticleSystem particle;
    public Transform CameraTransform => camera.transform;

    private MergeItem currentSelectedItem;

    private bool sellUiActive;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Init()
    {
        foreach (MergeCell cell in cells)
        {
            cell.Init();
            UpgradeLevel upgradeLevel = UserManager.Instance.GetMergePlatformCell(cell.Index);
            if (upgradeLevel.Level != -1)
            {
                Item i = GameManager.Instance.Player.Config.GetItem(upgradeLevel.Type, upgradeLevel.Level);
                MergeItem item = Instantiate(i.MergePrefab, cell.transform).GetComponent<MergeItem>();
                item.Init(i.Level, i.Type);
                cell.SetItem(item);
            }
        }

        currentSelectedItem = null;
        GameManager.Instance.Player.transform.position = vehiclePos.position;
        GameManager.Instance.Player.transform.rotation = vehiclePos.rotation;

        sellUiActive = true;
        HideSellUI();
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
                bool merged = false;
                if (Vector3.Distance(GameManager.Instance.Player.transform.position, currentSelectedItem.transform.position) < mergeToVehicleDistance)
                {
                    if (GameManager.Instance.Player.SetUpgrade(currentSelectedItem.Type, currentSelectedItem.Level))
                    {
                        if (UserManager.Instance.Data.SeenMergeTutorial || (!UserManager.Instance.Data.SeenMergeTutorial && NumberOfFullCells()==0))
                        {
                            merged = true;
                            GameManager.Instance.Player.ShowUpgradeVisuals();
                            MergeItem currentItem = currentSelectedItem;
                            currentSelectedItem.MoveToPlayerVehicle(moveSpeed, () =>
                            {
                                PlayParticle();
                                Destroy(currentItem.gameObject);
                                if (!UserManager.Instance.Data.SeenMergeTutorial)
                                {
                                    TutorialManager.Instance.ShowPlayHint();
                                    UserManager.Instance.SeenMergeTutorial();
                                    UIManager.Instance.Refresh();
                                }
                            });
                        }
                    }
                }

                if (!merged)
                {
                    MergeCell closestCell = GetClosestCell(currentSelectedItem);
                    if (closestCell == null)
                        closestCell = currentSelectedItem.CurrentCell;
                    MergeItem currentItem = currentSelectedItem;
                    currentSelectedItem.MoveToCell(closestCell, moveSpeed, () =>
                    {
                        closestCell.SetItem(currentItem);
                        if (!UserManager.Instance.Data.SeenMergeTutorial && closestCell != currentItem.CurrentCell)
                        {
                            TutorialManager.Instance.ShowAssembleHint();
                            TutorialManager.Instance.ShowHand(camera.WorldToScreenPoint(closestCell.transform.position), camera.WorldToScreenPoint(GameManager.Instance.Player.transform.position));
                        }
                    });
                }
                
                currentSelectedItem = null;
            }
        }

        if (NumberOfFullCells() == 6)
        {
            ShowSellUI();
        }
        else
        {
            HideSellUI();
        }
    }

    public void ItemSelected(MergeItem item)
    {
        currentSelectedItem = item;
    }

    public void SpawnUpgrade()
    {
        if (NumberOfFullCells() == 6)
            return;


        int price = GetCurrentUpgradePrice();
        if(price>UserManager.Instance.Data.Coins)
            return;
        
        UserManager.Instance.Data.Coins -= firstUpgradePrice;
        
        SpawnItemInCell(GetRandomEmptyCell());
        
        UserManager.Instance.NextUpgrade();
        UIManager.Instance.Refresh();
        
        if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount==3)
        {
            TutorialManager.Instance.ShowMergeHint();
            TutorialManager.Instance.ShowHand(camera.WorldToScreenPoint(cells[0].transform.position), camera.WorldToScreenPoint(cells[1].transform.position));
        }
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
        if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount == 1)
        {
            return cells[0];
        } 
        if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount == 2)
        {
            return cells[1];
        } 
        
        List<MergeCell> emptyCells = new List<MergeCell>();
        foreach (MergeCell cell in cells)
        {
            if(!cell.IsFull)
                emptyCells.Add(cell);
        }
        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    public int NumberOfFullCells()
    {
        int n = 0;
        foreach (MergeCell cell in cells)
        {
            if (cell.IsFull)
                n++;
        }

        return n;
    }
    
    private Item GetRandomItem()
    {
        if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount == 1)
        {
            return GameManager.Instance.Player.Config.GetItem(UpgradeType.Gun, 1);
        } 
        if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount == 2)
        {
            return GameManager.Instance.Player.Config.GetItem(UpgradeType.Gun, 1);
        } 
        
        List<UpgradeType> probabilities = new List<UpgradeType>();
        foreach(UpgradeType upgradeType in UpgradeType.GetValues(typeof(UpgradeType)))
        {
            int p = GameManager.Instance.Player.Config.GetProbability(upgradeType);
            if (GameManager.Instance.Player.Config.GetItem(upgradeType,
                UserManager.Instance.GetUpgradeLevel(LevelManager.Instance.CurrentLevelData.Vehicle, upgradeType)+1) == null)
            {
                p = 0;
            } 
            for (int i = 0; i < p; i++)
            {
                probabilities.Add(upgradeType);
            }
        }

        if (NumberOfFullCells() == 5)
        {
            UpgradeType t = probabilities[0];
            foreach (MergeCell cell in cells)
            {
                if (cell.Item!=null && cell.Item.Level == 1)
                    t = cell.Item.Type;
            }

            return GameManager.Instance.Player.Config.GetItem(t, 1);
        }
        else
        {
            UpgradeType randomType = probabilities[Random.Range(0, probabilities.Count)];
            return GameManager.Instance.Player.Config.GetItem(randomType, 1);
        }
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
            {
                if (cell.Item.Type != item.Type || cell.Item.Level != item.Level)
                    continue;
                else if (GameManager.Instance.Player.Config.GetItem(item.Type, item.Level + 1) == null)
                    continue;
            }

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

    public void ClearPlatform()
    {
        for (int i=0; i<cells.Length ; i++)
        {
            UserManager.Instance.SetMergePlatformCell(i, UpgradeType.Tire, -1);
        }
        Init();
    }

    public void Show()
    {
        content.gameObject.SetActive(true);
    }

    public void Hide()
    {
        content.gameObject.SetActive(false);
    }

    public int GetCurrentUpgradePrice()
    {
        return GetUpgradePrice(UserManager.Instance.Data.UpgradeCount);
    }

    public void PlayParticle()
    {
        particle.Play();
    }

    public void ShowSellUI()
    {
        if (sellUiActive)
            return;
        foreach (MergeCell cell in cells)
        {
            cell.ShowSellUI();
        }
        sellUiActive = true;
    }
    
    public void HideSellUI()
    {
        if (!sellUiActive)
            return;
        foreach (MergeCell cell in cells)
        {
            cell.HideSellUI();
        }

        sellUiActive = false;
    }

    private int GetUpgradePrice(int n)
    {
        if (n <= 1)
        {
            return 23;
        }
        else
        {
            return Mathf.FloorToInt((24.97f * (n - 1)) + 22.64f + GetUpgradePrice(n-1));
        }
    }
}

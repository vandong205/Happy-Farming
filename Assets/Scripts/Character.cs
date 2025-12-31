using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Character : MonoBehaviour
{
    
    public enum CharacterState
    {
        Free,
        Busy,
    }
    public enum ActionMode
    {
        None,
        UseTool,
        UseItem
    }
    #region Fields
    [SerializeField] InputActionAsset IAAsset;
    [SerializeField] Camera followCamera;
    [SerializeField]  float stopDistance = 0.05f;
    [SerializeField] private float moveSpeed;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer holdingItemSlot;

    private bool IsMoving = false;
    private InputAction moveAction;
    private Queue<Vector3Int> cellsActionQueue  =  new();
    private ActionMode currentAction;
    private Coroutine moveRoutine;
    private ToolNames selectedTool;
    public CharacterState state { get; private set; }
    public float speed {  get; private set; }
    public int holdingItemId { get; private set; }
    #endregion
    #region Private Method
    private void Awake()
    {
        if(animator==null) animator = GetComponent<Animator>();
        if (holdingItemSlot == null) GetComponentInChildren<SpriteRenderer>();
        moveAction = IAAsset
            .FindActionMap("Character")
            .FindAction("Move");
        moveAction.performed += Move;
        SelectTool(ToolNames.Scythe);
    }
    void HoldItem(int itemId)
    {
        if(state!=CharacterState.Free)
        {
            NotificationManager.Instance.ShowPopUpNotify("Nhân vật đang bận !", NotifyType.Warning);
            return;
        }
        animator.SetBool("IsHoldingItem", true);
        holdingItemId = itemId;
        ItemData item  =GameDatabase.Instance.ItemDB.GetItem(holdingItemId);
        if (item != null) {
            holdingItemSlot.sprite = item.sprite;
        }
        currentAction = ActionMode.UseItem;
    }

    void SelectTool(ToolNames tool)
    {
        selectedTool = tool;
        currentAction = ActionMode.UseTool;
    }
    void CancelAction()
    {
        cellsActionQueue.Clear();
        currentAction = ActionMode.None;
        state = CharacterState.Free;
    }
    public void SetFree()
    {
        state = CharacterState.Free;
    }
    private void OnEnable()
    {
        IAAsset.FindActionMap("Character").Enable();
    }
    private void OnDestroy()
    {
        moveAction.performed -= Move;
    }
    private void Move(InputAction.CallbackContext ctx)
    {
        if (state != CharacterState.Free) return;
        cellsActionQueue.Clear();
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = followCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        Vector3Int cellPos = WorldManager.Instance.WorldPosToCellPos(mouseWorldPos);
        Debug.Log($"Clicked grid Cell: {cellPos}");
            MoveToCell(cellPos);

    }
    private void MoveToCell(Vector3Int targetCell)
    {
        IsMoving = true;
        animator.SetBool("IsMoving",IsMoving);
        Vector3Int currentCell =
            WorldManager.Instance.WorldPosToCellPos(transform.position);

        List<Vector3Int> path =
            WorldManager.Instance.FindPath(currentCell, targetCell);
        if (path == null || path.Count == 0)
        {
            Debug.Log("Khong tim thay duong di");
            return;
        }
        // Chi them hanh dong neu den duoc cell do 
        if (path[path.Count - 1] == targetCell) cellsActionQueue.Enqueue(targetCell);
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveAlongPath(path));
    }
    private IEnumerator MoveAlongPath(List<Vector3Int> path)
    {

        // Bỏ cell đầu (cell đang đứng)
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 targetPos =
                WorldManager.Instance.CellPosToWorldCenter(path[i]);
            SetMoveDirection(transform.position, targetPos);
            while (Vector3.Distance(transform.position, targetPos) > stopDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            // snap cho chắc
            transform.position = targetPos;
        }
        IsMoving = false;
        animator.SetBool("IsMoving", IsMoving);
        if (TryToDoAction()) DoAction();

    }
    private bool TryToDoAction()
    {
        if (cellsActionQueue.Count == 0) return false;

        Vector3Int cell = cellsActionQueue.Peek();
        Vector2 cellPos = new Vector2(cell.x, cell.y);

        int tileId = WorldManager.Instance.HasObjectOn(cellPos)?WorldManager.Instance.GetTileBaseId(cellPos):WorldManager.Instance.GetGroundTileId(cellPos);
        NotificationManager.Instance.ShowPopUpNotify("Đang check thực hiện hành động trên ID " + tileId);
        if (tileId == -1) return false;

        if (currentAction == ActionMode.UseItem)
        {
            return GameDatabase.Instance.ItemDB
                .CheckItemCanUseOn(holdingItemId, tileId);
        }

        if (currentAction == ActionMode.UseTool)
        {
            return GameDatabase.Instance.ToolDB
                .CheckToolCanUseOn(selectedTool, tileId);
        }
        return false;
    }

    private void DoAction()
    {
        state = CharacterState.Busy;

        if (currentAction == ActionMode.UseItem)
            DoPlaceItem();
        else if (currentAction == ActionMode.UseTool)
            DoToolAction();
    }
    private void DoToolAction()
    {
        switch (selectedTool)
        {
            case ToolNames.Scythe:
                DoScythe();
                break;
            case ToolNames.Axe:
                DoAxe();
                break;
            case ToolNames.WaterCan:
                DoWatering();
                break;
        }
    }

    private void DoAxe()
    {
        animator.SetTrigger("UseAxe");
        Debug.Log("Dang chat cay");
    }
    private void DoPlaceItem()
    {
        Vector3Int pos = cellsActionQueue.Dequeue();
        Vector2 cellPos = new Vector2(pos.x, pos.y);
        bool noItem = (holdingItemId == -1);
        int tileId = WorldManager.Instance.HasObjectOn(cellPos) ? WorldManager.Instance.GetTileBaseId(cellPos) : WorldManager.Instance.GetGroundTileId(cellPos);
        bool itemCanNotUse = (!GameDatabase.Instance.ItemDB.CheckItemCanUseOn(holdingItemId, tileId));
        ItemData item  = GameDatabase.Instance.ItemDB.GetItem(holdingItemId);
        if (item == null || noItem || itemCanNotUse)
        {
            SetFree();
            return;
        }
        switch (item.Type)
        {
            // hien tai chi plantseed==holdingItem moi == plantiD
            case ItemType.PlantSeed:
                PlantMangager.Instance.PlantTree(pos,holdingItemId);
                holdingItemSlot.sprite = null;
                break;
        }
        SetFree();
        animator.SetBool("IsHoldingItem", false);
        SelectTool(ToolNames.Scythe);
    }
    private void DoScythe()
    {
        animator.SetTrigger("UseScythe");
        Vector3Int pos = cellsActionQueue.Dequeue();
        Vector2 cellPos = new Vector2(pos.x, pos.y);
        if (!WorldManager.Instance.HasObjectOn(cellPos))
        {
            NotificationManager.Instance.ShowPopUpNotify("Cắt cỏ",NotifyType.Info);
            ParticleManager.Instance.PlayParticle(Particle.grass, WorldManager.Instance.CellPosToWorldCenter(pos));
            WorldManager.Instance.SetMatrixTile(pos, 1, true);
            WorldManager.Instance.SetBaseGroundTile(pos, 1);
        }
        else
        {
            NotificationManager.Instance.ShowPopUpNotify("Thu hoạch", NotifyType.Info);
        }
        SelectTool(ToolNames.WaterCan);
    }
    private void DoWatering()
    {
        animator.SetTrigger("UseWaterCan");
        Vector3Int pos = cellsActionQueue.Dequeue();
        WorldManager.Instance.SetMatrixTile(pos, 2,true);
        WorldManager.Instance.SetBaseGroundTile(pos, 2);
        selectedTool = ToolNames.Hand;
        SetFree();
        HoldItem(201);
    }
    private void SetMoveDirection(Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;

        float x = 0f;
        float y = 0f;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            x = dir.x > 0 ? 1f : -1f;
        }
        else
        {
            y = dir.y > 0 ? 1f : -1f;
        }

        animator.SetFloat("Xmove", x);
        animator.SetFloat("Ymove", y);
    }

    #endregion
}
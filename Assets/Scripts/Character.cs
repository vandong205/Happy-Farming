using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    
    public enum CharacterState
    {
        Free,
        Busy
    }
    #region Fields
    [SerializeField] InputActionAsset IAAsset;
    [SerializeField] Camera followCamera;
    [SerializeField]  float stopDistance = 0.05f;
    [SerializeField] private float moveSpeed;
    [SerializeField] Animator animator;
    private bool IsMoving = false;
    private InputAction moveAction;
    private Coroutine moveRoutine;
    public CharacterState state { get; private set; }
    public float speed {  get; private set; }
    #endregion
    #region Private Method
    private void Awake()
    {
        if(animator==null) animator = GetComponent<Animator>();
        moveAction = IAAsset
            .FindActionMap("Character")
            .FindAction("Move");
        moveAction.performed += Move;
        
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
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = followCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3Int cellPos = WorldManager.Instance.WorldPosToCellPos(mouseWorldPos);
        Debug.Log($"Clicked grid Cell: {cellPos}");
        if (!WorldManager.Instance.IsWalkable(cellPos.x, cellPos.y))
        {
            Debug.Log($"O+{cellPos} khong the di toi!");
            return;
        }
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
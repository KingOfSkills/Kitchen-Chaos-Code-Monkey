using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawn;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawn = null;
    }

    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedUpSomething;
    public static event EventHandler OnAnyPickedUpSomething;
    public event EventHandler<OnSelectedCounterChangeEventArgs> OnSelectedCounterChange;
    public class OnSelectedCounterChangeEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private List<Vector3> spawnPositionsList;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upForce = 10f;
    [SerializeField] private float maxDistanceFromSelectedCounter = 2f;
    [SerializeField] private Vector3 gizmosWireCubeSize;


    private bool isWalking;
    private Vector3 lastInteractionDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        //Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        //GameInput.Instance.OnThrowAction += GameInput_OnThrowAction;

        PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    //private void GameInput_OnThrowAction(object sender, EventArgs e)
    //{
    //    if (HasKitchenObject())
    //    {
    //        KitchenObject kitchenObjectThrow = GetKitchenObject();
    //        ClearKitchenObject();
    //        kitchenObjectThrow.ClearFollowTransform();
    //        Vector3 upForceVector = new Vector3(0, upForce, 0);
    //        kitchenObjectThrow.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce + upForceVector, ForceMode.Impulse);
    //        //Debug.Log(transform.forward);
    //        //Debug.Log(transform.forward * throwForce);
    //        //Debug.Log(transform.forward * throwForce + upForceVector);
    //    }
    //}

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionsList[GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestoryKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Move();
        Interact();
    }

    private void Interact()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalize();

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDirection != Vector3.zero)
        {
            lastInteractionDirection = moveDirection;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractionDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            // Checks infront of player for colliders
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Checks for the script ClearCounter on transform
                if (baseCounter != selectedCounter)
                {
                    // Checks if clearCounter is a different one
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                //SetSelectedCounter(null);
            }
        }
        else
        {
            // If raycast hits nothing
            if (selectedCounter != null && Vector3.Distance(selectedCounter.transform.position, transform.position) > maxDistanceFromSelectedCounter)
            {
                // Still near last selected counter
                SetSelectedCounter(null);
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    float interactDistance = 2f;
    //    Gizmos.color = Color.red;
    //    //Debug.DrawLine(transform.position, transform.position + transform.forward * 2f);
    //    Gizmos.DrawLine(transform.position, transform.position + transform.forward * interactDistance);
    //    Gizmos.DrawWireCube(transform.position + transform.forward + new Vector3(0, .5f, 0) * interactDistance, gizmosWireCubeSize);
    //}

    private void Move()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalize();

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = Time.deltaTime * moveSpeed;
        float playerRadius = .7f;
        //float playerHieght = 2f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            // Can not move towards direction

            // Attempt x movement

            Vector3 moveDirectionX = new Vector3(inputVector.x, 0, 0).normalized;
            canMove = (moveDirection.x < -.5f || moveDirection.x > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                moveDirection = moveDirectionX;
            }
            else
            {
                // Attempt z movement

                Vector3 moveDirectionZ = new Vector3(0, 0, inputVector.y).normalized;
                canMove = (moveDirection.z < -.5f || moveDirection.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
                else
                {
                    //Cannot move
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }

        isWalking = moveDirection != Vector3.zero;

        float rotateSpeed = 15f;

        //if (isWalking)
        //{
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        //}
    }
    public bool IsWalking()
    {
        return isWalking;
    }
    private void SetSelectedCounter(BaseCounter clearCounter)
    {
        selectedCounter = clearCounter;

        OnSelectedCounterChange?.Invoke(this, new OnSelectedCounterChangeEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedUpSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedUpSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}

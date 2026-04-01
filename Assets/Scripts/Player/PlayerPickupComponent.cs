using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerPickupComponent : MonoBehaviour
{
    private InputActionMap _playerActionMap;

    [Header("PickUp")]
    //Sockets for where you hold items
    [SerializeField] private Transform _itemSocket;

    private readonly List<ItemScript> _itemsInRange = new();        //All ItemScripts of all items in Pick Up range
    private readonly List<KidBehaviour> _kidsInRange = new();

    //Current held Item
    private GameObject _heldItem;
    private ItemScript _heldItemScript;

    //Current held kid
    private GameObject _heldKid;
    private KidBehaviour _heldKidBehaviour;

    private BaseStand _ClosestStand;
    private BaseStand _ClosestKidStand;

    private PlayerUIManager _playerUIManager;

    [Header("Throw")]
    [SerializeField] private float _throwStrength = 0f; //250

    private bool _ChargeThrow = false;
    private void Awake()
    {
        PlayerInput input = GetComponentInParent<PlayerInput>();
        _playerActionMap = input.actions.FindActionMap("Player");

        _playerActionMap.FindAction("Interact").started += context => TryInteract();
        _playerActionMap.FindAction("Throw").started += context => StartThrow();
        _playerActionMap.FindAction("Throw").canceled += context => TryThrow();
        _playerActionMap.FindAction("Use").started += context => TryUse();
        _playerUIManager = transform.parent.GetComponentInChildren<PlayerUIManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void StartThrow() 
    {
        if (_heldItemScript == null || _heldItem == null) return;
        _ChargeThrow = true;
        _throwStrength = 0f;
    }

    private void Update()
    {

        if (_ChargeThrow && _throwStrength < 1200f)
        {
            _throwStrength += Time.deltaTime*500f;
        }


        //UI
        bool useInd = false;
        bool pickupInd = false;
        if (_ClosestStand != null && _heldItem != null) 
        {
            useInd = true;
        }
        else if (_itemsInRange.Count > 0 || _kidsInRange.Count >0) 
        {
            pickupInd = true;
            
        }

        _playerUIManager.EnablePickupIndicator(pickupInd);
        _playerUIManager.EnableUseIndicator(useInd);
    }

    private void OnEnable() => _playerActionMap.Enable();
    private void OnDisable() => _playerActionMap.Disable();

    private void TryInteract()
    {
        // --- drop first ---
        if (_heldItem != null)
        {
            Debug.Log("Dropping");
            DropHeldItem();
            return;
        }

        if (_heldKid != null)
        {
            Debug.Log("Dropping Kid");
            DropHeldKid();
            return;
        }

        // --- kids ---
        KidBehaviour closestKid = GetClosestKid();

        if (closestKid != null)
        {
            Debug.Log("Trying to pick up");
            PickUpKid(closestKid);
            return;
        }

        ItemScript closest = GetClosestItem();

        if (closest == null) return;

        if (closest != null && _heldItem == null)       //Picks up closest Item
        {
            Debug.Log("Trying to pick up");
            PickUpItem(closest);
        }
    }

    private void TryThrow()
    {

        if (_heldItem == null) return;
        _ChargeThrow = false;
        DropHeldItem(default, transform.forward * _throwStrength);
        _throwStrength = 0f;

    }

    private void TryUse()
    {
        ItemScript closest = GetClosestItem();

        if (closest != null && closest.GetItemType() == ItemScript.ItemTypes.Box && _heldItem == null)
        {
            closest.HandleUnboxing();
            return;
        }

        TryPlace();

    }

    private void TryPlace() 
    {
        if (_ClosestStand == null || _heldItem == null || _heldItemScript == null) return;

        if ((_ClosestStand.transform.position - transform.position).sqrMagnitude > 100)
        {
            _ClosestStand = null;
            return;
        } //Distance check incase of failure to reset

        var itemType = _ClosestStand.StandItemType;

        if (_ClosestStand.IsTrashCan) 
        {


                Debug.Log("Place Item in trash");
                _ClosestStand.PlaceItem();
                //_ClosestStand.StartQTE(transform.parent.gameObject);
                _heldItemScript.UnCarry(Vector3.zero);
                _heldItem.transform.SetParent(null);
                Destroy(_heldItem);
                _heldItem = null;
                _heldItemScript = null;
                return;
            
        }
        else if (_heldItemScript.ItemType == itemType && _ClosestStand.HasKid && _heldItemScript.isValid) 
        {
            Debug.Log("Place Item");
            _ClosestStand.PlaceItem();
            _ClosestStand.StartQTE(transform.parent.gameObject);
            _heldItemScript.UnCarry(Vector3.zero);
            _heldItem.transform.SetParent(null);
            Destroy(_heldItem);
            _heldItem = null;
            _heldItemScript = null;
            return ;
        }



    }
    private void TryAssignKid()
    {
        if (_ClosestKidStand == null || _heldKid == null || _heldKidBehaviour == null || _heldKidBehaviour.ItemWanted != _ClosestKidStand.StandItemType) return;

        _ClosestKidStand.PlaceChild(_heldKidBehaviour);


    }

    public void SetClosestStand(BaseStand stand) 
    {

        if (stand == null) Debug.Log("Setting closest stand to null can be dangerous! Use RemoveClosestStand");
        
        if (_ClosestStand != null)
        {

            if (Vector3.Distance(transform.position, stand.transform.position) >= 
                Vector3.Distance(_ClosestStand.transform.position, stand.transform.position)) return;

        }

        _ClosestStand = stand;
    }

    public void SetClosestKidStand(BaseStand stand)
    {

        if (stand == null) Debug.Log("Setting closest stand to null can be dangerous! Use RemoveClosestStand");

        if (_ClosestKidStand != null)
        {

            if (Vector3.Distance(transform.position, stand.transform.position) >=
                Vector3.Distance(_ClosestKidStand.transform.position, stand.transform.position)) return;

        }

        _ClosestKidStand = stand;
    }

    public void RemoveClosestStand(BaseStand stand) 
    {
        if (_ClosestStand == stand) 
            _ClosestStand = null;
    }

    public void RemoveClosestKidStand(BaseStand stand)
    {
        if (_ClosestKidStand == stand) _ClosestKidStand = null;
    }


    #region Items
    //Picks up the given item
    private void PickUpItem(ItemScript item)
    {
        if (item == null || item.IsCarried()) return;

        _heldItemScript = item;
        _heldItem = item.gameObject;

        _heldItemScript.Carry();

        Transform parentSocket = _itemSocket;
        
        _heldItem.transform.position = parentSocket.position;
        _heldItem.transform.rotation = parentSocket.rotation;
        _heldItem.transform.SetParent(parentSocket);

        Debug.Log("Picked up: " + _heldItemScript.name);

        RemoveItemFromPickUpRange(item);
    }
    private void PickUpKid(KidBehaviour kid)
    {
        if (kid == null) return;

        _heldKidBehaviour = kid;
        _heldKid = kid.gameObject;

        _heldKidBehaviour.GetNeededStand.ShowIndicator(true);

        Transform parentSocket = _itemSocket;

        _heldKid.transform.position = parentSocket.position;
        _heldKid.transform.rotation = parentSocket.rotation;
        _heldKid.transform.SetParent(parentSocket);

        Debug.Log("Picked up: " + _heldKidBehaviour.name);
        RemoveKidFromRange(kid);
    }

    private void DropHeldItem(Vector3 dropPosition = default, Vector3 throwStrength = default)
    {
        if (_heldItemScript == null || _heldItem == null) return;

        _heldItemScript.UnCarry(throwStrength);
        _heldItem.transform.SetParent(null);

        if (dropPosition != default)    //Checks if you need to swap out items
        {
            _heldItem.transform.position = dropPosition;
        }

        AddItemToPickUpRange(_heldItemScript);

        _heldItem = null;
        _heldItemScript = null;
    }
    private void DropHeldKid()
    {
        if(_heldKid == null || _heldKidBehaviour == null) return;


        _heldKidBehaviour.GetNeededStand.ShowIndicator(false);
        _heldKid.transform.SetParent(null);

        AddKidToRange(_heldKidBehaviour);
        TryAssignKid();

        _heldKid = null;
        _heldKidBehaviour = null;



    }

    private ItemScript GetClosestItem()
    {
        ItemScript closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < _itemsInRange.Count; i++)
        {
            ItemScript item = _itemsInRange[i];

            if (item == null)
            {
                _itemsInRange.RemoveAt(i);
                continue;
            }

            if (item.IsCarried())
            {
                _itemsInRange.RemoveAt(i);
                continue;
            }

            float distance = (item.transform.position - playerPosition).magnitude;

            if (distance < closestDistance)
            {
                closest = item;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private KidBehaviour GetClosestKid()
    {
        KidBehaviour closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (var kid in _kidsInRange)
        {
            if (kid == null)
            {
                _kidsInRange.Remove(kid);
                continue;
            }

            float distance = (kid.transform.position - playerPosition).magnitude;
            if(distance < closestDistance)
            {
                closest = kid; 
                closestDistance = distance;
            }
        }

        return closest;
    }

    //Checks which items enter the player's Pick Up range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemScript itemScript = other.GetComponent<ItemScript>();
            if (itemScript == null) return;

            if (itemScript.IsCarried()) return;

            AddItemToPickUpRange(itemScript);
        }

        if(other.CompareTag("Kid"))
        {
            KidBehaviour kid = other.GetComponent<KidBehaviour>();
            if (kid == null) return;

            if (!kid.IsSad) return; // skip happy ones

            AddKidToRange(kid);
        }
    }

    //Checks which items leave the player's Pick Up range
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemScript itemScript = other.GetComponent<ItemScript>();
            if (itemScript == null) return;

            RemoveItemFromPickUpRange(itemScript);
        }

        if (other.CompareTag("Kid"))
        {
            KidBehaviour kid = other.GetComponent<KidBehaviour>();
            if (kid == null) return;

            RemoveKidFromRange(kid);
        }
    }
    //Adds items to the pick up range list
    private void AddItemToPickUpRange(ItemScript item)
    {
        if (item == null) return;

        Debug.Log("Added :" + item.name);

        if (!_itemsInRange.Contains(item))
            _itemsInRange.Add(item);
    }

    private void AddKidToRange(KidBehaviour kid)
    {
        if (kid == null) return;

        Debug.Log("Added :" + kid.name);
        if (!_kidsInRange.Contains(kid))
            _kidsInRange.Add(kid);
    }


    //Removes items from the pick up range list
    private void RemoveItemFromPickUpRange(ItemScript item)
    {
        if (item == null) return;

        _itemsInRange.Remove(item);
    }

    private void RemoveKidFromRange(KidBehaviour kid)
    {
        if(kid == null) return;

        _kidsInRange.Remove(kid);
    }
    #endregion Items
}

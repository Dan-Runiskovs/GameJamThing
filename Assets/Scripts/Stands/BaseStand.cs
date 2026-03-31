using UnityEngine;

public class BaseStand : MonoBehaviour
{

    [Header("Stand")]
    [SerializeField] ItemScript.ItemTypes _standItem;
    public ItemScript.ItemTypes StandItemType {  get { return _standItem; } }

    [SerializeField] int _itemAmount;

    [Space]
    [Header("Tech")]
    [SerializeField] Collider _checkCollider;

    void Start()
    {
        
    }
    void Update()
    {
        
    }


    public void IncrementItem() 
    {
        _itemAmount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player")) return;


        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null) 
        {
            pickupComp.SetClosestStand(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        PlayerPickupComponent pickupComp = other.gameObject.GetComponent<PlayerPickupComponent>();
        if (pickupComp != null)
        {
            pickupComp.RemoveClosestStand(this);
        }
    }
}

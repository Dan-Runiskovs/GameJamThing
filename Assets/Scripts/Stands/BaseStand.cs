using JetBrains.Annotations;
using UnityEngine;

public class BaseStand : MonoBehaviour
{
    public enum StandType
    {
        Balloon,
        Cake,
        Pinata,
    }



    [Header("Stand")]
    [SerializeField] ItemScript.ItemTypes _standItem;
    public ItemScript.ItemTypes StandItemType {  get { return _standItem; } }

    [SerializeField] bool _itemActive;

    [Space]
    [Header("Tech")]
    [SerializeField] Collider _checkCollider;




    void Start()
    {
        
    }
    void Update()
    {
        
    }


    public void PlaceItem() 
    {
        _itemActive = false;


    }

    public void StartQTE(GameObject playerObject) 
    {
        playerObject?.GetComponent<PlayerController>().EnableMovement(false);
        Debug.Log("QTE");



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

    public void OnChildTriggerEnter(Collider other) 
    {




    }

    public void OnChildTriggerExit(Collider other)
    {
    }
}

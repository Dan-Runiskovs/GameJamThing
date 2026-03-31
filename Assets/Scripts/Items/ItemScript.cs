using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private bool _carried;
    private Rigidbody _rb;
    private Collider _col;

    public enum ItemTypes
    {
        Box,        // Exceptional
        Balloon,
        Cake
        // whatever
    }

    [Tooltip("The type of item.")]
    [SerializeField] private ItemTypes Type;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    public ItemTypes GetItemType()
    {
        return Type;
    }

    public void Carry()
    {
        _carried = true;
        _rb.isKinematic = true;
        _col.enabled = false;
    }
    public void UnCarry(Vector3 throwStrenght)
    {
        _carried = false;
        _rb.isKinematic = false;
        _col.enabled = true;

        _rb.AddForce(throwStrenght, ForceMode.Force);
    }
    public bool IsCarried()
    {
        return _carried;
    }

    public void HandleUnboxing()
    {
        Debug.Log("Unboxing...");
    }
}

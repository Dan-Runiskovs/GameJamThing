using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField] private int _GridPositionIndex = 0;

    public int Index {  get { return _GridPositionIndex; } set { _GridPositionIndex = value; } }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

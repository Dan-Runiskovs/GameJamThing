using UnityEngine;

public class GridObject : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _GridPositionIndex = 0;
    public int Index { get { return _GridPositionIndex; } set { _GridPositionIndex = value; } }
    private GridBehaviour _GridObject;
    [Header("Box")]
    [SerializeField] private GameObject _ItemInBoxPrefab;





    public void SetGridObject(GridBehaviour gridObject) {_GridObject = gridObject; }

    public GameObject HandleUnboxing() 
    { 
        _GridObject.RemoveGridObject(_GridPositionIndex);


        return _ItemInBoxPrefab;



    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

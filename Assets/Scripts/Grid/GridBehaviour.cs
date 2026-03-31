using NUnit.Framework;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{

    [Header("Grid")]
    [SerializeField] private int _Colums;
    [SerializeField] private int _Rows;


    [Space]
    [SerializeField] private float _Width;
    [SerializeField] private float _Height;

    [Space]
    [SerializeField] GameObject _GridObjectPrefab;
    [SerializeField] int _ObjectsCount;




    [SerializeField]  private GridObject[] _Grid;
    [SerializeField]  private List<GridObject> _ExistingGridObjects = new List<GridObject>();



    void Start()
    {
        _Grid = new GridObject[_Colums * _Rows]; //Create new Grid
        transform.localScale = new Vector3(_Width, 1f, _Height);

        if (_ObjectsCount > _Grid.Length) throw new Exception("Object Count higher than gridsize! U stupid or smth?");

        PlaceObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetIndex(int x, int y) 
    {
        return y * _Colums + x;
    }

    public Vector2 GetPos(int i)
    {
        return new Vector2(i %  _Colums, i / _Colums);
    }

    private void PlaceObjects() 
    {
        int amountPlaced = 0;
        int failer = 0;

        while (amountPlaced < _ObjectsCount) 
        {
            int randIndex = Random.Range(0, _Grid.Length);
            failer++;

            if (failer > 1000) throw new Exception("Place Objects endless loop"); //Debug 
            if (_Grid[randIndex] == null) 
            {
                float tileHeight = _Height / _Rows;
                float tileWidth = _Width / _Colums;

                Vector2 gridPos = GetPos(randIndex);
                Vector3 goPos = transform.position;
                goPos.x -= _Width / 2f - 0.5f;
                goPos.z -= _Height / 2f - 0.5f;
                Vector3 pos = new Vector3(gridPos.x*tileWidth+goPos.x,goPos.y, gridPos.y * tileHeight+goPos.z);
                GridObject gobject = Instantiate<GameObject>(_GridObjectPrefab, pos, transform.rotation).GetComponent<GridObject>();
                _Grid[randIndex] = gobject;
                _ExistingGridObjects.Add(gobject);
                gobject.Index = randIndex;
                gobject.SetGridObject(this);
                amountPlaced++;
            }



        }
        
    }

    public void RemoveGridObject(int index) 
    {
        _Grid[index] = null;
    }

}

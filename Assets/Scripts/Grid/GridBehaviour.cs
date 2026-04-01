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


    private float _Width;
    private float _Height;

    [Space]
    [SerializeField] GameObject _GridObjectPrefab;
    [SerializeField] int _ObjectsCount;




    [SerializeField]  private GridObject[] _Grid;
    [SerializeField]  private List<GridObject> _ExistingGridObjects = new List<GridObject>();



    public void StartGrid()
    {

        _Grid = new GridObject[_Colums * _Rows]; //Create new Grid

        _Width = transform.localScale.x;
        _Height = transform.localScale.z;

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
        float tileHeight = _Height / _Rows;
        float tileWidth = _Width / _Colums;
        //Size Check DEBUG
        if (_GridObjectPrefab.transform.localScale.x > tileWidth || _GridObjectPrefab.transform.localScale.z > tileHeight)
            Debug.Log("WARNING: Boxes are too big!");


        while (amountPlaced < _ObjectsCount) 
        {
            int randIndex = Random.Range(0, _Grid.Length);
            failer++;

            if (failer > 1000) throw new Exception("Place Objects endless loop"); //Debug 
            if (_Grid[randIndex] == null) 
            {


               



                Vector2 gridPos = GetPos(randIndex);
                Vector3 goPos = transform.position;
                goPos.x -= _Width / 2f - 0.5f;
                goPos.z -= _Height / 2f - 0.5f;
                Vector3 pos = new Vector3(gridPos.x*tileWidth+goPos.x,goPos.y+1f, gridPos.y * tileHeight+goPos.z);
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

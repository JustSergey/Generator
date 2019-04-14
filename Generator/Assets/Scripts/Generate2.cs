﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Generate2 : MonoBehaviour
{
    [SerializeField]
    private DetailPrefabs detailPrefabs;

    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Grid grid = new Grid(new Vector3(6, 6, 11), new Vector3(0, 0, 5));
        Car car = new Car(detailPrefabs, transform, grid);
        car.Generate(0, 2);
    }
}

public enum Direction { Forward, Back, Up, AtSide, length }
public enum DetailType { Empty, Platform, Wheel, Cabin, length }

[System.Serializable]
public struct DetailPrefabs
{
    public GameObject Platform;
    public GameObject Wheel;
    public GameObject Cabin;
}

public class Detail
{
    public static Detail Empty => new Detail(null, Vector3.zero, Quaternion.identity, null);

    public Vector3 GridPosition;
    private readonly GameObject detailObject;
    private static Dictionary<Direction, Vector3> directionVector = new Dictionary<Direction, Vector3>
    {
        { Direction.Forward, new Vector3(0, 0, 1) },
        { Direction.Back, new Vector3(0, 0, -1) },
        { Direction.AtSide, new Vector3(1, 0, 0) },
        { Direction.Up, new Vector3(0, 1, 0) }
    };

    public Detail(GameObject _prefab, Vector3 _position, Quaternion _rotation, Transform _parent)
    {
        if (_prefab != null)
            detailObject = Object.Instantiate(_prefab, _position, _rotation, _parent);
    }

    private Detail CreateDetail(DetailPrefabs detailPrefabs, DetailType detailType, Vector3 direction)
    {
        if (detailType == DetailType.Empty)
            return Empty;

        var field = typeof(DetailPrefabs).GetFields()[(int)detailType - 1];
        GameObject prefab = (GameObject)field.GetValue(detailPrefabs);

        Vector3 detail_size = detailObject.GetComponent<Renderer>().bounds.size;
        Vector3 prefab_size = prefab.GetComponent<Renderer>().bounds.size;
        Vector3 offset = (detail_size + prefab_size) / 2;
        offset = new Vector3(offset.x * direction.x, offset.y * direction.y, offset.z * direction.z);
        return new Detail(prefab, detailObject.transform.position + offset, Quaternion.identity, detailObject.transform.parent);
    }

    public Detail[] Generate(DetailPrefabs detailPrefabs, int deep, Probabilities probabilities, Grid grid)
    {
        Detail[] details = new Detail[(int)Direction.length];
        for (int dir = 0; dir < (int)Direction.length; dir++)
        {
            directionVector.TryGetValue((Direction)dir, out Vector3 direction);
            if (grid.CheckForDetail(grid.CurrentPosition + direction))
                continue;
            float[] probability = probabilities.GetProbabilities(deep, (Direction)dir);
            float rnd = Random.Range(0f, 99.9f);
            float sum = 0f;
            for (int j = 0; j < probability.Length; j++)
            {
                sum += probability[j];
                if (rnd <= sum)
                {
                    details[dir] = CreateDetail(detailPrefabs, (DetailType)j, direction);
                    break;
                }
            }
            details[dir].GridPosition = grid.CurrentPosition + direction;
            grid.SetDetail(details[dir], details[dir].GridPosition);
        }
        return details;
    }

    public static bool operator ==(Detail detail1, Detail detail2) => detail1.detailObject == detail2.detailObject;
    public static bool operator !=(Detail detail1, Detail detail2) => detail1.detailObject != detail2.detailObject;
}

public class Car
{
    private Detail Head;
    private Probabilities probabilities;
    private DetailPrefabs detailPrefabs;
    private Grid grid;

    public Car(DetailPrefabs _detailPrefabs, Transform _transform, Grid _grid)
    {
        detailPrefabs = _detailPrefabs;
        grid = _grid;
        Head = new Detail(detailPrefabs.Platform, _transform.position, _transform.rotation, _transform);
        grid.SetDetail(Head);
        probabilities = new Probabilities((int)grid.Size.magnitude);
        probabilities.SetRandomProbabilities();
    }

    public void Generate(int deep, int max_deep)
    {
        if (deep >= max_deep)
            return;
        Detail[] details = Head.Generate(detailPrefabs, deep, probabilities, grid);
        deep++;
        for (int i = 0; i < details.Length; i++)
        {
            if (!(details[i] is null) && details[i] != Detail.Empty)
            {
                Head = details[i];
                grid.CurrentPosition = Head.GridPosition;
                Generate(deep, max_deep);
            }
        }
    }
}

public struct Grid
{
    public Vector3 CurrentPosition;
    public readonly Vector3 Size;
    private Detail[,,] data;

    public Grid(Vector3 size, Vector3 begin_position)
    {
        CurrentPosition = begin_position;
        Size = new Vector3((int)size.x, (int)size.y, (int)size.z);
        data = new Detail[(int)Size.x, (int)Size.y, (int)Size.z];
    }

    public bool CheckForDetail(Vector3 position) => data[(int)position.x, (int)position.y, (int)position.z] is null ? false : true;

    public void SetDetail(Detail detail)
    {
        this[(int)CurrentPosition.x, (int)CurrentPosition.y, (int)CurrentPosition.z] = detail;
    }

    public void SetDetail(Detail detail, Vector3 position)
    {
        this[(int)position.x, (int)position.y, (int)position.z] = detail;
    }

    public Detail this[int x, int y, int z]
    {
        get => x < Size.x && y < Size.y && z < Size.z &&
            x >= 0 && y >= 0 && z >= 0 ? data[x, y, z] : throw new System.ArgumentOutOfRangeException();
        set
        {
            if (x < Size.x && y < Size.y && z < Size.z &&
          x >= 0 && y >= 0 && z >= 0) data[x, y, z] = value;
            else throw new System.ArgumentOutOfRangeException();
        }
    }
}

public struct Probabilities
{
    private float[][][] data;

    public Probabilities(int size)
    {
        data = new float[size][][];
        for (int i = 0; i < size; i++)
        {
            data[i] = new float[(int)Direction.length][];
            for (int j = 0; j < data[i].Length; j++)
                data[i][j] = new float[(int)DetailType.length];
        }
    }

    public void SetRandomProbabilities()
    {
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                for (int k = 0; k < data[i][j].Length; k++)
                    data[i][j][k] = Random.Range(0f, 100f);
                Normalize(i, (Direction)j);
            }
        }
    }

    public void SetDefaultProbabilities()
    {

    }

    public void Normalize(int deep, Direction direction)
    {
        float[] _data = GetProbabilities(deep, direction);
        float sum = _data[0];
        for (int i = 1; i < _data.Length; i++)
            sum += _data[i];
        sum = 100 / sum;
        for (int i = 0; i < _data.Length; i++)
            data[deep][(int)direction][i] *= sum;
    }

    public float[] GetProbabilities(int deep, Direction direction)
    {
        return data[deep][(int)direction];
    }
}
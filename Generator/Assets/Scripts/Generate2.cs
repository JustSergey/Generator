using System.Collections;
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
        Grid grid = new Grid(new Vector3(11, 6, 11), new Vector3(5, 0, 5));
        Car car = new Car(detailPrefabs, transform, grid);
        Detail detail = Detail.Empty;
        car.Generate(3);
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

public class Detail : MonoBehaviour
{
    public static Detail Empty => new Detail(null, Vector3.zero, Quaternion.identity, null);
    
    private readonly GameObject detailObject;
    private static Dictionary<Direction, Vector3> direction_offset = new Dictionary<Direction, Vector3>
    {
        { Direction.Forward, new Vector3(0, 0, 1) },
        { Direction.Back, new Vector3(0, 0, -1) },
        { Direction.AtSide, new Vector3(1, 0, 0) },
        { Direction.Up, new Vector3(0, 1, 0) }
    };

    public Detail(GameObject prefab, Vector3 _position, Quaternion _rotation, Transform _parent)
    {
        if (prefab != null)
            detailObject = Instantiate(prefab, _position, _rotation, _parent);
    }

    public Detail[] Generate(DetailPrefabs detailPrefabs, int distance_from_center, Probabilities probabilities, Grid grid)
    {
        Detail[] details = new Detail[(int)Direction.length];
        for (int dir = 0; dir < (int)Direction.length; dir++)
        {
            float[] probability = probabilities.GetProbabilities(distance_from_center, (Direction)dir);
            float rnd = Random.Range(0f, 99.9f);
            float sum = probability[0];
            if (rnd <= sum)
                details[dir] = Empty;
            else
            {
                for (int j = 1; j < probability.Length; j++)
                {
                    sum += probability[j];
                    if (rnd <= sum)
                    {
                        var field = typeof(DetailPrefabs).GetFields()[j - 1];
                        GameObject prefab = (GameObject)field.GetValue(detailPrefabs);

                        Vector3 detail_size = detailObject.GetComponent<Renderer>().bounds.size;
                        Vector3 prefab_size = prefab.GetComponent<Renderer>().bounds.size;
                        Vector3 offset = (detail_size + prefab_size) / 2;
                        direction_offset.TryGetValue((Direction)dir, out Vector3 mult_offset);
                        offset = new Vector3(offset.x * mult_offset.x, offset.y * mult_offset.y, offset.z * mult_offset.z);
                        details[dir] = new Detail(prefab, detailObject.transform.position + offset, Quaternion.identity, detailObject.transform.parent);
                        break;
                    }
                }
            }
        }
        return details;
    }
}

public class Car : MonoBehaviour
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
    }

    public void Generate(int deep)
    {
        Detail[] details = Head.Generate(detailPrefabs, deep, probabilities, grid);
        if (deep <= 0)
            return;
        deep--;
        for (int i = 0; i < details.Length; i++)
        {
            Head = details[i];
            Generate(deep);
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

    public void SetDetail(Detail detail)
    {
        this[(int)CurrentPosition.x, (int)CurrentPosition.y, (int)CurrentPosition.z] = detail;
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

    public void SetDefaultProbabilities()
    {

    }

    public void Normalize(int distance_from_center, Direction direction)
    {
        float[] _data = GetProbabilities(distance_from_center, direction);
        float sum = _data[0];
        for (int i = 1; i < _data.Length; i++)
            sum += _data[i];
        sum = 100 / sum;
        for (int i = 0; i < _data.Length; i++)
            data[distance_from_center][(int)direction][i] *= sum;
    }

    public float[] GetProbabilities(int distance_from_center, Direction direction)
    {
        return data[distance_from_center][(int)direction];
    }
}
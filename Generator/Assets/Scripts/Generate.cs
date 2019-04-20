using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Generate : MonoBehaviour
{
    [SerializeField]
    private DetailPrefabs detailPrefabs;

    private static Car car;

    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Create();
    }

    private void Create()
    {
        Grid grid = new Grid(new Vector3(6, 6, 11), new Vector3(0, 0, 5));
        car = new Car(detailPrefabs, transform, grid);
        car.Generate(0, 2);
        GetComponent<MoveCar>().InitWheels();
    }

    public void Respawn(Vector3 position, Quaternion quaternion, RespawnType respawnType)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = quaternion;
        transform.position = position;
        car.Clear(transform);

        Grid grid = new Grid(new Vector3(6, 6, 11), new Vector3(0, 0, 5));
        car.Update(transform, grid);
        if (respawnType == RespawnType.Mutation)
            car.Mutation();
        else if (respawnType == RespawnType.New)
            car.NewProbabilities(true);
        car.Generate(0, 2);
        GetComponent<MoveCar>().InitWheels();
    }
}

public enum RespawnType { Default, Mutation, New }
public enum Direction { Forward, Back, Up, AtSide, length }
public enum DetailType { Empty, Platform, Wheel, Cabin, Weapon, Box, length }

[System.Serializable]
public struct DetailPrefabs
{
    public GameObject Platform;
    public GameObject Wheel;
    public GameObject Cabin;
    public GameObject Weapon;
    public GameObject Box;
}

public class Detail
{
    public static Detail Empty => new Detail(null, Vector3.zero, Quaternion.identity, null, DetailType.Empty);

    public Vector3 GridPosition;
    private readonly GameObject detailObject;
    private readonly DetailType detailType;
    private static Dictionary<Direction, Vector3> directionVector = new Dictionary<Direction, Vector3>
    {
        { Direction.Forward, new Vector3(0, 0, 1) },
        { Direction.Back, new Vector3(0, 0, -1) },
        { Direction.AtSide, new Vector3(1, 0, 0) },
        { Direction.Up, new Vector3(0, 1, 0) }
    };

    public Detail(GameObject _prefab, Vector3 _position, Quaternion _rotation, Transform _parent, DetailType _detailType)
    {
        detailType = _detailType;
        if (_prefab != null)
            detailObject = Object.Instantiate(_prefab, _position, _rotation, _parent);
    }

    private Detail CreateDetail(DetailPrefabs detailPrefabs, DetailType detailType, Vector3 direction, Vector3 center)
    {
        if (detailType == DetailType.Empty)
            return Empty;

        var field = typeof(DetailPrefabs).GetFields()[(int)detailType - 1];
        GameObject prefab = (GameObject)field.GetValue(detailPrefabs);

        Vector3 detail_size = detailObject.GetComponent<Renderer>().bounds.size;
        Vector3 prefab_size = prefab.GetComponent<Renderer>().bounds.size;
        Vector3 offset = (detail_size + prefab_size) / 2;
        offset = new Vector3(offset.x * direction.x, offset.y * direction.y, offset.z * direction.z);
        Vector3 position = detailObject.transform.position + offset;
        if (center.x - position.x != 0f)
        {
            position.x = 2 * center.x - position.x;
            new Detail(prefab, position, Quaternion.identity, detailObject.transform.parent, detailType);
        }
        return new Detail(prefab, detailObject.transform.position + offset, Quaternion.identity, detailObject.transform.parent, detailType);
    }

    public Detail[] Generate(DetailPrefabs detailPrefabs, int deep, Probabilities probabilities, Grid grid, Vector3 center)
    {
        Detail[] details = new Detail[(int)Direction.length];
        for (int dir = 0; dir < (int)Direction.length; dir++)
        {
            directionVector.TryGetValue((Direction)dir, out Vector3 direction);
            if (grid.CheckForDetail(grid.CurrentPosition + direction))
                continue;
            float[] probability = probabilities.GetProbabilities(detailType, deep, (Direction)dir);
            float rnd = Random.Range(0f, 99.9f);
            float sum = 0f;
            for (int j = 0; j < probability.Length; j++)
            {
                sum += probability[j];
                if (rnd <= sum)
                {
                    details[dir] = CreateDetail(detailPrefabs, (DetailType)j, direction, center);
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
    private Vector3 center;

    public Car(DetailPrefabs _detailPrefabs, Transform _transform, Grid _grid)
    {
        detailPrefabs = _detailPrefabs;
        Update(_transform, _grid);
        NewProbabilities(true);
    }

    public void Generate(int deep, int max_deep)
    {
        if (deep >= max_deep)
            return;
        Detail[] details = Head.Generate(detailPrefabs, deep, probabilities, grid, center);
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

    public void Clear(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
            Object.Destroy(transform.GetChild(i).gameObject);
    }

    public void Update(Transform transform, Grid grid)
    {
        center = transform.position;
        this.grid = grid;
        Head = new Detail(detailPrefabs.Platform, transform.position, transform.rotation, transform, DetailType.Platform);
        grid.SetDetail(Head);
    }

    public void NewProbabilities(bool random)
    {
        probabilities = new Probabilities(random);
    }

    public void Mutation()
    {
        probabilities.Mutation();
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
    private float[,] weight;

    public Probabilities(bool random)
    {
        weight = new float[4, (int)DetailType.length];
        if (random)
            SetRandomWeights();
    }

    private void SetRandomWeights()
    {
        for (int i = 0; i < weight.GetLength(0); i++)
        {
            for (int j = 0; j < weight.GetLength(1); j++)
                weight[i, j] = Random.Range(0f, 1f);
        }
    }

    public void Mutation()
    {
        int index0 = Random.Range(0, weight.GetLength(0));
        int index1 = Random.Range(0, weight.GetLength(1));
        weight[index0, index1] = Random.Range(0f, 1f);
    }

    private void SetDefaultWeights()
    {

    }

    private float[] Normalize(float[] data)
    {
        float sum = 0;
        for (int i = 0; i < data.Length; i++)
            sum += data[i];

        for (int i = 0; i < data.Length; i++)
            data[i] *= 100 / sum;
        return data;
    }

    public float[] GetProbabilities(DetailType detailType, int deep, Direction direction)
    {
        float[] result = new float[(int)DetailType.length];
        for (int detail = 0; detail < result.Length; detail++)
        {
            result[detail] = weight[0, detail] * (int)detailType +
                            weight[1, detail] * deep +
                            weight[2, detail] * (direction == Direction.Up ? 10 : (int)direction) +
                            weight[3, detail] * (direction == Direction.AtSide ? 10 : (int)direction);
        }
        return Normalize(result);
    }
}
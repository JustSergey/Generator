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
        Grid grid = new Grid(new Vector3(11, 11, 11), new Vector3(5, 5, 5));
        grid.SetDetail(new Vector3(11, 11, 11), Detail.Empty);
        Car car = new Car(detailPrefabs, transform, grid);
        car.Generate(3);
        
    }
}

[System.Serializable]
public struct DetailPrefabs
{
    public GameObject Platform;
    public GameObject Wheel;
    public GameObject Cabin;
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
        set { if (x < Size.x && y < Size.y && z < Size.z &&
            x >= 0 && y >= 0 && z >= 0) data[x, y, z] = value; else throw new System.ArgumentOutOfRangeException(); }
    }
}

public class Detail : MonoBehaviour
{
    public DetailType Type;
    public Direction Direction;
    private GameObject detailObject;
    public Detail[] Next;

    public static Detail Empty => new Detail(null, Vector3.zero, Quaternion.identity, null, Direction.Null, DetailType.length);

    public Detail(GameObject _detail_object, Vector3 _position, Quaternion _rotation, Transform _parent, Direction _direction, DetailType _type)
    {
        if (_detail_object != null)
            detailObject = Instantiate(_detail_object, _position, _rotation, _parent);
        Direction = _direction;
        Type = _type;
        Next = new Detail[(int)Direction.length];
    }

    public int Generate(DetailPrefabs detailPrefabs)
    {
        if (Type == DetailType.Wheel)
            return -1;
        int num = Random.Range((Direction == Direction.Null) ? 2 : 1, (int)Direction.length);
        for (int i = 0; i < num; i++)
        {
            Direction direction;
            do
            {
                direction = (Direction)Random.Range(0, (int)Direction.length);
            } while (direction == Direction.Forward && Direction == Direction.Back ||
                    direction == Direction.Back && Direction == Direction.Forward);
            DetailType detailType = (DetailType)Random.Range(0, (int)DetailType.length);

            var field = typeof(DetailPrefabs).GetField(detailType.ToString());
            GameObject prefab = (GameObject)field.GetValue(detailPrefabs);

            Vector3 detail_size = detailObject.GetComponent<Renderer>().bounds.size;
            Vector3 prefab_size = prefab.GetComponent<Renderer>().bounds.size;
            Vector3 offset = new Vector3(0, 0, 0);
            if (direction == Direction.Forward)
                offset = new Vector3(0, 0, detail_size.z / 2 + prefab_size.z / 2);
            else if (direction == Direction.Back)
                offset = -new Vector3(0, 0, detail_size.z / 2 + prefab_size.z / 2);
            else if (direction == Direction.AtSide)
                offset = new Vector3(detail_size.x / 2 + prefab_size.x / 2, 0, 0);
            else if (direction == Direction.Up)
                offset = new Vector3(0, detail_size.y / 2 + prefab_size.y / 2, 0);

            Next[i] = new Detail(prefab, detailObject.transform.position + offset,
                Quaternion.identity, detailObject.transform.parent, direction, detailType);
        }
        return num;
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
        grid = _grid;
        detailPrefabs = _detailPrefabs;
        Head = new Detail(detailPrefabs.Platform, _transform.position, _transform.rotation, _transform, Direction.Null, DetailType.Platform);
        grid.SetDetail(Head);
        probabilities = new Probabilities { Direction = new int[4], Detail = new int[byte.MaxValue] };
    }

    public void Generate(int deep)
    {
        int num = Head.Generate(detailPrefabs);
        if (deep <= 0)
            return;
        deep--;
        for (int i = 0; i < num; i++)
        {
            Head = Head.Next[i];
            Generate(deep);
        }
    }
}

public enum Direction { Null = -1, Forward, Back, Up, AtSide, length }
public enum DetailType { Platform, Wheel, Cabin, length }

public struct Probabilities
{
    public int[] Direction;
    public int[] Detail;
}
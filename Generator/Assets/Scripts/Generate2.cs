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
        Car car = new Car(detailPrefabs, transform);
        car.Generate();
        int type = 0;
        var field = typeof(DetailPrefabs).GetField(((DetailType)type).ToString());
        field.GetValue(detailPrefabs);


    }
}

[System.Serializable]
public struct DetailPrefabs
{
    public GameObject Platform;
    public GameObject Wheel;
    public GameObject Cabin;
}

public class Detail : MonoBehaviour
{
    public DetailType Type;
    public Direction Direction;
    private GameObject detailObject;
    private Detail[] Next;

    public Detail(GameObject _detail_object, Vector3 _position, Quaternion _rotation, Transform _parent, Direction _direction, DetailType _type)
    {
        detailObject = Instantiate(_detail_object, _position, _rotation, _parent);
        Direction = _direction;
        Type = _type;
        Next = new Detail[(int)Direction.length];
    }

    public void Generate(DetailPrefabs detailPrefabs)
    {
        int num = Random.Range((Direction == Direction.Null) ? 2 : 1, (int)Direction.length);
        for (int i = 0; i < num; i++)
        {
            int type = Random.Range(0, (int)DetailType.length);
            var field = typeof(DetailPrefabs).GetField(((DetailType)type).ToString());
            field.GetValue(detailPrefabs);
        }
    }
}

public class Car : MonoBehaviour
{
    private Detail Head;
    private Probabilities probabilities;
    private DetailPrefabs detailPrefabs;

    public Car(DetailPrefabs _detailPrefabs, Transform _transform)
    {
        detailPrefabs = _detailPrefabs;
        Head = new Detail(detailPrefabs.Platform, _transform.position, _transform.rotation, _transform, Direction.Null, DetailType.Platfrom);
        probabilities = new Probabilities { Direction = new int[4], Detail = new int[byte.MaxValue] };
    }

    public void Generate()
    {

    }
}

public enum Direction { Null = -1, Forward, Back, Up, AtSide, length }
public enum DetailType { Platfrom, Wheel, Cabin, length }

public struct Probabilities
{
    public int[] Direction;
    public int[] Detail;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate2 : MonoBehaviour
{
    public GameObject platform;
    public GameObject wheel;
    public GameObject cabin;

    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Car car = new Car(platform, transform);
        car.Generate(new GameObject[] { platform, wheel, cabin });
        

    }
}

public class Detail : MonoBehaviour
{
    public DetailType Type;
    public Direction Direction;
    private GameObject detailObject;

    public Detail(GameObject _detail_object, Vector3 _position, Quaternion _rotation, Transform _parent, Direction _direction, DetailType _type)
    {
        detailObject = Instantiate(_detail_object, _position, _rotation, _parent);
        Direction = _direction;
        Type = _type;
    }
}

public class Car : MonoBehaviour
{
    private List<Detail> details;
    private Probabilities probabilities;

    public Car(GameObject platformPrefab, Transform transform)
    {
        details = new List<Detail> {
            new Detail(platformPrefab, transform.position, transform.rotation, transform, Direction.Null, DetailType.Platfrom) };
        probabilities = new Probabilities { Direction = new int[4], Detail = new int[byte.MaxValue] };
    }

    public void Generate(GameObject[] detailPrefabs)
    {

    }
}

public enum Direction { Null = -1, Forward, Back, Up, AtSide }
public enum DetailType { Platfrom, Wheel, Cabin }

public struct Probabilities
{
    public int[] Direction;
    public int[] Detail;
}
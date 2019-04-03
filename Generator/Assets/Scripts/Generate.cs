using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Generate : MonoBehaviour
{
    const int normal_car = 525882947;

    public GameObject platform;
    public GameObject wheel;
    public GameObject cabin;

    void Start()
    {
        Random.InitState(normal_car);
        CarObject Main = new CarObject(transform, platform, transform.position, transform.rotation, -1, ObjectType.Platform);
        Main.GenerateAll(platform, wheel, cabin, 3);
        Main.GenerateAll(platform, wheel, cabin, 3);
        Main.GenerateAll(platform, wheel, cabin, 3);
    }
}

public class CarObject : MonoBehaviour
{
    ObjectType object_Type;
    int location;
    private GameObject car_Object;

    public CarObject(Transform _parent, GameObject _gameObject, Vector3 _position, Quaternion _rotation, int _location, ObjectType _type)
    {
        car_Object = Instantiate(_gameObject, _position, _rotation);
        car_Object.transform.SetParent(_parent);
        location = _location;
        object_Type = _type;
    }

    public void GenerateAll(GameObject _platform, GameObject _wheel, GameObject _cabin, int deep)
    {
        if (deep >= 0)
        {
            deep--;
            CarObject[] objects = Generate(_platform, _wheel, _cabin);
            if (objects == null)
                return;
            for (int i = 0; i < objects.Length; i++)
                objects[i].GenerateAll(_platform, _wheel, _cabin, deep);
        }
    }

    public CarObject[] Generate(GameObject _platform, GameObject _wheel, GameObject _cabin)
    {
        Bounds bounds = car_Object.GetComponent<Renderer>().bounds;
        int position = Random.Range(0, 4);
        int type = Random.Range(0, 6);

        if (object_Type == ObjectType.Wheel || object_Type == ObjectType.Cabin)
            return null;

        while (true)
        {
            if (type == 0 || type == 1)
            {
                Bounds wheel_bounds = _wheel.GetComponent<Renderer>().bounds;
                Vector3 offset = new Vector3(bounds.size.x / 2 + wheel_bounds.size.x / 2, 0, 0);
                return new CarObject[] {
                    new CarObject(car_Object.transform.parent, _wheel, car_Object.transform.position + offset, Quaternion.Euler(0, 90, 0), 1, ObjectType.Wheel),
                    new CarObject(car_Object.transform.parent, _wheel, car_Object.transform.position - offset, Quaternion.Euler(0, -90, 0), 3, ObjectType.Wheel)};
            }
            else if (type == 2)
            {
                Bounds cabin_bounds = _cabin.GetComponent<Renderer>().bounds;
                Vector3 offset = new Vector3(0, bounds.size.y / 2 + cabin_bounds.size.y / 2, 0);
                return new CarObject[] { new CarObject(car_Object.transform.parent, _cabin, car_Object.transform.position + offset, Quaternion.identity, 0, ObjectType.Cabin) };
            }
            else
            {
                Bounds platform_bounds = _platform.GetComponent<Renderer>().bounds;
                Vector3 offset = new Vector3(0, 0, bounds.size.z / 2 + platform_bounds.size.z / 2);
                if (position % 2 == 0 && location != 2)
                    return new CarObject[] { new CarObject(car_Object.transform.parent, _platform, car_Object.transform.position + offset, Quaternion.identity, 0, ObjectType.Platform) };
                else if (location != 0)
                    return new CarObject[] { new CarObject(car_Object.transform.parent, _platform, car_Object.transform.position - offset, Quaternion.identity, 2, ObjectType.Platform) };
            }
            position = Random.Range(0, 4);
        }
    }

    public void Destroy(float time)
    {
        Destroy(car_Object, time);
    }
}

public enum ObjectType { Platform, Wheel, Cabin };

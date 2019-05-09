using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Generate : MonoBehaviour
{
    [SerializeField]
    private DetailPrefabs detailPrefabs;

    private static Car car;
    const string probabilities_path = "Probabilities";

    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Rules.InitRules();
        Create();
    }

    private void Create()
    {
        Grid grid = new Grid(new Vector3(12, 12, 12));
        car = new Car(detailPrefabs, transform, grid);
        car.LoadProbabilities(transform.position, probabilities_path);
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

        Grid grid = new Grid(new Vector3(12, 12, 12));
        car.Update(transform, grid);
        if (respawnType == RespawnType.Mutation)
            car.Mutation();
        else if (respawnType == RespawnType.New)
            car.NewProbabilities();
        car.SaveProbabilities(position, probabilities_path);
        car.Generate(0, 2);
        GetComponent<MoveCar>().InitWheels();
    }
}

public enum RespawnType { Default, Mutation, New }

[System.Serializable]
public struct DetailPrefabs
{
    public GameObject Platform;
    public GameObject Wheel;
    public GameObject Cabin;
    public GameObject Weapon;
    public GameObject Box;
}
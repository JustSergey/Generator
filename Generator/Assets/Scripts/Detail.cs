using UnityEngine;
using System.Collections.Generic;

public enum Direction { Forward, Back, Up, AtSide, length }
public enum DetailType { Empty, Platform, Wheel, Cabin, Weapon, Box, length }

public class Detail
{
    public static Detail Empty => new Detail(null, Vector3.zero, Quaternion.identity, null, DetailType.Empty);

    public Vector3 GridPosition;
    private readonly GameObject detailObject;
    public readonly DetailType detailType;
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

            DetailType[] possible_details = Rules.GetRule(detailType, (Direction)dir);
            if (possible_details.Length <= 0)
                continue;
            float[] probability = probabilities.GetProbabilities(detailType, deep, (Direction)dir);

            float rnd = Random.Range(0f, 99.9f);
            float sum = 0f;
            for (int j = 0; j < probability.Length; j++)
            {
                sum += probability[j];
                if (rnd <= sum)
                {
                    details[dir] = CreateDetail(detailPrefabs, possible_details[j], direction, center);
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

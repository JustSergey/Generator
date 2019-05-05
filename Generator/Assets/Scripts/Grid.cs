using UnityEngine;

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
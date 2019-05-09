using UnityEngine;

public class Grid
{
    private bool[,,] data;
    private readonly Vector3 center;

    public Grid(Vector3 size)
    {
        data = new bool[(int)size.x * 10, (int)size.y * 10, (int)size.z * 10];
        center = size / 2;
    }

    public void SetDetail(Vector3 size, Vector3 position)
    {
        for (float x = position.x - size.x / 2; x < position.x + size.x / 2; x += 0.1f)
        {
            for (float y = position.y - size.y / 2; y < position.y + size.y / 2; y += 0.1f)
            {
                for (float z = position.z - size.z / 2; z < position.z + size.z / 2; z += 0.1f)
                {
                    data[(int)((x + center.x) * 10), (int)((y + center.y) * 10), (int)((z + center.z) * 10)] = true;
                }
            }
        }
    }

    public bool CheckForCollision(Vector3 size, Vector3 position)
    {
        bool isCollision = false;
        for (float x = position.x - size.x / 2; x < position.x + size.x / 2 && !isCollision; x += 0.1f)
        {
            for (float y = position.y - size.y / 2; y < position.y + size.y / 2 && !isCollision; y += 0.1f)
            {
                for (float z = position.z - size.z / 2; z < position.z + size.z / 2 && !isCollision; z += 0.1f)
                {
                    if (data[(int)((x + center.x) * 10), (int)((y + center.y) * 10), (int)((z + center.z) * 10)] == true)
                        isCollision = true;
                }
            }
        }
        return isCollision;
    }

    /*
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
    */
}
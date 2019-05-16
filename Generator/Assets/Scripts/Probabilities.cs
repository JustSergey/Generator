using UnityEngine;
using System.IO;

public class Probabilities
{
    private float[,,][] probabilities;
    private const int max_deep = 5;

    public Probabilities()
    {
        probabilities = new float[(int)DetailType.length, max_deep, (int)Direction.length][];
        for (int i = 0; i < (int)DetailType.length; i++)
        {
            for (int j = 0; j < (int)Direction.length; j++)
            {
                for (int k = 0; k < max_deep; k++)
                    probabilities[i, k, j] = SetRandomProbabilities(Rules.GetRule((DetailType)i, (Direction)j).Length);
            }
        }
    }

    private float[] SetRandomProbabilities(int length)
    {
        float[] result = new float[length];
        for (int i = 0; i < result.Length; i++)
            result[i] = Random.Range(0f, 1f);
        return Normalize(result);
    }

    public void Load(string path)
    {
        if (!File.Exists(path))
            return;

        FileStream stream = File.OpenRead(path);
        for (int i = 0; i < (int)DetailType.length; i++)
        {
            for (int j = 0; j < (int)Direction.length; j++)
            {
                for (int k = 0; k < max_deep; k++)
                {
                    for (int l = 0; l < probabilities[i, k, j].Length; l++)
                    {
                        byte[] bytes = new byte[sizeof(float)];
                        stream.Read(bytes, 0, bytes.Length);
                        probabilities[i, k, j][l] = System.BitConverter.ToSingle(bytes, 0);
                    }
                }
            }
        }
        stream.Close();
    }

    public void Save(string path)
    {
        FileStream stream = File.Create(path);
        for (int i = 0; i < (int)DetailType.length; i++)
        {
            for (int j = 0; j < (int)Direction.length; j++)
            {
                for (int k = 0; k < max_deep; k++)
                {
                    for (int l = 0; l < probabilities[i, k, j].Length; l++)
                    {
                        byte[] bytes = System.BitConverter.GetBytes(probabilities[i, k, j][l]);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        stream.Close();
    }

    public void Mutation()
    {
        int n = Random.Range(1, 3);
        for (int p = 0; p < n; p++)
        {
            int i = Random.Range(0, (int)DetailType.length);
            int j = Random.Range(0, (int)Direction.length);
            int k = Random.Range(0, max_deep);
            probabilities[i, k, j] = SetRandomProbabilities(probabilities[i, k, j].Length);
        }
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
        return probabilities[(int)detailType, deep, (int)direction];
    }
}

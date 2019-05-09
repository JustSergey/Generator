using UnityEngine;
using System.IO;

public class Probabilities
{
    private float[,] weight;
    private float[,,][] probabilities;

    public Probabilities(bool random)
    {
        probabilities = new float[(int)DetailType.length, 10, (int)Direction.length][];
        for (int i = 0; i < (int)DetailType.length; i++)
        {
            for (int j = 0; j < (int)Direction.length; j++)
            {
                for (int k = 0; k < 10; k++)
                    probabilities[i, k, j] = SetRandomProbabilities(Rules.GetRule((DetailType)i, (Direction)j).Length);
            }
        }

        weight = new float[4, (int)DetailType.length];
        if (random)
            SetRandomWeights();
    }

    private float[] SetRandomProbabilities(int length)
    {
        float[] result = new float[length];
        for (int i = 0; i < result.Length; i++)
            result[i] = Random.Range(0f, 1f);
        return Normalize(result);
    }

    private void SetRandomWeights()
    {
        for (int i = 0; i < weight.GetLength(0); i++)
        {
            for (int j = 0; j < weight.GetLength(1); j++)
                weight[i, j] = Random.Range(0f, 1f);
        }
    }

    private void SetDefaultWeights()
    {

    }

    public void Load(string path)
    {
        if (!File.Exists(path))
        {
            SetRandomWeights();
            return;
        }
        FileStream stream = File.OpenRead(path);
        for (int i = 0; i < weight.GetLength(0); i++)
        {
            for (int j = 0; j < weight.GetLength(1); j++)
            {
                byte[] bytes = new byte[sizeof(float)];
                stream.Read(bytes, 0, bytes.Length);
                weight[i, j] = System.BitConverter.ToSingle(bytes, 0);
            }
        }
        stream.Close();
    }

    public void Save(string path)
    {
        FileStream stream = File.Create(path);
        for (int i = 0; i < weight.GetLength(0); i++)
        {
            for (int j = 0; j < weight.GetLength(1); j++)
            {
                byte[] bytes = System.BitConverter.GetBytes(weight[i, j]);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        stream.Close();
    }

    public void Mutation()
    {
        int i = Random.Range(0, (int)DetailType.length);
        int j = Random.Range(0, (int)Direction.length);
        int k = Random.Range(0, 10);
        probabilities[i, k, j] = SetRandomProbabilities(probabilities[i, k, j].Length);
        /*
        int index0 = Random.Range(0, weight.GetLength(0));
        int index1 = Random.Range(0, weight.GetLength(1));
        weight[index0, index1] = Random.Range(0f, 1f);
        */
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

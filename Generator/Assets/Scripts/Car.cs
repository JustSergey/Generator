using UnityEngine;
using System.IO;

public class Car
{
    private Detail Head;
    private Probabilities probabilities;
    private DetailPrefabs detailPrefabs;
    private Grid grid;
    private Vector3 center;

    public Car(DetailPrefabs _detailPrefabs, Transform _transform, Grid _grid)
    {
        detailPrefabs = _detailPrefabs;
        Update(_transform, _grid);
        NewProbabilities(false);
    }

    public void Generate(int deep, int max_deep)
    {
        if (deep >= max_deep)
            return;
        Detail[] details = Head.Generate(detailPrefabs, deep, probabilities, grid, center);
        deep++;
        for (int i = 0; i < details.Length; i++)
        {
            if (!(details[i] is null) && details[i] != Detail.Empty)
            {
                Head = details[i];
                grid.CurrentPosition = Head.GridPosition;
                Generate(deep, max_deep);
            }
        }
    }

    public void Clear(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
            Object.Destroy(transform.GetChild(i).gameObject);
    }

    public void Update(Transform transform, Grid grid)
    {
        center = transform.position;
        this.grid = grid;
        Head = new Detail(detailPrefabs.Platform, transform.position, transform.rotation, transform, DetailType.Platform);
        grid.SetDetail(Head);
    }

    public void NewProbabilities(bool random)
    {
        probabilities = new Probabilities(random);
    }

    public void Mutation()
    {
        probabilities.Mutation();
    }

    public void LoadProbabilities(Vector3 car_position, string probabilities_path)
    {
        probabilities.Load(probabilities_path + "\\" + car_position.ToString());
    }

    public void SaveProbabilities(Vector3 car_position, string probabilities_path)
    {
        Directory.CreateDirectory(probabilities_path);
        probabilities.Save(probabilities_path + "\\" + car_position.ToString());
    }
}

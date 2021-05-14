using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    public GameObject pointPrefab = default;

    [SerializeField, Range (10, 100)]
    public int maximumUnit = 10;

    [SerializeField]
    public FunctionLibrary.FunctionName function = default;

    public bool AutomaticTransition = false;

    [SerializeField, Min(0f)]
    public float FunctionDuration = 1.5f;

    [SerializeField, Min(0f)]
    public float LerpDuration = 1.0f;

    private float currentLerpDuration = 0f;
    private bool isTransitioning = false;
    private float duration = 0f;
    private float grad = 0f;
    private Vector3 scale = new Vector3();
    private List<GameObject> points;

    private void Start()
    {
        GameObject currentPoint = null;
        this.points = new List<GameObject>();

        this.grad = 2.0f / this.maximumUnit;
        this.scale = Vector3.one * grad;


        for (int i = 0; i < maximumUnit * maximumUnit; i++)
        {
            currentPoint = Instantiate(this.pointPrefab);

            points.Add(currentPoint);

            currentPoint.transform.localScale = this.scale;
            currentPoint.transform.SetParent(this.transform, false);
        }
    }

    private void Update()
    {
        FunctionLibrary.FunctionData functionData;

        if (AutomaticTransition && (this.duration > this.FunctionDuration || isTransitioning))
        {
            duration = 0f;
            this.isTransitioning = true;
            functionData = FunctionLibrary.GetNextFunction(this.function);
        }
        else
        {
            this.duration += Time.deltaTime;
            functionData = FunctionLibrary.GetFunction(this.function);
        }

        this.UpdateGraph(functionData);
    }

    public void UpdateGraph(FunctionLibrary.FunctionData functionData)
    {
        float time = Time.time;
        float v = 0.5f * this.grad - 1f;
        float u = 0f;

        for (int i = 0, x = 0, z = 0; i < maximumUnit * maximumUnit; i++, x++)
        {
            u = (x + 0.5f) * this.grad - 1f;
            if (x == maximumUnit)
            {
                x = 0;
                z++;
                v = (z + 0.5f) * this.grad - 1f;
            }

            if (isTransitioning && this.currentLerpDuration < this.LerpDuration)
            {
                points[i].transform.localPosition = FunctionLibrary.MorphGraphs(u, v, time,
                    FunctionLibrary.GetFunction(this.function).function,
                    functionData.function,
                    this.currentLerpDuration / LerpDuration);
            }
            else
            {
                points[i].transform.localPosition = functionData.function(u, v, time);
                this.isTransitioning = false;
            }   
        }


        if (isTransitioning)
        {
            this.currentLerpDuration += Time.deltaTime;
            //this.function = FunctionLibrary.GetFunction(this.function).functionName;
        }
        else
        {
            this.currentLerpDuration = 0;
            this.function = functionData.functionName;
        }
    }
}

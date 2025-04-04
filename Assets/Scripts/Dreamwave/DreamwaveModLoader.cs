using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TempoManager;
using static GameManager;

public class DreamwaveModLoader : MonoBehaviour
{
    public string ChartsLocation;
    public Transform PlayerChart;
    public Transform EnemyChart;

    public GameObject LeftNote;
    public GameObject DownNote;
    public GameObject UpNote;
    public GameObject RightNote;
    public GameObject HoldNote;

    private void Start()
    {
        SetupChartSettings(ChartsLocation + "settings.txt");
        CreateChart(ChartsLocation + "pchart.txt", PlayerChart);
        CreateChart(ChartsLocation + "echart.txt", EnemyChart);
    }

    public void SetupChartSettings(string location)
    {
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + location);

        foreach (string line in lines)
        {
            if (line.StartsWith("startScrollAtStep="))
            {
                Instance.SongStartStep = int.Parse(line.Split("=")[1]);
            }
            else if (line.StartsWith("scrollSpeedMultiplier="))
            {
                Instance.scrollManager.scrollSpeedMultiplier = float.Parse(line.Split("=")[1]);
            }
            else if (line.StartsWith("snapThreshold="))
            {
                Instance.scrollManager.snapThreshold = float.Parse(line.Split("=")[1]);
            }
        }
    }

    GameObject currentNote;
    int lane;
    public void CreateChart(string location, Transform chartParent)
    {
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + location);

        foreach (string line in lines)
        {
            if (line.Contains("# start"))
            {
                currentNote = new GameObject();
                currentNote.transform.SetParent(chartParent);
            }
            else if (line.Contains("# end"))
            {
                Destroy(currentNote);
                currentNote = null;
            }

            if (line.StartsWith("lane="))
            {
                lane = int.Parse(line.Split("=")[1]);

                if (lane == 0) currentNote.transform.localPosition = new Vector3(2.248f, 0, 0);
                else if (lane == 1) currentNote.transform.localPosition = new Vector3(0.7479999f, 0, 0);
                else if (lane == 2) currentNote.transform.localPosition = new Vector3(-0.7520001f, 0, 0);
                else if (lane == 3) currentNote.transform.localPosition = new Vector3(-2.252f, 0, 0);
            }
            else if (line.StartsWith("position="))
            {
                currentNote.transform.localPosition = new Vector3(currentNote.transform.localPosition.x, float.Parse(line.Split("=")[1]), 0);
            }
            else if (line.StartsWith("type="))
            {
                if (line.Split("=")[1] == "N")
                {
                    if (lane == 0)
                    {
                        var iNote = Instantiate(LeftNote, currentNote.transform.position, LeftNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        Debug.Log($"Left note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 1)
                    {
                        var iNote = Instantiate(DownNote, currentNote.transform.position, DownNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        Debug.Log($"Down note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 2)
                    {
                        var iNote = Instantiate(UpNote, currentNote.transform.position, UpNote.transform.rotation);
                        currentNote.transform.SetParent(chartParent);
                        iNote.transform.SetParent(chartParent);
                        Debug.Log($"Up note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 3)
                    {
                        var iNote = Instantiate(RightNote, currentNote.transform.position, RightNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        Debug.Log($"Right note created at {currentNote.transform.localPosition}");
                    }
                }
            }
        }
    }
}

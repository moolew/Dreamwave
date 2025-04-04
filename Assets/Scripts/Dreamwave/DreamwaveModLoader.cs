using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TempoManager;
using static GameManager;
using TreeEditor;

public class DreamwaveModLoader : MonoBehaviour
{
    public string ChartsLocation;
    public Transform EventsChart;
    public Transform PlayerChart;
    public Transform EnemyChart;

    public GameObject Event;
    public GameObject LeftNote;
    public GameObject DownNote;
    public GameObject UpNote;
    public GameObject RightNote;
    public GameObject LeftHoldNote;
    public GameObject DownHoldNote;
    public GameObject UpHoldNote;
    public GameObject RightHoldNote;

    private void Start()
    {
        SetupChartSettings(ChartsLocation + "settings.txt");
        CreateChart(ChartsLocation + "pchart.txt", PlayerChart, 6);
        CreateChart(ChartsLocation + "echart.txt", EnemyChart, 7);
        CreateEventsChart(ChartsLocation + "cchart.txt", EventsChart);
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
    public void CreateChart(string location, Transform chartParent, int layer)
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
                currentNote.transform.localPosition = new Vector3(currentNote.transform.localPosition.x, -float.Parse(line.Split("=")[1]), 0);
            }
            else if (line.StartsWith("type="))
            {
                if (line.Split("=")[1] == "N") // normal note
                {
                    if (lane == 0)
                    {
                        var iNote = Instantiate(LeftNote, currentNote.transform.position, LeftNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; iNote.tag = "Note"; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Left note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 1)
                    {
                        var iNote = Instantiate(DownNote, currentNote.transform.position, DownNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; iNote.tag = "Note"; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Down note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 2)
                    {
                        var iNote = Instantiate(UpNote, currentNote.transform.position, UpNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; iNote.tag = "Note"; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Up note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 3)
                    {
                        var iNote = Instantiate(RightNote, currentNote.transform.position, RightNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; iNote.tag = "Note"; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Right note created at {currentNote.transform.localPosition}");
                    }
                }
                else if (line.Split("=")[1] == "H") // hold note
                {
                    if (lane == 0)
                    {
                        var iNote = Instantiate(LeftHoldNote, currentNote.transform.position, LeftHoldNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Left note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 1)
                    {
                        var iNote = Instantiate(DownHoldNote, currentNote.transform.position, DownHoldNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Down note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 2)
                    {
                        var iNote = Instantiate(UpHoldNote, currentNote.transform.position, UpHoldNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; }
                        else if (layer == 7) { iNote.layer = 7; iNote.tag = "EnemyNote"; }
                        Debug.Log($"Up note created at {currentNote.transform.localPosition}");
                    }
                    else if (lane == 3)
                    {
                        var iNote = Instantiate(RightHoldNote, currentNote.transform.position, RightHoldNote.transform.rotation);
                        iNote.transform.SetParent(chartParent);
                        if (layer == 6) { iNote.layer = 6; }
                        else if (layer == 7) { iNote.layer = 7; }
                        Debug.Log($"Right note created at {currentNote.transform.localPosition}");
                    }
                }
            }
        }
    }

    GameObject currentEvent;
    public void CreateEventsChart(string location, Transform chartParent)
    {
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + location);

        foreach (string line in lines)
        {
            if (line.StartsWith("# start"))
            {
                currentEvent = new GameObject();
                currentEvent.transform.SetParent(chartParent);
            }
            else if (line.StartsWith("# end"))
            {
                Destroy(currentEvent);
                currentEvent = null;
            }

            if (line.StartsWith("position="))
            {
                currentEvent.transform.position = new Vector3(0, -float.Parse(line.Split("=")[1], 0));
            }
            else if (line.StartsWith("type="))
            {
                string ev = line.Split("=")[1];
                string[] splitEv = ev.Split('-');

                if (splitEv[0] == "PF")
                {
                    string focus = splitEv.Length > 1 ? splitEv[1] : "";

                    if (focus == "p")
                    {
                        var r = Instantiate(Event, currentEvent.transform.position, Event.transform.rotation);
                        r.transform.SetParent(chartParent);
                        r.layer = 6;
                        r.GetComponent<ScrollEvents>().typeOfScrollEvent = TypeOfScrollEvent.FocusPlayerRight;
                    }
                    else if (focus == "e")
                    {
                        var r = Instantiate(Event, currentEvent.transform.position, Event.transform.rotation);
                        r.transform.SetParent(chartParent);
                        r.layer = 6;
                        r.GetComponent<ScrollEvents>().typeOfScrollEvent = TypeOfScrollEvent.FocusPlayerLeft;
                    }
                    else if (focus == "c")
                    {
                        var r = Instantiate(Event, currentEvent.transform.position, Event.transform.rotation);
                        r.transform.SetParent(chartParent);
                        r.layer = 6;
                        r.GetComponent<ScrollEvents>().typeOfScrollEvent = TypeOfScrollEvent.FocusCentre;
                    }
                }
            }
        }
    }
}

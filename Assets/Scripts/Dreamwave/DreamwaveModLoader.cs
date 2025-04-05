using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TempoManager;
using static GameManager;

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
    public GameObject LeftHoldNoteChunk;
    public GameObject LeftHoldNoteEnd;

    public GameObject DownHoldNote;
    public GameObject DownHoldChunk;
    public GameObject DownHoldEnd;

    public GameObject UpHoldNote;
    public GameObject UpHoldChunk;
    public GameObject UpHoldEnd;

    public GameObject RightHoldNote;
    public GameObject RightHoldChunk;
    public GameObject RightHoldEnd;

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
    float holdLength = 0f;
    string noteType = "";
    public void CreateChart(string location, Transform chartParent, int layer)
    {
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + location);

        foreach (string line in lines)
        {
            if (line.Contains("# start"))
            {
                currentNote = new GameObject("NoteHolder");
                currentNote.transform.SetParent(chartParent);
                currentNote.transform.localPosition = Vector3.zero;
                holdLength = 0f;
                noteType = "";
            }
            else if (line.Contains("# end"))
            {
                if (noteType == "N" || noteType == "H")
                {
                    Vector3 spawnPosition = currentNote.transform.localPosition;

                    GameObject prefab = null;
                    GameObject chunkPrefab = null;
                    GameObject endPrefab = null;

                    switch (lane)
                    {
                        case 0:
                            prefab = (noteType == "N") ? LeftNote : LeftHoldNote;
                            chunkPrefab = LeftHoldNoteChunk;
                            endPrefab = LeftHoldNoteEnd;
                            break;
                        case 1:
                            prefab = (noteType == "N") ? DownNote : DownHoldNote;
                            chunkPrefab = DownHoldChunk;
                            endPrefab = DownHoldEnd;
                            break;
                        case 2:
                            prefab = (noteType == "N") ? UpNote : UpHoldNote;
                            chunkPrefab = UpHoldChunk;
                            endPrefab = UpHoldEnd;
                            break;
                        case 3:
                            prefab = (noteType == "N") ? RightNote : RightHoldNote;
                            chunkPrefab = RightHoldChunk;
                            endPrefab = RightHoldEnd;
                            break;
                    }

                    if (prefab != null)
                    {
                        var note = Instantiate(prefab, chartParent);
                        note.transform.localPosition = spawnPosition;
                        note.layer = layer;
                        note.tag = (layer == 6) ? "Note" : "EnemyNote";

                        if (noteType == "H" && chunkPrefab != null && endPrefab != null)
                        {
                            float chunkStep = 0.5f;
                            float startY = spawnPosition.y;
                            float endY = spawnPosition.y - holdLength;

                            for (float y = startY - chunkStep; y > endY; y -= chunkStep)
                            {
                                var chunk = Instantiate(chunkPrefab, note.transform.position, chunkPrefab.transform.rotation); // parented to hold note
                                chunk.transform.localPosition = new Vector3(0, y - startY, 0); // local relative to note
                                chunk.layer = layer;
                            }

                            var endNote = Instantiate(endPrefab, note.transform.position, endPrefab.transform.rotation); // parented to hold note
                            endNote.transform.localPosition = new Vector3(0, endY - startY, 0); // local relative to note
                            endNote.layer = layer;
                        }
                    }

                    if (noteType == "H" && chunkPrefab != null && endPrefab != null)
                    {
                        float chunkStep = 0.39f;
                        float startY = spawnPosition.y;
                        float endY = spawnPosition.y - holdLength;

                        for (float y = startY - chunkStep; y > endY; y -= chunkStep)
                        {
                            var chunk = Instantiate(chunkPrefab, chartParent);
                            chunk.transform.localPosition = new Vector3(spawnPosition.x, y, 0);
                            chunk.layer = layer;
                            if (chartParent == EnemyChart)
                            {
                                var doc = chunk.GetComponent<DisableOnCollision>();
                                doc._ai = true;
                            }
                        }

                        var endNote = Instantiate(endPrefab, chartParent);
                        endNote.transform.localPosition = new Vector3(spawnPosition.x, endY, 0);
                        endNote.layer = layer;
                        if (chartParent == EnemyChart)
                        {
                            var doc = endNote.GetComponent<DisableOnCollision>();
                            doc._ai = true;
                        }
                    }
                }

                Destroy(currentNote);
                currentNote = null;
            }
            else if (line.StartsWith("lane="))
            {
                lane = int.Parse(line.Split("=")[1]);
                float x = 0f;
                if (lane == 0) x = 2.248f;
                else if (lane == 1) x = 0.748f;
                else if (lane == 2) x = -0.752f;
                else if (lane == 3) x = -2.252f;
                currentNote.transform.localPosition = new Vector3(x, currentNote.transform.localPosition.y, 0);
            }
            else if (line.StartsWith("position="))
            {
                float y = -float.Parse(line.Split("=")[1]);
                currentNote.transform.localPosition = new Vector3(currentNote.transform.localPosition.x, y, 0);
            }
            else if (line.StartsWith("type="))
            {
                noteType = line.Split("=")[1];
            }
            else if (line.StartsWith("length=") && noteType == "H")
            {
                holdLength = float.Parse(line.Split("=")[1]);
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
                currentEvent = new GameObject("EventHolder");
                currentEvent.transform.SetParent(chartParent);
                currentEvent.transform.localPosition = Vector3.zero;
            }
            else if (line.StartsWith("# end"))
            {
                Destroy(currentEvent);
                currentEvent = null;
            }
            else if (line.StartsWith("position="))
            {
                float y = -float.Parse(line.Split("=")[1]);
                currentEvent.transform.localPosition = new Vector3(0, y, 0);
            }
            else if (line.StartsWith("type="))
            {
                string ev = line.Split("=")[1];
                string[] splitEv = ev.Split('-');

                if (splitEv[0] == "PF")
                {
                    string focus = splitEv.Length > 1 ? splitEv[1] : "";

                    var r = Instantiate(Event, chartParent);
                    r.transform.localPosition = currentEvent.transform.localPosition;
                    r.layer = 6;

                    var se = r.GetComponent<ScrollEvents>();
                    if (focus == "p") se.typeOfScrollEvent = TypeOfScrollEvent.FocusPlayerRight;
                    else if (focus == "e") se.typeOfScrollEvent = TypeOfScrollEvent.FocusPlayerLeft;
                    else if (focus == "c") se.typeOfScrollEvent = TypeOfScrollEvent.FocusCentre;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TempoManager;
using static GameManager;
using static DreamwaveGlobal;

public class DreamwaveModLoader : MonoBehaviour
{
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
        ModSong mod = LoadedModSong;

        SetupChartSettings(mod.chartSettings);
        CreateChart(mod.playerChart, PlayerChart, 6);
        CreateChart(mod.enemyChart, EnemyChart, 7);
        CreateEventsChart(mod.eventChart, EventsChart);
    }

    public void SetupChartSettings(string location)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, location);
        Debug.Log("Loading chart settings from: " + fullPath);
        string[] lines = File.ReadAllLines(fullPath);

        foreach (string line in lines)
        {
            if (line.StartsWith("startScrollAtStep="))
            {
                Instance.SongStartStep = int.Parse(line.Split('=')[1]);
            }
            else if (line.StartsWith("scrollSpeedMultiplier="))
            {
                Instance.scrollManager.scrollSpeedMultiplier = float.Parse(line.Split('=')[1]);
            }
            else if (line.StartsWith("songPitch="))
            {
                instance.audioSource.pitch = float.Parse(line.Split('=')[1]);
            }
            else if (line.StartsWith("cameraYOffset="))
            {
                GameObject.Find("Main Camera").transform.localPosition = new Vector3(0, float.Parse(line.Split('=')[1]), 0);
            }
        }
    }

    GameObject currentNote;
    int lane;
    float holdLength = 0f;
    string noteType = "";

    HashSet<string> usedNotes = new HashSet<string>();

    public void CreateChart(string location, Transform chartParent, int layer)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, location);
        Debug.Log("Loading chart from: " + fullPath);
        string[] lines = File.ReadAllLines(fullPath);

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
                    string noteKey = $"{chartParent.GetInstanceID()}_{lane}_{spawnPosition.y:F3}"
;

                    if (usedNotes.Contains(noteKey))
                    {
                        Debug.LogWarning($"Duplicate note detected at lane {lane}, position {spawnPosition.y:F3}, skipping.");
                        Destroy(currentNote);
                        continue;
                    }

                    usedNotes.Add(noteKey);

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
                            float chunkStep = 0.39f;
                            float startY = spawnPosition.y;
                            float endY = spawnPosition.y - holdLength;
                            for (float y = startY - chunkStep; y > endY; y -= chunkStep)
                            {
                                var chunk = Instantiate(chunkPrefab, chartParent);
                                chunk.transform.localPosition = new Vector3(spawnPosition.x, y, 0);
                                chunk.layer = layer;

                                var docc = chunk.GetComponent<DisableOnCollision>();
                                docc._ai = (chartParent == EnemyChart);
                            }
                            var endNote = Instantiate(endPrefab, chartParent);
                            endNote.transform.localPosition = new Vector3(spawnPosition.x, endY, 0);
                            endNote.layer = layer;

                            var doce = endNote.GetComponent<DisableOnCollision>();
                            doce._ai = (chartParent == EnemyChart);
                        }
                    }
                }

                Destroy(currentNote);
                currentNote = null;
            }
            else if (line.StartsWith("lane="))
            {
                lane = int.Parse(line.Split('=')[1]);
                float x = lane switch
                {
                    0 => 2.248f,
                    1 => 0.748f,
                    2 => -0.752f,
                    3 => -2.252f,
                    _ => 0f
                };
                currentNote.transform.localPosition = new Vector3(x, currentNote.transform.localPosition.y, 0);
            }
            else if (line.StartsWith("position="))
            {
                float y = -float.Parse(line.Split('=')[1]);
                y *= PlayerPrefs.GetFloat("scrollSpeed");

                currentNote.transform.localPosition = new Vector3(currentNote.transform.localPosition.x, y, 0);
            }
            else if (line.StartsWith("type="))
            {
                noteType = line.Split('=')[1];
            }
            else if (line.StartsWith("length=") && noteType == "H")
            {
                holdLength = float.Parse(line.Split('=')[1]);
                holdLength *= PlayerPrefs.GetFloat("scrollSpeed"); // Scale hold length
            }
        }
    }

    // EventsChart remains unchanged
    GameObject currentEvent;
    ScrollEvents currentEventI;
    string eventType = "";

    string axis;

    public void CreateEventsChart(string location, Transform chartParent)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, location);
        Debug.Log("Loading events chart from: " + fullPath);
        string[] lines = File.ReadAllLines(fullPath);

        foreach (string line in lines)
        {
            if (line.StartsWith("# start"))
            {
                currentEvent = new GameObject("EventHolder");
                currentEvent.transform.SetParent(chartParent);
                currentEvent.transform.localPosition = Vector3.zero;
                eventType = "";
                currentEventI = null;
            }
            else if (line.StartsWith("# end"))
            {
                Destroy(currentEvent);
                currentEvent = null;
                eventType = "";
                currentEventI = null;
            }
            else if (line.StartsWith("position="))
            {
                float y = -float.Parse(line.Split('=')[1]);
                y *= PlayerPrefs.GetFloat("scrollSpeed");

                currentEvent.transform.localPosition = new Vector3(0, y, 0);
            }
            else if (line.StartsWith("type="))
            {
                string ev = line.Split('=')[1];
                eventType = ev;
                string[] splitEv = ev.Split('-');

                if (splitEv[0] == "PF")
                {
                    string focus = splitEv.Length > 1 ? splitEv[1] : "";
                    var r = Instantiate(Event, chartParent);
                    r.transform.localPosition = currentEvent.transform.localPosition;
                    r.layer = 6;
                    currentEventI = r.GetComponent<ScrollEvents>();
                    if (focus == "p") currentEventI.typeOfScrollEvent = TypeOfScrollEvent.FocusPlayerRight;
                    else if (focus == "e") currentEventI.typeOfScrollEvent = TypeOfScrollEvent.FocusPlayerLeft;
                    else if (focus == "c") currentEventI.typeOfScrollEvent = TypeOfScrollEvent.FocusCentre;
                }
                else if (ev == "Z")
                {
                    var r = Instantiate(Event, chartParent);
                    r.transform.localPosition = currentEvent.transform.localPosition;
                    r.layer = 6;
                    currentEventI = r.GetComponent<ScrollEvents>();
                    currentEventI.typeOfScrollEvent = TypeOfScrollEvent.CameraFov;
                }
                else if (ev == "R")
                {
                    var r = Instantiate(Event, chartParent);
                    r.transform.position = currentEvent.transform.position;
                    r.layer = 6;
                    currentEventI = r.GetComponent<ScrollEvents>();
                    currentEventI.typeOfScrollEvent = TypeOfScrollEvent.RepeatedTile;
                }
                else if (ev == "RC")
                {
                    var rc = Instantiate(Event, chartParent);
                    rc.transform.position = currentEvent.transform.position;
                    rc.layer = 6;
                    currentEventI = rc.GetComponent<ScrollEvents>();
                    currentEventI.typeOfScrollEvent = TypeOfScrollEvent.RotateTile;
                }
                else if (ev == "MOV")
                {
                    var rc = Instantiate(Event, chartParent);
                    rc.transform.position = currentEvent.transform.position;
                    rc.layer = 6;
                    currentEventI = rc.GetComponent<ScrollEvents>();
                    currentEventI.typeOfScrollEvent = TypeOfScrollEvent.MoveTiles;

                    if (axis == "x")
                        currentEventI.Axis = "X";
                    else if (axis == "y")
                        currentEventI.Axis = "Y";
                }
            }
            else if (eventType == "Z" && line.StartsWith("amount="))
            {
                currentEventI.ZoomAmount = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "Z" && line.StartsWith("speed="))
            {
                currentEventI.ZoomSpeed = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "Z" && line.StartsWith("bpmBump="))
            {
                currentEventI.BpmBump = bool.Parse(line.Split('=')[1]);
            }
            else if (eventType == "R" && line.StartsWith("repeatRate="))
            {
                currentEventI.RepeatRate = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "R" && line.StartsWith("repeatTime="))
            {
                currentEventI.RepeatTime = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "RC" && line.StartsWith("rotateAmount="))
            {
                currentEventI.RotateAmount = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "RC" && line.StartsWith("rotateTime="))
            {
                currentEventI.RotateTime = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "MOV" && line.StartsWith("axis="))
            {
                currentEventI.Axis = line.Split('=')[1];

                if (currentEventI.Axis.ToUpper() == "X")
                    axis = "x";
                else if (currentEventI.Axis.ToUpper() == "X")
                    axis = "y";
                else
                    axis = "y";
            }
            else if (eventType == "MOV" && line.StartsWith("moveAmount="))
            {
                currentEventI.MoveAmount = float.Parse(line.Split('=')[1]);
            }
            else if (eventType == "MOV" && line.StartsWith("moveTime="))
            {
                currentEventI.MoveTime = float.Parse(line.Split('=')[1]);
            }
        }
    }
}

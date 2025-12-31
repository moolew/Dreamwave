using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static GameManager;
using static TempoManager;
using static DreamwaveGlobal;
using System.Linq;

/// <summary>
/// Custom asset loader.
/// 
/// This script searches through the game's mod index for
/// a text document that contains the state of custom assets.
/// 
/// this code hurts my brain
/// </summary>
public enum TypeAsset
{
    Custom,
    Mod,
    Default //fallback
}

public class CustomAssetLoader : MonoBehaviour
{
    public TypeAsset _typePlayerOne;
    public TypeAsset _typePlayerTwo;
    public TypeAsset _typeNoteAsset;
    public TypeAsset _typeNoteParticleAsset;

    #region Note Particle Renderer Section
    [Space(10)]
    [Header("Note Particle Settings")]
    public string CustomNoteParticleFileName;
    public int _particleSpriteWidth = 1080;
    public int _particleSpriteHeight = 1080;
    [Header("Note Particle Renderers")]
    [SerializeField] private List<DreamwaveParticle> _dreamwaveParticles;
    [SerializeField] private List<Sprite> _noteParticleAnimation;
    #endregion

    #region Note Section
    [Space(10)]
    [Header("Note Renderers")]
    public string PlayerCustomNoteFileLocation;
    public string AiCustomNoteFileLocation;

    public string PlayerCustomStreamedNoteFileLocation;
    public string AiPlayerCustomStreamedNoteFileLocation;
    public string PlayerCustomChunkNoteFileLocation;
    public string AiCustomNoteChunkNoteFileLocation;
    public string PlayerCustomHoldNoteEndFileLocation;
    public string AiCustomNoteHoldNoteEndFileLocation;

    public string PlayerCustomNoteHeldFileLocation;
    public string AiCustomNoteHeldFileLocation;
    public int _NoteWidth = 470;
    public int _NoteHeight = 540;
    public Color[] _noteColors;
    [SerializeField] private List<SpriteRenderer> _playerNoteSpriteRenderers;
    [SerializeField] private List<SpriteRenderer> _enemyNoteSpriteRenderers;

    [SerializeField] private List<SpriteRenderer> _playerStreamedNoteSpriteRenderers;
    [SerializeField] private List<SpriteRenderer> _enemyStreamedNoteSpriteRenderers;

    [SerializeField] private List<SpriteRenderer> _playerHoldNoteChunkRenderers;
    [SerializeField] private List<SpriteRenderer> _enemyHoldNoteChunkRenderers;

    [SerializeField] private List<SpriteRenderer> _playerHoldNoteEndRenderers;
    [SerializeField] private List<SpriteRenderer> _enemyHoldNoteEndRenderers;

    [SerializeField] private NoteController _noteController;
    [SerializeField] private List<DreamwaveAICommunicator> _aiNoteControllers;
    #endregion

    #region Player One Section
    [Space(10)]
    [Header("Player One Animation Settings")]
    public string CustomPlayerOneFileName;
    public int _playerOneSpriteWidth = 1080;
    public int _playerOneSpriteHeight = 1080;
    public float _playerOneRectX = 1f;
    public float _playerOneRectY = 1f;
    public bool _shouldFlipPlayerOneCustom = true;
    public bool _shouldPlayerOneAntiAlias = true;
    [SerializeField] private DreamwaveCharacter _playerOneScript;

    [Space(5)]
    public string CustomPlayerOneIconFileName;
    public int _playerOneIconWidth = 1080;
    public int _playerOneIconHeight = 1080;
    public float _playerOneIconRectX = 1f;
    public float _playerOneIconRectY = 1f;
    [SerializeField] private DreamwaveIcon _playerOneIconScript;
    #endregion

    #region Player Two Section - AI
    [Space(10)]
    [Header("Player Two Animation Settings")]
    public string CustomAiPlayerTwoFileName;
    public int _playerTwoAiSpriteWidth = 1080;
    public int _playerTwoAiSpriteHeight = 1080;
    public float _playerTwoRectX = 0.5f;
    public float _playerTwoRectY = 0.5f;
    public bool _shouldFlipPlayerTwoCustom = false;
    public bool _shouldPlayerTwoAntiAlias = true;
    [SerializeField] private DreamwaveAICharacter _playerTwoAiScript;

    [Space(5)]
    public string CustomPlayerTwoIconFileName;
    public int _playerTwoIconWidth = 1080;
    public int _playerTwoIconHeight = 1080;
    public float _playerTwoIconRectX = 1f;
    public float _playerTwoIconRectY = 1f;
    [SerializeField] private DreamwaveIcon _playerTwoIconScript;
    #endregion

    [Space(10)]
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private string _backgroundFileName;

    [Space(10)]
    [SerializeField] private string mp3FileName;

    private void Awake()
    {
        // loaded mod instance
        ModSong mod = LoadedModSong;

        // credits
        Instance.SongName = mod.name;
        Instance.SongCreatorName = mod.creator;

        // characters
        CustomPlayerOneFileName = mod.playerSprites;
        CustomAiPlayerTwoFileName = mod.enemySprites;
        _backgroundFileName = mod.backgroundSprite;
         
        // controller notes
        PlayerCustomNoteFileLocation = mod.playerNoteControllerSprites;
        AiCustomNoteFileLocation = mod.enemyNoteControllerSprites;

        // streamed notes
        PlayerCustomStreamedNoteFileLocation = mod.playerStreamedNoteSprites;
        AiPlayerCustomStreamedNoteFileLocation = mod.enemyStreamedNoteSprites;

        // hold chunks
        PlayerCustomChunkNoteFileLocation = mod.playerStreamedNoteHoldChunkSprites;
        AiCustomNoteChunkNoteFileLocation = mod.enemyStreamedNoteHoldChunkSprites;

        // hold chunks
        PlayerCustomHoldNoteEndFileLocation = mod.playerStreamedNoteHoldEndSprites;
        AiCustomNoteHoldNoteEndFileLocation = mod.enemyStreamedNoteHoldEndSprites;

        // dunno what the fuck this was for
        AiCustomNoteHeldFileLocation = mod.enemyNoteControllerSprites;

        // music
        mp3FileName = mod.music;

        Instance.SongName = mod.name;
        Instance.SongCreatorName = mod.creator;

        StartCoroutine(LoadMP3());
    }

    private void Start()
    {
        PlayerPrefs.SetFloat("scrollSpeed", 2.8f);

        GatherNeededObjects();
    }

    private void GatherNeededObjects()
    {
        StartCoroutine(GetNotes());
    }

    // wait for the notes to be created
    public bool _loadingNotes = true;
    public bool _loadingAssets = true;
    private IEnumerator GetNotes()
    {
        yield return new WaitForSecondsRealtime(0.01f);

        GameObject[] _streamedNotes = GameObject.FindGameObjectsWithTag("Note");
        GameObject[] _streamedEnemyNotes = GameObject.FindGameObjectsWithTag("EnemyNote");
        GameObject[] holdNoteChunks = GameObject.FindGameObjectsWithTag("Note Hold");
        GameObject[] holdNoteEnds = GameObject.FindGameObjectsWithTag("Note Hold End");

        for (int i = 0; i < _streamedNotes.Length; i++)
        {
            SpriteRenderer spriteRenderer = _streamedNotes[i].GetComponent<SpriteRenderer>();
            _playerStreamedNoteSpriteRenderers.Add(spriteRenderer);
        }
        for (int i = 0; i < _streamedEnemyNotes.Length; i++)
        {
            SpriteRenderer enemySpriteRenderer = _streamedEnemyNotes[i].GetComponent<SpriteRenderer>();
            _enemyStreamedNoteSpriteRenderers.Add(enemySpriteRenderer);
        }

        for (int i = 0; i < holdNoteChunks.Length; i++)
        {
            if (holdNoteChunks[i].transform.parent.name == "Enemy Notes") continue;
            SpriteRenderer holdNoteParenSpriteRenderer = holdNoteChunks[i].GetComponent<SpriteRenderer>();
            _playerHoldNoteChunkRenderers.Add(holdNoteParenSpriteRenderer);
        }
        for (int i = 0; i < holdNoteChunks.Length; i++)
        {
            if (holdNoteChunks[i].transform.parent.name == "Notes") continue;
            SpriteRenderer holdNoteParenSpriteRenderer = holdNoteChunks[i].GetComponent<SpriteRenderer>();
            _enemyHoldNoteChunkRenderers.Add(holdNoteParenSpriteRenderer);
        }

        for (int i = 0; i < holdNoteEnds.Length; i++)
        {
            if (holdNoteEnds[i].transform.parent.name == "Enemy Notes") continue;
            SpriteRenderer holdNoteParenSpriteRenderer = holdNoteEnds[i].GetComponent<SpriteRenderer>();
            _playerHoldNoteEndRenderers.Add(holdNoteParenSpriteRenderer);
        }
        for (int i = 0; i < holdNoteEnds.Length; i++)
        {
            if (holdNoteEnds[i].transform.parent.name == "Notes") continue;
            SpriteRenderer holdNoteParenSpriteRenderer = holdNoteEnds[i].GetComponent<SpriteRenderer>();
            _enemyHoldNoteEndRenderers.Add(holdNoteParenSpriteRenderer);
        }

        yield return new WaitForSecondsRealtime(0.01f);

        _loadingNotes = false;
        LoadCustomAssets();
        Debug.Log($"Notes Created: {_streamedNotes.Length} {_streamedEnemyNotes.Length} {holdNoteChunks.Length} {holdNoteEnds.Length}");
    }

    private void LoadCustomAssets()
    {
        // Note Particle Assets
        switch (_typeNoteParticleAsset)
        {
            case TypeAsset.Custom:
                {
                    string filePath = NormalisePath(Path.Combine(Application.streamingAssetsPath, CustomNoteParticleFileName));
                    _noteParticleAnimation.AddRange(LoadSpritesFromPath(filePath, _particleSpriteWidth, _particleSpriteHeight, 0.5f, 0.5f, true));

                    for (int i = 0; i < _dreamwaveParticles.Count; i++)
                    {
                        _dreamwaveParticles[i].ParticleAnimation.AddRange(_noteParticleAnimation);
                        if (i == _dreamwaveParticles.Count - 1) break;
                    }
                }
                break;
            case TypeAsset.Mod:
                break;
            case TypeAsset.Default:
                break;
        }

        // Note Asset Sprites
        switch (_typeNoteAsset)
        {
            case TypeAsset.Custom:
                // clear default animations for player
                _noteController.noteSpritesDown.Clear();
                _noteController.noteSpritesRelease.Clear();

                // clear default animations for ai
                foreach (DreamwaveAICommunicator aic in _aiNoteControllers)
                {
                    aic._noteSpritesHeld.Clear();
                    aic._noteSpritesReleased.Clear();
                }

                string filePathHeld = NormalisePath(Path.Combine(Application.streamingAssetsPath, PlayerCustomNoteFileLocation, "Held Sprites"));
                string filePathHeldE = NormalisePath(Path.Combine(Application.streamingAssetsPath, AiCustomNoteFileLocation, "Held Sprites"));
                
                string filePathReleased = NormalisePath(Path.Combine(Application.streamingAssetsPath, PlayerCustomNoteFileLocation, "Release Sprites"));
                string filePathReleasedE = NormalisePath(Path.Combine(Application.streamingAssetsPath, AiCustomNoteFileLocation, "Held Sprites"));

                string filePathNoteStreamed = NormalisePath(Path.Combine(Application.streamingAssetsPath, PlayerCustomStreamedNoteFileLocation));
                string filePathNoteStreamedE = NormalisePath(Path.Combine(Application.streamingAssetsPath, AiPlayerCustomStreamedNoteFileLocation));

                string filePathNoteHold = NormalisePath(Path.Combine(Application.streamingAssetsPath, PlayerCustomChunkNoteFileLocation));
                string filePathNoteHoldE = NormalisePath(Path.Combine(Application.streamingAssetsPath, AiCustomNoteChunkNoteFileLocation));

                string filePathNoteEnd = NormalisePath(Path.Combine(Application.streamingAssetsPath, PlayerCustomHoldNoteEndFileLocation));
                string filePathNoteEndE = NormalisePath(Path.Combine(Application.streamingAssetsPath, AiCustomNoteHoldNoteEndFileLocation));

                #region controller notes

                _noteController.noteSpritesDown.AddRange(LoadSpritesFromPath(filePathHeld, _NoteWidth, _NoteHeight, 0.5f, 0.5f, true));
                _noteController.noteSpritesRelease.AddRange(LoadSpritesFromPath(filePathReleased, _NoteWidth, _NoteHeight, 0.5f, 0.5f, true));

                foreach (DreamwaveAICommunicator aic in _aiNoteControllers)
                {
                    aic._noteSpritesHeld.AddRange(LoadSpritesFromPath(filePathHeldE, _NoteWidth, _NoteHeight, 0.5f, 0.5f, true));
                    aic._noteSpritesReleased.AddRange(LoadSpritesFromPath(filePathReleasedE, _NoteWidth, _NoteHeight, 0.5f, 0.5f, true));
                }

                #endregion

                foreach (DreamwaveAICommunicator aic in _aiNoteControllers)
                {
                    aic._spriteRenderer.sprite = aic._noteSpritesReleased[0];
                }

                for (int i = 0; i < _playerNoteSpriteRenderers.Count; i++)
                {
                    _playerNoteSpriteRenderers[i].sprite = _noteController.noteSpritesRelease[_noteController.noteSpritesRelease.Count - 1];
                }

                foreach (SpriteRenderer sr in _playerStreamedNoteSpriteRenderers)
                {
                    sr.sprite = LoadStreamedSprite(filePathNoteStreamed, "0.png", 1080, 1080);
                }

                foreach (SpriteRenderer sr in _enemyStreamedNoteSpriteRenderers)
                {
                    sr.sprite = LoadStreamedSprite(filePathNoteStreamedE, "0.png", 1080, 1080);
                }

                for (int i = 0; i < _playerHoldNoteChunkRenderers.Count; i++)
                {
                    _playerHoldNoteChunkRenderers[i].sprite = LoadStreamedSprite(filePathNoteHold, "0.png", 1080, 1080);
                }
                for (int i = 0; i < _enemyHoldNoteChunkRenderers.Count; i++)
                {
                    _enemyHoldNoteChunkRenderers[i].sprite = LoadStreamedSprite(filePathNoteHoldE, "0.png", 1080, 1080);
                }

                for (int i = 0; i < _playerHoldNoteEndRenderers.Count; i++)
                {
                    _playerHoldNoteEndRenderers[i].sprite = LoadStreamedSprite(filePathNoteEnd, "0.png", 1080, 1080);
                }
                for (int i = 0; i < _enemyHoldNoteEndRenderers.Count; i++)
                {
                    _enemyHoldNoteEndRenderers[i].sprite = LoadStreamedSprite(filePathNoteEndE, "0.png", 1080, 1080);
                }
                break;
        }

        // Player One Assets
        switch (_typePlayerOne)
        {
            case TypeAsset.Custom:
                // Clear all animations and offsets
                _playerOneScript.IdleAnimation.Clear();
                _playerOneScript.IdleOffsets.Clear();
                _playerOneScript.LeftAnimations.Clear();
                _playerOneScript.LeftOffsets.Clear();
                _playerOneScript.DownAnimations.Clear();
                _playerOneScript.DownOffsets.Clear();
                _playerOneScript.UpAnimations.Clear();
                _playerOneScript.UpOffsets.Clear();
                _playerOneScript.RightAnimations.Clear();
                _playerOneScript.RightOffsets.Clear();

                _playerOneIconScript._critical.Clear();
                _playerOneIconScript._criticalOffsets.Clear();
                _playerOneIconScript._losing.Clear();
                _playerOneIconScript._losingOffsets.Clear();
                _playerOneIconScript._neutral.Clear();
                _playerOneIconScript._neutralOffsets.Clear();
                _playerOneIconScript._winning.Clear();
                _playerOneIconScript._winningOffsets.Clear();


                // Load sprite settings
                string settingsPath = NormalisePath(Path.Combine(Application.streamingAssetsPath, CustomPlayerOneFileName, "settings.txt"));
                string[] lines = File.ReadAllLines(settingsPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("spriteWidth="))
                        _playerOneSpriteWidth = int.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("spriteHeight="))
                        _playerOneSpriteHeight = int.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("spriteScale="))
                    {
                        float scale = float.Parse(line.Split("=")[1]);
                        _playerOneScript.transform.localScale = new Vector3(scale, scale, scale);
                    }
                    else if (line.StartsWith("spritePositionX="))
                    {
                        _playerOneScript.gameObject.transform.parent.transform.position = new Vector3(float.Parse(line.Split("=")[1]), 
                            _playerOneScript.gameObject.transform.parent.transform.position.y, 0);
                    }
                    else if (line.StartsWith("spritePositionY="))
                    {
                        _playerOneScript.gameObject.transform.parent.transform.position = new Vector3(_playerOneScript.gameObject.transform.parent.transform.position.x, 
                            float.Parse(line.Split("=")[1]), 0);
                    }
                    else if (line.StartsWith("shouldFlip="))
                        _shouldFlipPlayerOneCustom = bool.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("antiAliasing="))
                        _shouldPlayerOneAntiAlias = bool.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("frameRate="))
                        _playerOneScript.AnimationSpeed = 1 / float.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("holdFrameRate="))
                        _playerOneScript.AnimationHoldSpeed = 1 / float.Parse(line.Split("=")[1]);
                }

                // Define animation names for Player One
                string[] animationNames = { "Idle", "Left", "Down", "Up", "Right" };
                // For each animation, load sprites and offsets from the corresponding subfolder
                foreach (string animationName in animationNames)
                {
                    string animationPath = NormalisePath(Path.Combine(Application.streamingAssetsPath, CustomPlayerOneFileName, animationName));
                    string offsetPath = NormalisePath(Path.Combine(animationPath, "offsets.txt"));
                    var sprites = LoadSpritesFromPath(animationPath, _playerOneSpriteWidth, _playerOneSpriteHeight, _playerOneRectX, _playerOneRectY, _shouldPlayerOneAntiAlias);
                    var offsets = LoadSpriteOffsetsFromPath(offsetPath);

                    switch (animationName)
                    {
                        case "Idle":
                            _playerOneScript.IdleAnimation.AddRange(sprites);
                            _playerOneScript.IdleOffsets.AddRange(offsets);
                            break;
                        case "Left":
                            _playerOneScript.LeftAnimations.AddRange(sprites);
                            _playerOneScript.LeftOffsets.AddRange(offsets);
                            break;
                        case "Down":
                            _playerOneScript.DownAnimations.AddRange(sprites);
                            _playerOneScript.DownOffsets.AddRange(offsets);
                            break;
                        case "Up":
                            _playerOneScript.UpAnimations.AddRange(sprites);
                            _playerOneScript.UpOffsets.AddRange(offsets);
                            break;
                        case "Right":
                            _playerOneScript.RightAnimations.AddRange(sprites);
                            _playerOneScript.RightOffsets.AddRange(offsets);
                            break;
                    }
                }

                // The custom icon loading block is commented out.

                _playerOneScript.Renderer.flipX = _shouldFlipPlayerOneCustom;
                _playerOneScript.Renderer.sprite = _playerOneScript.IdleAnimation[0];
                _playerOneScript.Renderer.transform.localPosition = _playerOneScript.IdleOffsets[0];
                break;
        }

        // Player Two Assets
        switch (_typePlayerTwo)
        {
            case TypeAsset.Custom:
                // Clear all animations and offsets for player two
                _playerTwoAiScript.IdleAnimation.Clear();
                _playerTwoAiScript.IdleOffsets.Clear();
                _playerTwoAiScript.LeftAnimations.Clear();
                _playerTwoAiScript.LeftOffsets.Clear();
                _playerTwoAiScript.DownAnimations.Clear();
                _playerTwoAiScript.DownOffsets.Clear();
                _playerTwoAiScript.UpAnimations.Clear();
                _playerTwoAiScript.UpOffsets.Clear();
                _playerTwoAiScript.RightAnimations.Clear();
                _playerTwoAiScript.RightOffsets.Clear();

                _playerTwoIconScript._critical.Clear();
                _playerTwoIconScript._criticalOffsets.Clear();
                _playerTwoIconScript._losing.Clear();
                _playerTwoIconScript._losingOffsets.Clear();
                _playerTwoIconScript._neutral.Clear();
                _playerTwoIconScript._neutralOffsets.Clear();
                _playerTwoIconScript._winning.Clear();
                _playerTwoIconScript._winningOffsets.Clear();


                string aiSettingsPath = NormalisePath(Path.Combine(Application.streamingAssetsPath, CustomAiPlayerTwoFileName, "settings.txt"));
                string[] aiLines = File.ReadAllLines(aiSettingsPath);
                foreach (string line in aiLines)
                {
                    if (line.StartsWith("spriteWidth="))
                        _playerTwoAiSpriteWidth = int.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("spriteHeight="))
                        _playerTwoAiSpriteHeight = int.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("spriteScale="))
                    {
                        float scale = float.Parse(line.Split("=")[1]);
                        _playerTwoAiScript.transform.localScale = new Vector3(scale, scale, scale);
                    }
                    else if (line.StartsWith("spritePositionX="))
                    {
                        _playerTwoAiScript.gameObject.transform.parent.transform.position = new Vector3(float.Parse(line.Split("=")[1]),
                            _playerTwoAiScript.gameObject.transform.parent.transform.position.y, 0);
                    }
                    else if (line.StartsWith("spritePositionY="))
                    {
                        _playerTwoAiScript.gameObject.transform.parent.transform.position = new Vector3(_playerTwoAiScript.gameObject.transform.parent.transform.position.x,
                            float.Parse(line.Split("=")[1]), 0);
                    }
                    else if (line.StartsWith("shouldFlip="))
                        _shouldFlipPlayerTwoCustom = bool.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("antiAliasing="))
                        _shouldPlayerTwoAntiAlias = bool.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("frameRate="))
                        _playerTwoAiScript.AnimationSpeed = 1 / float.Parse(line.Split("=")[1]);
                    else if (line.StartsWith("holdFrameRate="))
                        _playerTwoAiScript.AnimationHoldSpeed = 1 / float.Parse(line.Split("=")[1]);
                }

                // Define animation names for Player Two
                string[] aiAnimationNames = { "Idle", "Left", "Down", "Up", "Right" };
                foreach (string animationName in aiAnimationNames)
                {
                    string animationPath = NormalisePath(Path.Combine(Application.streamingAssetsPath, CustomAiPlayerTwoFileName, animationName));
                    string offsetPath = NormalisePath(Path.Combine(animationPath, "offsets.txt"));
                    var sprites = LoadSpritesFromPath(animationPath, _playerTwoAiSpriteWidth, _playerTwoAiSpriteHeight, _playerTwoRectX, _playerTwoRectY, _shouldPlayerTwoAntiAlias);
                    var offsets = LoadSpriteOffsetsFromPath(offsetPath);

                    switch (animationName)
                    {
                        case "Idle":
                            _playerTwoAiScript.IdleAnimation.AddRange(sprites);
                            _playerTwoAiScript.IdleOffsets.AddRange(offsets);
                            break;
                        case "Left":
                            _playerTwoAiScript.LeftAnimations.AddRange(sprites);
                            _playerTwoAiScript.LeftOffsets.AddRange(offsets);
                            break;
                        case "Down":
                            _playerTwoAiScript.DownAnimations.AddRange(sprites);
                            _playerTwoAiScript.DownOffsets.AddRange(offsets);
                            break;
                        case "Up":
                            _playerTwoAiScript.UpAnimations.AddRange(sprites);
                            _playerTwoAiScript.UpOffsets.AddRange(offsets);
                            break;
                        case "Right":
                            _playerTwoAiScript.RightAnimations.AddRange(sprites);
                            _playerTwoAiScript.RightOffsets.AddRange(offsets);
                            break;
                    }
                }

                // The custom icon loading block for player two is commented out.

                _playerTwoAiScript.Renderer.flipX = _shouldFlipPlayerTwoCustom;
                _playerTwoAiScript.Renderer.sprite = _playerTwoAiScript.IdleAnimation[0];
                _playerTwoAiScript.Renderer.transform.localPosition = _playerTwoAiScript.IdleOffsets[0];
                break;
        }

        _loadingAssets = false;
        _background.sprite = LoadStreamedSprite(Application.streamingAssetsPath, _backgroundFileName, (1920 / 2), (1080 / 2));
    }

    public Sprite LoadStreamedSprite(string filepath, string fileName, int width, int height)
    {
        string filePath = NormalisePath(Path.Combine(filepath, fileName));

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.LoadImage(fileData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        else
        {
            Debug.LogError("Could not find any file with the given path " + filePath);
            return null;
        }
    }

    public List<Sprite> LoadSpritesFromPath(string filePath, int width, int height, float rectX, float rectY, bool antiAliasing)
    {
        List<Sprite> sprites = new List<Sprite>();
        filePath = NormalisePath(filePath);

        if (Directory.Exists(filePath))
        {
            // Get all .png files in the directory
            string[] imageFiles = Directory.GetFiles(filePath, "*.png");

            foreach (string file in imageFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (int.TryParse(fileName, out int index))
                {
                    byte[] fileData = File.ReadAllBytes(file);
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    if (texture.LoadImage(fileData))
                    {
                        texture.wrapMode = TextureWrapMode.Clamp;
                        texture.filterMode = antiAliasing ? FilterMode.Trilinear : FilterMode.Point;
                        texture.mipMapBias = 0;
                        texture.anisoLevel = 0;
                        texture.Apply();
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(rectX, rectY));
                        sprites.Add(sprite);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Directory does not exist: " + filePath);
        }

        return sprites;
    }

    public List<Vector2> LoadSpriteOffsetsFromPath(string filePath)
    {
        List<Vector2> offsets = new List<Vector2>();
        filePath = NormalisePath(filePath);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (values.Length == 2 &&
                    float.TryParse(values[0].Trim(), out float x) &&
                    float.TryParse(values[1].Trim(), out float y))
                {
                    Vector2 offset = new Vector2(x, y);
                    offsets.Add(offset);
                }
                else
                {
                    Debug.LogWarning("Invalid line in file: " + filePath + " Line: " + line);
                }
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }

        return offsets;
    }

    private IEnumerator LoadMP3()
    {
        string filePath = NormalisePath(Path.Combine(Application.streamingAssetsPath, mp3FileName));
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load MP3: " + www.error);
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            AudioSource audioSource = instance.audioSource;
            if (audioSource != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }
}

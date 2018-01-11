using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIManager : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField]
    GameObject vehicleLoadButtonPrefab;

    [SerializeField]
    GameObject blockButtonPrefab;

    [Space(10)]
    [Header("Save")]
    [SerializeField]
    GameObject SavePanel;

    [SerializeField]
    InputField saveNameInput;

    [SerializeField]
    Button saveButton;

    [SerializeField]
    Button openSavePanelButton;

    [Space(10)]
    [Header("Load")]
    [SerializeField]
    GameObject LoadPanel;

    [SerializeField]
    Button loadButton;

    [SerializeField]
    RectTransform vehicleButtons;

    [Space(10)]
    [Header("Other")]


    [SerializeField]
    Button resetButton;

    [SerializeField]
    Button playButton;

    [SerializeField]
    Button buildButton;

    [SerializeField]
    GameObject blocksButtonsScrollView;

    [SerializeField]
    GameObject blocksButtons;

    bool needToReenableControls = false;

    float controlsTimer = 0.0f;
    float controlsMaxTime = 0.2f;

    private void Start()
    {
        GameManager.OnGameStateChange += GameStateChanged;
        if (BlockDatabase.instance.BlocksInitialised) // if the blocks already have been initialized, create the buttons
            CreateBlocksButtons();
        BlockDatabase.OnBlocksInitialised += CreateBlocksButtons; //Add this event anyway in case we call Load Blocks later in game

        //CreateBlocksButtons(); //Temporary
    }

    void GameStateChanged(GameManager.GameStateEnum state)
    {
        switch(state)
        {
            case GameManager.GameStateEnum.Play:
                DisableEditorUI();
                break;
            case GameManager.GameStateEnum.Editor:
                EnableEditorUI();
                break;
        }
    }

    private void Update()
    {
        
        if (needToReenableControls)
        {
            controlsTimer += Time.unscaledDeltaTime;
            if (controlsTimer > controlsMaxTime)
            {
                needToReenableControls = false;
                ReEnableControls();
                controlsTimer = 0.0f;
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameStateChanged;
        BlockDatabase.OnBlocksInitialised -= CreateBlocksButtons;
    }


    void DisableEditorUI()
    {
        buildButton.gameObject.SetActive(true);

        playButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        vehicleButtons.gameObject.SetActive(false);
        openSavePanelButton.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(false);
        blocksButtonsScrollView.SetActive(false);
    }

    void EnableEditorUI()
    {
        buildButton.gameObject.SetActive(false);

        playButton.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);
        vehicleButtons.gameObject.SetActive(true);
        openSavePanelButton.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(true);
        blocksButtonsScrollView.SetActive(true);
    }

    public void OpenSavePanel()
    {
        SavePanel.gameObject.SetActive(true);
        saveButton.interactable = false;
        saveNameInput.text = string.Empty;
        GameManager.instance.DisableBuildingControls = true;
        GameManager.instance.DisableCameraControls = true;
        UIElement.DisableDisabling = true;
    }

    public void CloseSavePanel()
    {
        GameManager.instance.DisableCameraControls = false;
        needToReenableControls = true;
        SavePanel.SetActive(false);
    }

    void ReEnableControls()
    {
        GameManager.instance.DisableBuildingControls = false;
        UIElement.DisableDisabling = false;
    }

    public void OnSaveNameInputValueChanged()
    {
        if(saveNameInput.text.Length < 3)
        {
            saveButton.interactable = false;
        }
        else
        {
            saveButton.interactable = true;
        }
    }

    public void SaveWithInputName()
    {
        GameManager.instance.SaveVehicle(Directories.VEHICLE_SAVE_DIRECTORY + saveNameInput.text + ".sav");
    }

    public void OpenLoadPanel()
    {
        UpdateLoadPanel();
        LoadPanel.SetActive(true);
        GameManager.instance.DisableBuildingControls = true;
        GameManager.instance.DisableCameraControls = true;
        UIElement.DisableDisabling = true;
    }

    public void CloseLoadPanel()
    {
        GameManager.instance.DisableCameraControls = false;
        needToReenableControls = true;
        LoadPanel.SetActive(false);
    }

    public void UpdateLoadPanel()
    {
        while(vehicleButtons.childCount > 0)
        {
            DestroyImmediate(vehicleButtons.GetChild(0).gameObject);
        }
        string[] vehicleFiles = Directory.GetFiles(Directories.VEHICLE_SAVE_DIRECTORY);
        foreach(string str in vehicleFiles)
        {
            if (!str.EndsWith(".sav"))
                continue;
            string vehicleName = str.Replace(Directories.VEHICLE_SAVE_DIRECTORY, "");
            vehicleName = vehicleName.Replace(".sav", "");
            Button vehicleButton = Instantiate(vehicleLoadButtonPrefab, vehicleButtons).GetComponent<Button>();
            vehicleButton.onClick.RemoveAllListeners();
            vehicleButton.onClick.AddListener(() => GameManager.instance.LoadVehicle(str));
            vehicleButton.onClick.AddListener(() => CloseLoadPanel());
            vehicleButton.GetComponentInChildren<Text>().text = vehicleName;
        }
    }


    public void CreateBlocksButtons()
    {
        while (blocksButtons.transform.childCount > 0)
        {
            DestroyImmediate(blocksButtons.transform.GetChild(0).gameObject);
        }

        for(int i = 0; i < BlockDatabase.instance.Blocks.Length; i++)
        {
            //Ignore Core block
            if (BlockDatabase.instance.Blocks[i].GetComponent<CoreBlock>())
                continue;
            Button blockButton = Instantiate(blockButtonPrefab, blocksButtons.transform).GetComponent<Button>();
            Block block = BlockDatabase.instance.Blocks[i].GetComponent<Block>();
            blockButton.onClick.RemoveAllListeners();
            blockButton.onClick.AddListener(() => GameManager.instance.SetSelectedBlockFromID(block.Id));
            blockButton.GetComponentInChildren<Text>().text = block.BlockName;
            blockButton.GetComponent<Image>().sprite = block.Image;
        }
    }
}

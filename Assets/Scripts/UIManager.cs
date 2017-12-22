using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIManager : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField]
    GameObject vehicleLoadButtonPrefab;

    [Space(10)]
    [Header("Save")]
    [SerializeField]
    GameObject SavePanel;

    [SerializeField]
    InputField saveNameInput;

    [SerializeField]
    Button saveButton;

    [Space(10)]
    [Header("Load")]
    [SerializeField]
    GameObject LoadPanel;

    [SerializeField]
    RectTransform vehicleButtons;

    bool needToReenableControls = false;

    float controlsTimer = 0.0f;
    float controlsMaxTime = 0.2f;

    private void Start()
    {
        
    }

    private void Update()
    {
        controlsTimer += Time.unscaledDeltaTime;
        if (needToReenableControls)
        {
            if(controlsTimer > controlsMaxTime)
            {
                needToReenableControls = false;
                ReEnableControls();
                controlsTimer = 0.0f;
            }
        }
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

}

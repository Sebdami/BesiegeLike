using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetPreviewImageSaver : EditorWindow
{
    Object selectedAsset;
    [MenuItem("Window/Asset Preview Image Saver")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssetPreviewImageSaver));
    }

    void OnGUI()
    {
        selectedAsset = EditorGUILayout.ObjectField("Asset", selectedAsset, typeof(Object), false);
        if (selectedAsset == null)
            GUI.enabled = false;
        if(GUILayout.Button("Create Image"))
        {
            CreateAssetImage();
        }
    }

    void CreateAssetImage()
    {
        string path = EditorUtility.SaveFilePanel("Save Asset Preview Image", "Assets/", name, "png");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);

        Texture2D img = AssetPreview.GetAssetPreview(selectedAsset);
        byte[] pngImg = img.EncodeToPNG();
        
        FileStream fs = File.Create(path);
        fs.Write(pngImg, 0, pngImg.Length);
        fs.Close();
        AssetDatabase.Refresh();
    }
}

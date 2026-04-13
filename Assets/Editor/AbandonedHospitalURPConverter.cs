using UnityEditor;
using UnityEngine;
using System.IO;

public class AbandonedHospitalURPConverter : EditorWindow
{
    private static string sourcePackagePath = "Assets/HE_HDRP_AbandonedHospital/URP/URP_HE_AbandonedHospital.unitypackage";
    private static string targetPackagePath = "Assets/URP_HE_AbandonedHospital.unitypackage";
    private static string hdrpDirectory = "Assets/HE_HDRP_AbandonedHospital";

    [MenuItem("Tools/Abandoned Hospital URP Converter/Convert")]
    public static void Convert()
    {
        // 1. Check if package exists
        if (!File.Exists(sourcePackagePath))
        {
            Debug.LogError($"[URP Converter] Source package not found at: {sourcePackagePath}");
            return;
        }

        // 2. Move package outside HDRP directory
        Debug.Log("[URP Converter] Moving URP package to root...");
        string error = AssetDatabase.MoveAsset(sourcePackagePath, targetPackagePath);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError($"[URP Converter] Failed to move package: {error}");
            return;
        }

        // 3. Delete HDRP directory
        Debug.Log("[URP Converter] Deleting HDRP assets...");
        if (AssetDatabase.DeleteAsset(hdrpDirectory))
        {
            Debug.Log("[URP Converter] HDRP directory deleted successfully.");
        }
        else
        {
            Debug.LogError("[URP Converter] Failed to delete HDRP directory.");
        }

        // 4. Trigger import
        Debug.Log("[URP Converter] Triggering URP package import...");
        AssetDatabase.ImportPackage(targetPackagePath, true);
        
        Debug.Log("[URP Converter] Process complete. After import is finished, please delete the .unitypackage in Assets root.");
    }
}

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class AddressableKeyRenamer : MonoBehaviour
{
    [MenuItem("Tools/Addressables/키를 파일이름으로 자동 설정")]
    public static void RenameKeys()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                string path = AssetDatabase.GUIDToAssetPath(entry.guid);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.SetAddress(fileName);  // 파일 이름으로 주소 변경
                Debug.Log($"변경됨: {entry.address}");
            }
        }
        AssetDatabase.SaveAssets(); 
        Debug.Log("Addressable 키 이름 일괄 수정 완료!");
    }
}
#endif
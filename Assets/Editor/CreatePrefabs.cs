using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreatePrefabs : MonoBehaviour
{
    //Creates a new menu (Examples) with a menu item (Create Prefab)
    [MenuItem("Tools/Create Prefab")]
    static void CreatePrefab()
    {
        //Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        //Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            //Set the path as within the Assets folder, and name it as the GameObject's name with the .prefab format
            string localPath = "Assets/Prefabs/" + gameObject.name + "_generated.prefab";

            //Check if the Prefab and/or name already exists at the path
            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                //Create dialog to ask if User is sure they want to overwrite existing Prefab
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "The Prefab already exists. Do you want to overwrite it?",
                    "Yes",
                    "No"))
                //If the user presses the yes button, create the Prefab
                {
                    CreateNew(gameObject, localPath);
                }
            }
            //If the name doesn't exist, create the new Prefab
            else
            {
                Debug.Log(gameObject.name + " is not a Prefab, will convert");
                CreateNew(gameObject, localPath);
            }
        }
    }

    // Disable the menu item if no selection is in place
    [MenuItem("CreatePrefab/Create Prefab", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null;
    }

    static void CreateNew(GameObject obj, string localPath)
    {
        //Create a new Prefab at the path given
        Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }
}

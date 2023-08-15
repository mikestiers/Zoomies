using UnityEngine;
using UnityEditor;

public class SpreadObjects : ScriptableObject
{
    [MenuItem("MyTools/Spread Selected Objects")]
    static void SpreadSelected()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length < 2)
            return;

        float fixedDistance = 100.0f; // Set this to your desired distance
        Vector3 startPos = selectedObjects[0].transform.position;

        for (int i = 1; i < selectedObjects.Length; i++)
        {
            Vector3 newPos = new Vector3(startPos.x + fixedDistance * i, startPos.y, startPos.z);
            selectedObjects[i].transform.position = newPos;
        }
    }
}

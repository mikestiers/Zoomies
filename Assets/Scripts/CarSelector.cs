using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelector : MonoBehaviour
{
    public Transform carsParent; // Drag the parent object of cars here
    public float slideSpeed = 3.0f;
    public float rotationSpeed = 150.0f;
    private int selectedIndex = 0; // Index of currently selected car

    void Update()
    {
        // Rotate all cars
        foreach (Transform car in carsParent)
        {
            car.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        // Slide between cars
        if (Input.GetKeyDown(KeyCode.RightArrow) && selectedIndex < carsParent.childCount - 1)
        {
            selectedIndex++;
            Vector3 targetPosition = -carsParent.GetChild(selectedIndex).localPosition;
            StopAllCoroutines(); // Stop any ongoing slide coroutines
            StartCoroutine(SlideToPosition(targetPosition));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && selectedIndex > 0)
        {
            selectedIndex--;
            Vector3 targetPosition = -carsParent.GetChild(selectedIndex).localPosition;
            StopAllCoroutines(); // Stop any ongoing slide coroutines
            StartCoroutine(SlideToPosition(targetPosition));
        }
    }

    IEnumerator SlideToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(carsParent.localPosition, targetPosition) > 0.05f)
        {
            carsParent.localPosition = Vector3.Lerp(carsParent.localPosition, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        carsParent.localPosition = targetPosition; // Ensure exact position
    }
}


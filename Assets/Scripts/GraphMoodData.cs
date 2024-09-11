using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphMoodData : MonoBehaviour
{
    public GameObject dotPrefab;  // The prefab to represent each mood data point (can be a button or UI image)
    public RectTransform graphContainer;  // The container where the dots will be instantiated
    public LineRenderer lineRenderer;  // Reference to the LineRenderer

    [SerializeField] private float ySpacing = 50f;  // Spacing on the Y-axis for mood values (scale the Y position)

    private List<RectTransform> pointsList = new List<RectTransform>();  // To keep track of instantiated points
    private string filePath;

    private void Awake()
    {
        // Set the file path for the JSON file
        filePath = System.IO.Path.Combine(Application.persistentDataPath, "moodData.json");
    }

    // Method to be assigned to the Button (now it takes no parameters)
    public void GenerateGraph()
    {
        // Load the mood values from the JSON file
        List<MoodData> moodValues = LoadMoodDataFromJson();

        // Generate the graph with the loaded mood data
        if (moodValues != null)
        {
            CreateGraph(moodValues);
        }
    }


    // Draws the graph using mood values and centers the points
    private void CreateGraph(List<MoodData> moodValues)
    {
        ClearExistingGraph();  // Clear any previous dots/lines

        // Limit the number of points to 5 (or fewer if there aren't enough data points)
        int pointsToShow = Mathf.Min(moodValues.Count, 5);

        // Calculate buffer and adjust the available width for points
        float buffer = 10f;  // 10-point buffer from the edges
        float containerWidth = graphContainer.rect.width - (2 * buffer);  // Subtract 20 points for buffer (10 on each side)

        // Calculate dynamic spacing based on the adjusted container width and the number of points
        float dynamicXSpacing = containerWidth / (pointsToShow - 1);  // Divide by (n-1) for full span

        Debug.Log($"Container Width: {containerWidth}, Dynamic X Spacing: {dynamicXSpacing}");

        // Calculate starting X position (with buffer from the left edge)
        float totalPointsWidth = (pointsToShow - 1) * dynamicXSpacing;
        float startingXPos = -(totalPointsWidth / 2);  // Add buffer to avoid placing points at the edge

        Debug.Log($"Total Points Width: {totalPointsWidth}, Starting X Pos: {startingXPos}");

        float xPos = startingXPos;

        for (int i = 0; i < pointsToShow; i++)
        {
            MoodData moodData = moodValues[i];

            // Instantiate a dot prefab for each mood data point
            GameObject dot = Instantiate(dotPrefab, graphContainer);
            RectTransform dotRectTransform = dot.GetComponent<RectTransform>();

            // Calculate Y position based on the mood data (sad = below 0, surprised = above 0, neutral = 0)
            float yPos = GetMoodYPosition(moodData) * ySpacing;

            // Debug statement to show the Y position of each point
            Debug.Log($"Point {i + 1} - Day: {moodData.day}, Y Position: {yPos}, X Position: {xPos}");

            // Set the position of the dot
            dotRectTransform.anchoredPosition = new Vector2(xPos, yPos);

            // Add text to display the mood and value
            // Assuming the TextMeshPro component is a child of the dot prefab
            TextMeshProUGUI moodValueText = dot.GetComponentInChildren<TextMeshProUGUI>();
            if (moodValueText != null)
            {
                // Set the mood and value text (adjust this format as needed)
                string moodType = GetMoodType(moodData);  // Create a method to determine the mood type (explained below)
                float moodValue = Mathf.Max(moodData.sadValue, moodData.surprisedValue, moodData.neutralValue);
                moodValueText.text = $"{moodType}: {moodValue:F2}";  // Example: "Sad: 0.85"
            }

            // Store the position for connecting with LineRenderer
            pointsList.Add(dotRectTransform);

            // Move to the next X position based on dynamic spacing
            xPos += dynamicXSpacing;
        }

        // Connect the dots using LineRenderer
        ConnectDotsWithLine();
    }

    // Method to determine the mood type based on the highest value
    private string GetMoodType(MoodData moodData)
    {
        if (moodData.sadValue > moodData.surprisedValue && moodData.sadValue > moodData.neutralValue)
        {
            return "Sad";
        }
        else if (moodData.surprisedValue > moodData.sadValue && moodData.surprisedValue > moodData.neutralValue)
        {
            return "Surprised";
        }
        else
        {
            return "Neutral";
        }
    }

    // Calculate the Y position based on the highest mood value (sad below 0, neutral at 0, surprised above 0)
    private float GetMoodYPosition(MoodData moodData)
    {
        // Prioritize sadness or surprised if they are above 0.8
        if (moodData.sadValue > 0.8f)
        {
            return -moodData.sadValue;  // Sad values should be negative on the graph
        }
        else if (moodData.surprisedValue > 0.8f)
        {
            return moodData.surprisedValue;  // Surprised values should be positive on the graph
        }

        // Otherwise, find the highest value between sad, surprised, and neutral
        float highestValue = Mathf.Max(moodData.sadValue, moodData.surprisedValue, moodData.neutralValue);

        // If the highest value is sadness, plot it negatively
        if (moodData.sadValue == highestValue && highestValue > 0.6f)
        {
            return -moodData.sadValue;  // Sad values should be negative on the graph
        }
        // If the highest value is surprised, plot it positively
        else if (moodData.surprisedValue == highestValue && highestValue > 0.6f)
        {
            return moodData.surprisedValue;  // Surprised values should be positive on the graph
        }
        // Otherwise, plot neutral or zero values at 0 (neutral state)
        else
        {
            return 0f;  // Neutral values are at 0
        }
    }

    // Method to connect the dots with a LineRenderer
    private void ConnectDotsWithLine()
    {
        lineRenderer.positionCount = pointsList.Count;
        Vector3[] positions = new Vector3[pointsList.Count];

        for (int i = 0; i < pointsList.Count; i++)
        {
            positions[i] = pointsList[i].anchoredPosition;  // Convert RectTransform to Vector3 for LineRenderer
        }

        lineRenderer.SetPositions(positions);  // Set the positions for the LineRenderer
    }

    // Method to clear the existing graph (dots and line)
    private void ClearExistingGraph()
    {
        // Clear all the previously instantiated dots
        foreach (RectTransform point in pointsList)
        {
            Destroy(point.gameObject);
        }
        pointsList.Clear();

        // Clear the LineRenderer
        lineRenderer.positionCount = 0;
    }

    // Load mood values from the JSON file
    private List<MoodData> LoadMoodDataFromJson()
    {
        if (System.IO.File.Exists(filePath))
        {
            string jsonData = System.IO.File.ReadAllText(filePath);
            MoodDataContainer moodDataContainer = JsonUtility.FromJson<MoodDataContainer>(jsonData);

            // Get the most recent 5 days (or fewer if not enough data)
            int totalEntries = moodDataContainer.moodDataList.Count;
            int entriesToLoad = Mathf.Min(totalEntries, 5);  // Only load up to 5 entries

            // Get the last 'entriesToLoad' from the list
            List<MoodData> mostRecentMoodData = moodDataContainer.moodDataList.GetRange(totalEntries - entriesToLoad, entriesToLoad);

            return mostRecentMoodData;
        }
        else
        {
            Debug.LogWarning("No mood data found.");
            return null;  // Return null if no data is found
        }
    }
}
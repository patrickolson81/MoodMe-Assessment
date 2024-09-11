using System.Collections;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace
using MoodMe;
using System.IO;



public class TrackEmotions : MonoBehaviour
{
    private MoodData moodData;
    private string filePath;


    // Variables to store the highest values of each emotion
    private float maxSad = 0f;
    private float maxSurprised = 0f;
    private float maxNeutral = 0f;

    public float trackDuration = 5f;  // Duration to track emotions, default is 5 seconds
    private const float maxSadValue = 0.5f;  // Scale sadness so 0.5 is considered the maximum

    // Reference to a TextMeshProUGUI element to display the mood
    public TextMeshProUGUI moodText;  // Use TextMeshProUGUI for TextMeshPro

    // Method to start tracking emotions (called from a button)

    void Awake()
    {
        // Set the file path for the JSON file
        filePath = Path.Combine(Application.persistentDataPath, "moodData.json");
    }
    public void StartTrackingEmotions()
    {
        StartCoroutine(TrackEmotionsCoroutine());
    }

    // Coroutine that tracks emotions over the specified duration
    private IEnumerator TrackEmotionsCoroutine()
    {
        float elapsedTime = 0f;

        // Reset the max values at the beginning of each tracking session
        maxSad = 0f;
        maxSurprised = 0f;
        maxNeutral = 0f;


        // Track the emotions for the specified duration
        while (elapsedTime < trackDuration)
        {
            elapsedTime += Time.deltaTime;

            // Get the current emotion values from EmotionsManager
            float currentSadRaw = EmotionsManager.Emotions.sad;  // Non-scaled sadness value
            float currentSadScaled = currentSadRaw / maxSadValue;  // Scaled sadness so 0.4 becomes 1
            float currentSurprised = EmotionsManager.Emotions.surprised;
            float currentNeutral = EmotionsManager.Emotions.neutral;

            // Debug statement to track the non-scaled and scaled sadness values
            Debug.Log($"Non-Scaled Sadness Value: {currentSadRaw:F2}");
            Debug.Log($"Scaled Sadness Value: {currentSadScaled:F2}");

            // Update the maximum values
            if (currentSadScaled > maxSad)
            {
                maxSad = currentSadScaled;
            }

            if (currentSurprised > maxSurprised)
            {
                maxSurprised = currentSurprised;
            }

            if (currentNeutral > maxNeutral)
            {
                maxNeutral = currentNeutral;
            }

            // Wait until the next frame
            yield return null;
        }

        // After tracking is done, log the max values
        Debug.Log("Tracking completed.");
        Debug.Log("Max Sad: " + maxSad);
        Debug.Log("Max Surprised: " + maxSurprised);
        Debug.Log("Max Neutral: " + maxNeutral);

        // Analyze mood for the day based on recorded max values
        string moodResult = DetermineMoodForTheDay();
        UpdateMoodText(moodResult);
    }

    // Method to determine the mood for the day
    private string DetermineMoodForTheDay()
    {
        string moodResult = "neutral";  // Default mood is neutral

        // Check if Sadness or Surprised exceeds 0.5, prioritize it
        if (maxSad > 0.5f && maxSad > maxSurprised)
        {
            moodResult = "bad";  // Sadness takes priority if scaled sadness > 0.5
        }
        else if (maxSurprised > 0.5f && maxSurprised > maxSad)
        {
            moodResult = "good";  // Surprised takes priority if > 0.5
        }
        else if (maxNeutral < 0.5)  // If no emotion exceeds 0.5, check based on lower values
        {
            // Prioritize Surprised and Sadness if they exceed the neutral value
            if (maxSad > maxSurprised && maxSad > maxNeutral)
            {
                moodResult = "bad";  // Sadness takes priority
            }
            else if (maxSurprised > maxSad && maxSurprised > maxNeutral)
            {
                moodResult = "good";  // Surprised takes priority
            }
        }

        Debug.Log("Mood for the day: " + moodResult);
        SaveMood(maxSad, maxSurprised, maxNeutral);

        return moodResult;
    }

    private void SaveMood(float sad, float surprised, float neutral)
    {

        // Add new mood data for the current day
        string day = System.DateTime.Now.DayOfWeek.ToString();
        MoodData newMoodData = new MoodData(day, sad, surprised, neutral);

        // Load the current mood data from the JSON file
        MoodDataContainer moodDataContainer = new MoodDataContainer();
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            moodDataContainer = JsonUtility.FromJson<MoodDataContainer>(jsonData);
        }

        // Add the new mood data
        moodDataContainer.moodDataList.Add(newMoodData);

        // Save the updated data back to the JSON file
        string updatedJson = JsonUtility.ToJson(moodDataContainer, true);
        File.WriteAllText(filePath, updatedJson);
        Debug.Log("Mood data saved to JSON: " + updatedJson);
    }

    // Method to update the TextMeshProUGUI to display the mood
    private void UpdateMoodText(string mood)
    {
        // Update the TextMeshProUGUI element with the mood message
        if (moodText != null)
        {
            moodText.text = "Your Mood Today is: " + mood;
        }
        else
        {
            Debug.LogWarning("Mood Text UI element is not assigned.");
        }
    }
}
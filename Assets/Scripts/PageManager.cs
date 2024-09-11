using UnityEngine;

public class PageManager : MonoBehaviour
{
    // An array of GameObjects representing different pages
    public GameObject[] pages;

    // Method to show a specific page by index and hide others
    private void Start()
    {
        ShowPage(0);
    }
    public void ShowPage(int pageIndex)
    {
        // Loop through all pages
        for (int i = 0; i < pages.Length; i++)
        {
            // Set the selected page active, and others inactive
            if (i == pageIndex)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }
}

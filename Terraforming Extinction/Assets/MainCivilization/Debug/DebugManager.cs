using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;

    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Text debugText; // UnityEngine.UI.Text (not TMPro)

    private Dictionary<string, string> identityDebugValues = new Dictionary<string, string>();
    private Dictionary<string, float> identityDebugTimestamps = new Dictionary<string, float>();

    private Dictionary<string, string> actionSelectionDebugValues = new Dictionary<string, string>();
    private Dictionary<string, float> actionSelectionDebugTimestamps = new Dictionary<string, float>();

    private const float staleThreshold = 5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
        }

        if (debugPanel.activeSelf)
        {
            string combined = "";

            combined += "=== Identity Debug ===\n";
            foreach (var kvp in identityDebugValues)
            {
                string tag = Time.time - identityDebugTimestamps[kvp.Key] > staleThreshold ? " [OLD]" : "";
                combined += $"{kvp.Key}: {kvp.Value}{tag}\n";
            }

            combined += "\n=== Action Selection Debug ===\n";
            foreach (var kvp in actionSelectionDebugValues)
            {
                string tag = Time.time - actionSelectionDebugTimestamps[kvp.Key] > staleThreshold ? " [OLD]" : "";
                combined += $"{kvp.Key}: {kvp.Value}{tag}\n";
            }

            debugText.text = combined;
        }
    }

    public void SetIdentityDebugValue(string label, object value)
    {
        identityDebugValues[label] = value.ToString();
        identityDebugTimestamps[label] = Time.time;
    }

    public void RemoveIdentityDebugValue(string label)
    {
        identityDebugValues.Remove(label);
        identityDebugTimestamps.Remove(label);
    }

    public void SetActionSelectionDebugValue(string label, object value)
    {
        actionSelectionDebugValues[label] = value.ToString();
        actionSelectionDebugTimestamps[label] = Time.time;
    }

    public void RemoveActionSelectionDebugValue(string label)
    {
        actionSelectionDebugValues.Remove(label);
        actionSelectionDebugTimestamps.Remove(label);
    }
}

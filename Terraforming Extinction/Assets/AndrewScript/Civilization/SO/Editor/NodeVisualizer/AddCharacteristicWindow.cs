using UnityEditor;
using UnityEngine;

public class AddCharacteristicWindow : EditorWindow
{
    private SubIdentifier subIdentifier;
    private int characteristicTypeIndex = 0; // 0 = Action, 1 = Appearance
    private string[] characteristicTypes = { "Action", "Appearance" };

    // Action and Appearance characteristics selection
    private int actionCharacteristicIndex = 0;
    private int appearanceCharacteristicIndex = 0;
    private string[] actionCharacteristics = System.Enum.GetNames(typeof(EnumActionCharacteristics));
    private string[] appearanceCharacteristics = System.Enum.GetNames(typeof(EnumAppearanceCharacteristics));

    // Field to input the characteristic value
    private string valueInput = "";

    public static void Open(SubIdentifier subIdentifier)
    {
        AddCharacteristicWindow window = GetWindow<AddCharacteristicWindow>("Add Characteristic");
        window.subIdentifier = subIdentifier;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Characteristic", EditorStyles.boldLabel);

        // Dropdown for selecting characteristic type
        characteristicTypeIndex = EditorGUILayout.Popup("Characteristic Type:", characteristicTypeIndex, characteristicTypes);

        // Show appropriate dropdown based on selection
        if (characteristicTypeIndex == 0) // Action
        {
            actionCharacteristicIndex = EditorGUILayout.Popup("Select Action Characteristic:", actionCharacteristicIndex, actionCharacteristics);
        }
        else if (characteristicTypeIndex == 1) // Appearance
        {
            appearanceCharacteristicIndex = EditorGUILayout.Popup("Select Appearance Characteristic:", appearanceCharacteristicIndex, appearanceCharacteristics);
        }

        // Input field for characteristic value
        valueInput = EditorGUILayout.TextField("Enter Characteristic Value:", valueInput);

        if (GUILayout.Button("Add"))
        {
            if (int.TryParse(valueInput, out int value))
            {
                if (characteristicTypeIndex == 0) // Action
                {
                    var newActionCharacteristic = new ActionCharacteristicWithValue(
                        (EnumActionCharacteristics)actionCharacteristicIndex, value);
                    subIdentifier.ActionCharacteristicsWithValue.Add(newActionCharacteristic);
                }
                else if (characteristicTypeIndex == 1) // Appearance
                {
                    var newAppearanceCharacteristic = new AppearanceCharacteristicWithValue(
                        (EnumAppearanceCharacteristics)appearanceCharacteristicIndex, value);
                    subIdentifier.AppearanceCharacteristicsWithValue.Add(newAppearanceCharacteristic);
                }

                //EditorUtility.SetDirty(FindParentUnityObject(subIdentifier));
                Close(); // Close the window after adding
            }
            else
            {
                Debug.LogError("Invalid input for characteristic value. Please enter a valid integer.");
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }

}

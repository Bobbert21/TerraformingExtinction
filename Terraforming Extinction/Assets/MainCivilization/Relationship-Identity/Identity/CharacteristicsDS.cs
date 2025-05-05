using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Characteristics
{
    None,
    Actions,
    Appearances,
    Count
}

public enum Objects
{
    None,
    Identifiers,
    Characteristics,
    Goals,
    Count
}

public enum EnumActionCharacteristics
{
    None,
    Fight,
    Flee,
    Befriend,
    Gossip,
    Count
}

public enum EnumAppearanceCharacteristics
{
    None,
    Tall,
    Short,
    HairColor,
    Count
}


// Change to an interface
public interface ICharacteristics
{
    Characteristics GeneralCharacteristic { get; set; }
    object MainCharacteristic { get; set; }
}

// Change ActionCharacteristic to a class
[System.Serializable]
public class ActionCharacteristic
{
    public Characteristics GeneralCharacteristic { get; set; }

    // This is specific to ActionCharacteristic
    public EnumActionCharacteristics LocalMainCharacteristic { get; set; }

    // Explicit interface implementation for MainCharacteristic
    public object MainCharacteristic
    {
        get { return LocalMainCharacteristic; }
        set
        {
            if (value is EnumActionCharacteristics actionCharacteristic)
            {
                LocalMainCharacteristic = actionCharacteristic;
            }
            else
            {
                throw new System.InvalidCastException("Invalid type assigned to MainCharacteristic.");
            }
        }
    }
}

// Change AppearanceCharacteristic to a class
[System.Serializable]
public class AppearanceCharacteristic
{
    public Characteristics GeneralCharacteristic { get; set; }

    // This is specific to AppearanceCharacteristic
    public EnumAppearanceCharacteristics LocalMainCharacteristic { get; set; }

    // Explicit interface implementation for MainCharacteristic
    public object MainCharacteristic
    {
        get { return LocalMainCharacteristic; }
        set
        {
            if (value is EnumAppearanceCharacteristics appearanceCharacteristic)
            {
                LocalMainCharacteristic = appearanceCharacteristic;
            }
            else
            {
                throw new System.InvalidCastException("Invalid type assigned to MainCharacteristic.");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identifier : MonoBehaviour
{
    // Outward: How others perceive their identity
    public IdentifierSO OutwardIdentity;
    // Inward: How the character perceives its own identity
    public IdentifierSO InwardIdentity;

    [HideInInspector]
    public List<EnumIdentifiers> OutwardIdentifiers = new();
    [HideInInspector]
    public List<EnumIdentifiers> InwardIdentifiers = new();
    void Start()
    {
        OutwardIdentifiers = OutwardIdentity.identifiers;
        InwardIdentifiers = InwardIdentity.identifiers;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

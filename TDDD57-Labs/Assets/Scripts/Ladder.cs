using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public EntryPoint entryPointBoat;
    public Transform intermediateInside;
    public Transform intermediateOutside;
    public EntryPoint entryPointWater;

    // Start is called before the first frame update
    void Start()
    {
        entryPointBoat.onEntryFunc = OnEntryBoat;
        entryPointWater.onEntryFunc = OnEntryWater;
    }
    
    void OnEntryBoat(Collider chara)
    {
        Character character = chara.gameObject.GetComponent<Character>();
        if (character != null)
        {
            if (!character.IsClimbing())
            {
                character.ClimbLadder(intermediateInside, intermediateOutside, entryPointWater.transform, Character.ClimbDirection.DOWN);
            }
        }
    }

    void OnEntryWater(Collider chara)
    {
        Character character = chara.gameObject.GetComponent<Character>();
        if (character != null)
        {
            if (!character.IsClimbing())
            {
                character.ClimbLadder(intermediateOutside, intermediateInside, entryPointBoat.transform, Character.ClimbDirection.UP);
            }
        }
    }
}
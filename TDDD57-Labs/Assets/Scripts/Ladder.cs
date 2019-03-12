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
        Crewman crewman = chara.gameObject.GetComponent<Crewman>();
        if (crewman != null)
        {
            if (!crewman.IsClimbing())
            {
                crewman.ClimbLadder(intermediateInside, intermediateOutside, entryPointWater.transform, Crewman.ClimbDirection.DOWN);
            }
        }
    }

    void OnEntryWater(Collider chara)
    {
        Crewman crewman = chara.gameObject.GetComponent<Crewman>();
        if (crewman != null)
        {
            ClimbUp(crewman);
        }
    }

    public void ClimbUp(Crewman crewman)
    {
        if (!crewman.IsClimbing())
        {
            crewman.ClimbLadder(intermediateOutside, intermediateInside, entryPointBoat.transform, Crewman.ClimbDirection.UP);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideable : MonoBehaviour
{
    public List<Renderer> renderers;

    public void SetVisible(bool flag)
    {
        for (int i = 0; i < renderers.Count; ++i)
        {
            renderers[i].enabled = flag;
        }
    }

    public bool IsVisible()
    {
        if (renderers.Count < 1)
        {
            return false;
        }
        else
        {
            return renderers[0].enabled;
        }
    }
}

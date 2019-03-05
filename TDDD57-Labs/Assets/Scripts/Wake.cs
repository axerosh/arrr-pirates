using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wake : MonoBehaviour
{
    private float lifeTime = 10.0f;
    private float setupTime = 0.5f;

    private SpriteRenderer r;

    private float alphaSetupPerSecond;
    private float alphaDecreasePerSecond;
    private Vector3 scaleIncreasePerSecond;

    private float passedTime = 0.0f;

    void Start()
    {
        r = GetComponent<SpriteRenderer>();

        float initalAlpha = r.color.a;
        alphaSetupPerSecond = initalAlpha / setupTime;
        alphaDecreasePerSecond = initalAlpha / (lifeTime - setupTime);

        Vector3 initalScale = transform.localScale;
        scaleIncreasePerSecond = 1.5f * initalScale / lifeTime;

        setAlpha(0.0f);
    }
    
    void Update()
    {
        transform.localScale = transform.localScale + scaleIncreasePerSecond * Time.deltaTime;

        passedTime += Time.deltaTime;
        if (passedTime < setupTime)
        {
            setAlpha(r.color.a + alphaSetupPerSecond * Time.deltaTime);
        }
        else if (passedTime < lifeTime)
        {
            setAlpha(r.color.a - alphaDecreasePerSecond * Time.deltaTime);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void setAlpha(float alpha)
    {
        r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
    }
}

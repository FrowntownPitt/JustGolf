using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowerIcon : MonoBehaviour {

    RawImage image;

    public Communication communication;

    public float scale;

    float prevTime;
    public float pulse;

    Vector2 stepperRange = new Vector2(0, 1000);
    Vector2 angleRange = new Vector2(0, (int)(2*Mathf.PI*1000));

	// Use this for initialization
	void Start () {
        pulse = communication.pulse;
        image = GetComponent<RawImage>();	
	}
	
	// Update is called once per frame
	void Update () {


        Vector2 mpos = Input.mousePosition;
        Vector2 delta = new Vector2(mpos.x - Screen.width / 2, mpos.y - Screen.height / 2);

        Vector2 pos = delta.normalized * scale;

        image.rectTransform.position = pos + new Vector2(Screen.width / 2, Screen.height / 2);

        if (Time.time - prevTime > pulse)
        {
            prevTime = Time.time;
            float angle = Mathf.Atan2(pos.y, pos.x);

            if (angle < 0)
            {
                angle = Mathf.PI + Mathf.PI + angle;
            }

            int target = (int)(angle * 1000);
            target = ToRange(target);

            //Debug.Log("Angle: " + target);

            if (communication != null)
            {
                communication.WriteToArduino("TARGET " + target);
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            communication.TryReset();
        }
	}

    private int ToRange(float angle)
    {
        return (int)((angle - angleRange.x) * (stepperRange.y - stepperRange.x) / (angleRange.y - angleRange.x) + stepperRange.x);
    }


}

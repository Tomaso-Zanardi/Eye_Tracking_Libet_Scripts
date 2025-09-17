using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class within : MonoBehaviour
{
    public bool Attend_Red;

    public bool Attend_Blue;

    public bool RedFast;
    public bool BlueFast;
    // Start is called before the first frame update
    void Start()
    {
        Inversion();
    }

    // Update is called once per frame
    void Update()
    {
        Inversion();
    }

    void Inversion()
    {
        if (Attend_Red)
        {
            Attend_Blue = !Attend_Red;
        }
        if (Attend_Blue)
        {
            Attend_Red = !Attend_Blue;
        }

        if (RedFast)
        {
            BlueFast = !RedFast;
        }
        if (BlueFast)
        {
            RedFast = !BlueFast;
        }
    }
}

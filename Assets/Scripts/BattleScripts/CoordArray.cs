using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordArray : MonoBehaviour
{
    // Start is called before the first frame update
    static int xSize = Location.xSize;
    static int ySize = Location.ySize;
    public static float[,,] cArray = new float [xSize,ySize,2];


    void Awake()
    {
        float leftBottomEvenX = -6.333068f;
        float leftBottomEvenY = -3.375f;
        float rightBottomEvenX = 2.764f;
        float leftTopEvenY = 3.374f;

        float xDistance = (rightBottomEvenX - leftBottomEvenX) / (xSize-1);
        float yDistance = (leftTopEvenY - leftBottomEvenY) / (ySize-1);

        for (int y =0; y<ySize; y++)
        {
            for (int x=0; x<xSize; x++)
            {
                if (y%2==0)
                {
                    cArray[x, y, 0] = leftBottomEvenX + xDistance * x;
                } else
                {
                    cArray[x, y, 0] = leftBottomEvenX + xDistance/2 + xDistance * x;
                }
                cArray[x, y, 1] = leftBottomEvenY + yDistance * y;
            }
        }

        //print("Координаты 4,19 следующие: x = "+cArray[4,19,0]+", у = "+cArray[4,19,1]);
    }

    public static Vector3 HexCenter(int x, int y)
    {
        
        return new Vector3(cArray[x, y, 0], cArray[x, y, 1]);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0)) //TODO Wrap it to method somewhere
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float xOne = (HexCenter(1, 0) - HexCenter(0, 0)).x;
            float yOne = (HexCenter(0, 1) - HexCenter(0, 0)).y;
            int firstX = (int)((mousePosition.x - HexCenter(0, 0).x) / xOne);
            int lastX = firstX + 2;
            int firstY = (int)((mousePosition.y - HexCenter(0, 0).y) / yOne);
            int lastY = firstY + 1;
            float minDistance = float.MaxValue;
            int foundX=-1;
            int foundY=-1;

            for (int x = firstX; x <= lastX; x++)
            {
                for (int y=firstY; y<=lastY; y++)
                {
                    float distance = (mousePosition - (Vector2)HexCenter(x, y)).magnitude;
                    if (distance<minDistance)
                    {
                        minDistance = distance;
                        foundX = x;
                        foundY = y;
                    }
                        
                }
            }    
             print(foundX + " " + foundY);
        }*/

    }
}

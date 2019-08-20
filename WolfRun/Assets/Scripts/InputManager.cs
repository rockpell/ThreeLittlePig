using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Pig nowPlayer;

    private float wheelValue = 0;

    private bool isRightButtonPressing = false;
    private int rightButtonPrssingCount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (nowPlayer == null)
        {
            nowPlayer = GameObject.FindObjectOfType<Pig>();
            if(nowPlayer == null)
                Debug.LogError("nowPlayer is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        nowPlayer.move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        nowPlayer.lookMouse(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        wheelValue = Input.GetAxisRaw("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            nowPlayer.constructionWall();
            nowPlayer.dressingUp();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            isRightButtonPressing = false;
            if (rightButtonPrssingCount < 10)
                nowPlayer.destroyWall();
            rightButtonPrssingCount = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isRightButtonPressing = true;
        }
        else if(wheelValue != 0) // 밑으로 휠 -값 위로 휠 +값
        {

        }


        if (isRightButtonPressing)
        {
            ++rightButtonPrssingCount;
            if(rightButtonPrssingCount > 10)
            {
                isRightButtonPressing = false;
                nowPlayer.fireWall();
            }
        }

        //Debug.Log("rightButtonPrssingCount: " + rightButtonPrssingCount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Pig nowPlayer;

    private float wheelValue = 0;

    void Start()
    {
        nowPlayer = GameManager.Instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        nowPlayer.move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        nowPlayer.lookMouse(_mouseWorldPosition);

        //nowPlayer.setNowLookTile(TileManager.Instance.lookForwardTile(nowPlayer.transform.position, _mouseWorldPosition));

        wheelValue = Input.GetAxisRaw("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            nowPlayer.NowConstructWallType = WallType.STRAW;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            nowPlayer.NowConstructWallType = WallType.WOOD;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            nowPlayer.NowConstructWallType = WallType.BRICK;
        }

        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) // 마우스가 UI 위에 없을때만
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                nowPlayer.constructionWall();
                nowPlayer.dressingUp();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                nowPlayer.destroyWall();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                nowPlayer.fireWall();
            }
            else if (wheelValue != 0) // 밑으로 휠 -값 위로 휠 +값
            {
                nowPlayer.wheelNowConstructWallType(wheelValue);
            }
        }
    }
}

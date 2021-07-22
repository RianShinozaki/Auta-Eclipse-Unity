using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public static class CamVariables
{
    public static float Screenshake = 0;
}
public class CameraManager : MonoBehaviour {
    public static CameraManager Instance;
    public CameraRoom CurrentRoom { get; private set; }
    public Transform CameraTarget;
    public float MoveSpeed;
    public Vector2 DeadZoneSize;

    Vector2 targetPos;
    Vector2 screenSizeCompensation;
    Camera cam;
    float lastCameraSize;

    private void Awake() {
        if(!Instance) {
            Instance = this;
        }

        cam = GetComponent<Camera>();
        SetCameraSizes();
    }

    private void Update() {
        if(cam.orthographicSize != lastCameraSize) {
            SetCameraSizes();
        }

        if (!CameraTarget)
        {
            return;
        }

        if(CameraTarget) {
            if(CameraTarget.position.y >= transform.position.y + DeadZoneSize.y || CameraTarget.position.y <= transform.position.y - DeadZoneSize.y) {
                targetPos.y = CameraTarget.position.y;
            }

            if(CameraTarget.position.x >= transform.position.x + DeadZoneSize.x || CameraTarget.position.x <= transform.position.x - DeadZoneSize.x) {
                targetPos.x = CameraTarget.position.x;
            }
        }

        if(CurrentRoom) {
            float xLeft = CurrentRoom.transform.position.x + CurrentRoom.XLimits.x + screenSizeCompensation.x;
            xLeft = Mathf.Clamp(xLeft, float.MinValue, CurrentRoom.transform.position.x);

            float xRight = CurrentRoom.transform.position.x + CurrentRoom.XLimits.y - screenSizeCompensation.x;
            xRight = Mathf.Clamp(xRight, CurrentRoom.transform.position.x, float.MaxValue);

            float yBottom = CurrentRoom.transform.position.y + CurrentRoom.YLimits.x + screenSizeCompensation.y;
            yBottom = Mathf.Clamp(yBottom, float.MinValue, CurrentRoom.transform.position.y);

            float yTop = CurrentRoom.transform.position.y + CurrentRoom.YLimits.y - screenSizeCompensation.y;
            yTop = Mathf.Clamp(yTop, CurrentRoom.transform.position.y, float.MaxValue);

            targetPos.x = Mathf.Clamp(
                targetPos.x,
                xLeft,
                xRight
                );

            targetPos.y = Mathf.Clamp(
                targetPos.y,
                yBottom,
                yTop
                );
        }

        float dist = Vector2.Distance(transform.position, targetPos);
        Vector3 newPos = Vector2.MoveTowards(transform.position, targetPos, MoveSpeed * (dist / 10f) * Time.deltaTime);
        newPos.z = -10f;

        Vector2 shake = new Vector2(0, 0);

        if(CamVariables.Screenshake > 0)
        {
            shake = new Vector2(Random.Range(-CamVariables.Screenshake, CamVariables.Screenshake), Random.Range(-CamVariables.Screenshake, CamVariables.Screenshake))/2;
            CamVariables.Screenshake -= 0.01f * 60 * Time.deltaTime;
        }
        transform.position = newPos + new Vector3(shake.x, shake.y, 0);
    }

    private void OnDrawGizmosSelected() {
        if(CurrentRoom) {
            Gizmos.color = Color.yellow;
            Vector2 screenSize = new Vector2();
            Vector2 start = cam.ViewportToWorldPoint(Vector2.zero);
            Vector2 end = cam.ViewportToWorldPoint(Vector2.right);
            screenSize.x = Vector2.Distance(start, end) / 2f;

            end = cam.ViewportToWorldPoint(Vector2.up);
            screenSize.y = Vector2.Distance(start, end) / 2f;

            Vector2 corner1 = (Vector2)CurrentRoom.transform.position + new Vector2(CurrentRoom.XLimits.x, CurrentRoom.YLimits.y);
            Vector2 corner2 = (Vector2)CurrentRoom.transform.position + new Vector2(CurrentRoom.XLimits.x, CurrentRoom.YLimits.x);
            Vector2 corner3 = (Vector2)CurrentRoom.transform.position + new Vector2(CurrentRoom.XLimits.y, CurrentRoom.YLimits.x);
            Vector2 corner4 = (Vector2)CurrentRoom.transform.position + new Vector2(CurrentRoom.XLimits.y, CurrentRoom.YLimits.y);

            corner1.x += screenSize.x;
            corner1.y -= screenSize.y;
            corner2.x += screenSize.x;
            corner2.y += screenSize.y;
            corner3.x -= screenSize.x;
            corner3.y += screenSize.y;
            corner4.x -= screenSize.x;
            corner4.y -= screenSize.y;

            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner1, corner4);
            Gizmos.DrawLine(corner4, corner3);
            Gizmos.DrawLine(corner3, corner2);
        }

        Gizmos.color = Color.cyan * new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, 0f), DeadZoneSize * 2f);
    }

    public void SetRoom(CameraRoom room) {
        CurrentRoom = room;
    }
    void SetCameraSizes() {
        //Set up screen size compensation
        Vector2 start = cam.ViewportToWorldPoint(Vector2.zero);
        Vector2 end = cam.ViewportToWorldPoint(Vector2.right);
        screenSizeCompensation.x = Vector2.Distance(start, end) / 2f;

        end = cam.ViewportToWorldPoint(Vector2.up);
        screenSizeCompensation.y = Vector2.Distance(start, end) / 2f;
        lastCameraSize = cam.orthographicSize;
    }
}
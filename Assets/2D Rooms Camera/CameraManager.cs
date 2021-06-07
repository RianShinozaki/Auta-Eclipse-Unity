using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour {
    public static CameraManager Instance;
    public CameraRoom CurrentRoom { get; private set; }
    public Transform CameraTarget;
    public float MoveSpeed;
    public float DeadZoneSize;

    Vector2 targetPos;
    Vector2 deadZoneOffset;
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

        float dist = Vector2.Distance(transform.position, CameraTarget.position);
        deadZoneOffset = ((Vector2)CameraTarget.position - (Vector2)transform.position).normalized * DeadZoneSize;
        if(CameraTarget && dist > DeadZoneSize) {
            targetPos = (Vector2)CameraTarget.position - deadZoneOffset;
        }

        if(CurrentRoom) {
            targetPos.x = Mathf.Clamp(
                targetPos.x,
                CurrentRoom.transform.position.x + CurrentRoom.XLimits.x + screenSizeCompensation.x,
                CurrentRoom.transform.position.x + CurrentRoom.XLimits.y - screenSizeCompensation.x
                );

            targetPos.y = Mathf.Clamp(
                targetPos.y,
                CurrentRoom.transform.position.y + CurrentRoom.YLimits.x + screenSizeCompensation.y,
                CurrentRoom.transform.position.y + CurrentRoom.YLimits.y - screenSizeCompensation.y
                );
        }

        dist = Vector2.Distance(transform.position, targetPos);
        Vector3 newPos = Vector2.MoveTowards(transform.position, targetPos, MoveSpeed * (dist / 10f) * Time.deltaTime);
        newPos.z = -10f;
        transform.position = newPos;
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

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, 0f), Vector3.forward, DeadZoneSize);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, 0f), deadZoneOffset, Color.white);
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
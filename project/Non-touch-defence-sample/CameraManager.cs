using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonobehaviour<CameraManager>
{
    public static Camera tkCamera;
    //public static tk2dCamera tkCamera;
    private bool isInit = false;
    //카메라가 움직이게 될 영역 범위.
    private Bounds rootForBounds = new Bounds();
    public float springDampStrengthen = 90.0f;
    public float springDampStrengthenMin = 2.5f;
    public float springDampStrengthenMax = 36.0f;
    public bool smoothDragStart = false;
    public float momentumAmount = 36.0f;
    public Vector2 momentumVector = Vector2.zero;
    public Vector3 dragStartPosition = Vector3.zero;

    public Vector3 lastMousePosition = Vector3.zero;
    public Vector2 panAcumVector = Vector2.zero;
    private float panThresHolds = 200.0f;

    public bool IsEnablePan = true;
    public bool IsCameraPanning = false;

    private SpringPosition springPosition = null;

    Bounds CalculateBounds()
    {
        GameObject boundsObject = GameObject.FindWithTag("MapBounds");
        if (boundsObject != null)
        {
            this.rootForBounds = boundsObject.GetComponent<BoxCollider>().bounds;
        }
        return this.rootForBounds;
    }

    private void InitCamera()
    {
        if (Camera.main != null && CameraManager.tkCamera == null)
        {
            this.isInit = true;
            this.CalculateBounds();
            CameraManager.tkCamera = Camera.main.GetComponent<Camera>();

            springPosition = Camera.main.GetComponent<SpringPosition>();
        }
    }
    Vector3 CalculateConstrainOffset()
    {
        Vector3 bottomLeft = Vector3.zero;
        Vector3 topRight = new Vector3(Screen.width, Screen.height, 0.0f);

        //bottomLeft = CameraManager.tkCamera.ScreenCamera.ScreenToWorldPoint(bottomLeft);
        //topRight = CameraManager.tkCamera.ScreenCamera.ScreenToWorldPoint(topRight);

        Vector2 minRect = new Vector2(this.rootForBounds.min.x, this.rootForBounds.min.y);
        Vector2 maxRect = new Vector2(this.rootForBounds.max.x, this.rootForBounds.max.y);

        return NGUIMath.ConstrainRect(minRect, maxRect, bottomLeft, topRight);
    }

    bool ConstrainBounds(bool Immediate = false)
    {
        Vector3 offset = CalculateConstrainOffset();
        Camera tk2D = CameraManager.tkCamera;
        if (offset.magnitude > 0.0f)
        {
            if (Immediate == true)
            {
                tk2D.transform.position -= offset;
            }
            else
            {
                SpringPosition sp = SpringPosition.Begin(tk2D.gameObject, tk2D.transform.position - offset,
                    this.springDampStrengthen);
                sp.ignoreTimeScale = true;
                sp.worldSpace = true;
            }
            return true;
        }
        return false;
    }

    void Pan()
    {
        if (InputManager.fingerInputDic.Count == 1)
        {
            InputManager.FingerInput fingerInput = InputManager.Instance.GetCurrentInput();

            if (fingerInput.currentTouchPhase == TouchPhase.Began)
            {
                this.momentumVector = Vector2.zero;
                this.springDampStrengthen = this.springDampStrengthenMax;
                this.dragStartPosition = fingerInput.currentPoint;
                if (this.springPosition != null)
                {
                    this.springPosition.enabled = false;
                }
            }
            else if (fingerInput.currentTouchPhase == TouchPhase.Moved)
            {
                float x = (fingerInput.prevPoint.x - fingerInput.currentPoint.x);
                float y = (fingerInput.currentPoint.y - fingerInput.prevPoint.y);

                Vector2 deltaDrag = new Vector2(x, y);
                //this.momentumVector = momentumVector + deltaDrag * ((0.02f / CameraManager.tkCamera.ZoomFactor) * momentumAmount);
                this.panAcumVector += this.momentumVector;


            }
            else if (fingerInput.currentTouchPhase == TouchPhase.Canceled ||
                    fingerInput.currentTouchPhase == TouchPhase.Ended)
            {
                Vector3 direction = (Vector3)fingerInput.currentPoint - this.dragStartPosition;
                Direction8Way way = Helper.GetDirectionType(direction);
                float panTH = this.panThresHolds;
                if (way == Direction8Way.n || way == Direction8Way.s)
                {
                    panTH = panTH * (Screen.height / Screen.width);
                }
                float distance = Vector3.Distance(fingerInput.currentPoint, this.dragStartPosition);
                if (distance > panTH)
                {
                    this.springDampStrengthen = this.springDampStrengthenMin;
                } else
                {
                    this.springDampStrengthen = this.springDampStrengthenMax;
                }
                this.ConstrainBounds(false);
            }

        } else
        {
            this.panAcumVector = Vector3.zero;
            this.IsCameraPanning = false;
        }

        float deltaTime = Time.deltaTime;
        if (momentumVector.magnitude > 0.0f)
        {
            Vector2 dampingVector = NGUIMath.SpringDampen(ref momentumVector, this.springDampStrengthen, deltaTime);
            if(float.IsNaN(dampingVector.x) == false && float.IsNaN(dampingVector.y) == false)
            {
                this.IsCameraPanning = true;
                CameraManager.tkCamera.transform.Translate(dampingVector, Space.Self);
            }
            if(ConstrainBounds(false) == false)
            {
                springPosition.enabled = false;
            }
        }

    }

    public void CameraUpdate()
    {
        if(this.isInit == false)
        {
            this.InitCamera();
        }
        if(this.IsEnablePan == true)
        {
            Pan();
        }
    }


}

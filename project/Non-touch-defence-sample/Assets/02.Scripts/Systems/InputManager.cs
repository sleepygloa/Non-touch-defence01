using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonobehaviour<InputManager>
{

    public static int rayHitLayer { get; set; }
    public const float DRAG_THRESHOLD = 15.0f;
    public enum MouseButton { Left = 0, Right = 1, Wheel = 2 };

    public class FingerInput
    {
        //
        public int fingerID = -1;

        public Vector2 beginPoint = Vector2.zero;
        public Vector3 beginTouchORMousePostion = Vector3.zero;
        public Vector3 beginRayHitPosition = Vector3.zero;
        public Collider2D beginRayHitCollider = null;
        public Transform beginRayHitTransform = null;
        public int beginRayHitLayer = 0; //backgroud, unit, building.

        public Vector2 currentPoint = Vector2.zero;
        public Vector3 currentTouchORMousePosition = Vector3.zero;
        public Vector3 currentRayHitPosition = Vector3.zero;
        public Collider2D currentRayHitCollider = null;
        public Transform currentRayHitTransform = null;
        public int currentRayHitLayer = 0;

        public Vector2 deltaPoint = Vector2.zero;
        public Vector3 deltaRayHitPosition = Vector3.zero;
        public Vector3 deltaTouchORMousePosition = Vector3.zero;

        public Vector2 prevPoint = Vector2.zero;
        public Vector3 prevTouchORMousePosition = Vector3.zero;
        public Vector3 prevRayHitPosition = Vector3.zero;
        public Collider2D prevRayHitCollider = null;
        public Transform prevRayHitTransform = null;
        public int prevRayHitLayer = 0;

        public TouchPhase currentTouchPhase;
        public TouchPhase prevTouchPhase;

        public Vector3 currentScreenPoint = Vector3.zero;
        public Camera MainCamera;   
        //public tk2dCamera MainCamera;

        public float pressingAccumTime = 0.0f;
        public bool isDragging = false;
        public bool isPressing = false;

        public FingerInput(int fID)
        {
            this.fingerID = fID;
            this.MainCamera = Camera.main.GetComponent<Camera>();
            //this.MainCamera = Camera.main.GetComponent<tk2dCamera>();
        }

        public void SetCurrentPoint(TouchPhase touchPhase, Vector2 currentPos)
        {
            Vector2 newPoint = Vector2.zero;
            Vector3 newTouchORMousePosition = Vector3.zero;
            Vector3 newRayHitPosition = Vector3.zero;
            Collider2D newRayHitCollider = null;
            Transform newRayHitTransform = null;
            int newRayHitLayer = -1;

            this.currentScreenPoint = currentPos;

            //newTouchORMousePosition = this.MainCamera.ScreenCamera.ScreenToWorldPoint(currentScreenPoint);
            newPoint = new Vector2(currentPos.x, Screen.height - currentPos.y);
            Debug.Log("Current Pos :" + currentPos.ToString() + " / New Pos : " + newPoint.ToString());
            RaycastHit2D raycastHit = Physics2D.Raycast(newTouchORMousePosition, Vector2.zero);

            if (raycastHit.collider != null)
            {
                //충돌검출이 되었다.
                newRayHitPosition = raycastHit.point;
                newRayHitCollider = raycastHit.collider;
                newRayHitTransform = raycastHit.transform;
                newRayHitLayer = 1 << raycastHit.transform.gameObject.layer;
            }
            else
            {
                //충돌 검출이 안되었다.. 빈 허공을 클릭,터치한 경우.
                newRayHitCollider = null;
                newRayHitLayer = -1;
                newRayHitTransform = null;
            }

            if (touchPhase == TouchPhase.Began)
            {
                this.beginPoint = this.currentPoint = newPoint;
                this.beginTouchORMousePostion = this.currentTouchORMousePosition = newTouchORMousePosition;
                this.beginRayHitPosition = this.currentRayHitPosition = newRayHitPosition;
                this.beginRayHitLayer = this.currentRayHitLayer = newRayHitLayer;
                this.beginRayHitTransform = this.currentRayHitTransform = newRayHitTransform;

            }
            else
            {
                deltaPoint = newPoint - currentPoint;
                deltaTouchORMousePosition = newTouchORMousePosition - currentTouchORMousePosition;
                deltaRayHitPosition = newRayHitPosition - currentRayHitPosition;

                prevPoint = currentPoint;
                prevTouchORMousePosition = currentTouchORMousePosition;
                prevRayHitPosition = currentRayHitPosition;
                prevRayHitLayer = currentRayHitLayer;
                prevRayHitCollider = currentRayHitCollider;
                prevRayHitTransform = currentRayHitTransform;

                currentPoint = newPoint;
                currentTouchORMousePosition = newTouchORMousePosition;
                currentRayHitLayer = newRayHitLayer;
                currentRayHitCollider = newRayHitCollider;
                currentRayHitPosition = newRayHitPosition;
                currentRayHitTransform = newRayHitTransform;

            }
            this.currentTouchPhase = touchPhase;

        }
    }

    public static Dictionary<int, FingerInput> fingerInputDic = new Dictionary<int, FingerInput>();

    public void TouchUpdate()
    {
        FingerInput fingerInput;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (fingerInputDic.TryGetValue(Input.touches[i].fingerId, out fingerInput) == false)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        fingerInput = new FingerInput(Input.touches[i].fingerId);
                        fingerInputDic.Add(Input.touches[i].fingerId, fingerInput);
                    }
                }
                rayHitLayer = Define.LAYERMASK_ALL_PICKLAYER;

                fingerInput.SetCurrentPoint(Input.touches[i].phase, Input.touches[i].position);

                switch (Input.touches[i].phase)
                {
                    case TouchPhase.Moved:
                        {
                            fingerInput.pressingAccumTime += Time.deltaTime;
                            float dragDelta = Vector2.Distance(fingerInput.beginPoint, fingerInput.currentPoint);
                            fingerInput.isDragging = (dragDelta > DRAG_THRESHOLD) ? true : false;
                        }
                        break;
                    case TouchPhase.Stationary:
                        {
                            fingerInput.pressingAccumTime += Time.deltaTime;
                            fingerInput.isPressing = true;
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        {
                            fingerInput.isPressing = false;
                            fingerInput.pressingAccumTime = 0.0f;
                            fingerInput.isDragging = false;
                        }
                        break;
                }
                fingerInput.prevTouchPhase = fingerInput.currentTouchPhase;

            }
        }
        else
        {
            //Mouse Control..
            if (Input.GetMouseButtonDown((int)MouseButton.Left) == true)
            {
                rayHitLayer = Define.LAYERMASK_ALL_PICKLAYER;
                if (fingerInputDic.TryGetValue((int)MouseButton.Left, out fingerInput) == false)
                {
                    fingerInput = new FingerInput((int)MouseButton.Left);
                    fingerInput.SetCurrentPoint(TouchPhase.Began, Input.mousePosition);
                    fingerInputDic.Add((int)MouseButton.Left, fingerInput);
                }
            }
            else if (Input.GetMouseButtonUp((int)MouseButton.Left) == true)
            {
                if (fingerInputDic.TryGetValue((int)MouseButton.Left, out fingerInput) == true)
                {
                    fingerInput.SetCurrentPoint(TouchPhase.Ended, Input.mousePosition);
                    fingerInput.isDragging = false;
                    fingerInput.isPressing = false;
                    fingerInput.pressingAccumTime = 0.0f;
                }
            }
            else if (Input.GetMouseButton((int)MouseButton.Left) == true)
            {
                if (fingerInputDic.TryGetValue((int)MouseButton.Left, out fingerInput) == true)
                {
                    fingerInput.SetCurrentPoint(TouchPhase.Moved, Input.mousePosition);
                    float dragDelta = Vector2.Distance(fingerInput.prevPoint, fingerInput.currentPoint);
                    fingerInput.isDragging = (dragDelta > DRAG_THRESHOLD) ? true : false;
                    fingerInput.isPressing = true;
                    fingerInput.pressingAccumTime += Time.deltaTime;
                }
            }
            else
            {
                if (fingerInputDic.Count > 0)
                {
                    fingerInputDic.Clear();
                }
            }
        }
    }

    public FingerInput GetCurrentInput()
    {
        if (fingerInputDic.Count == 0)
        {
            return null;
        }

        var enumerator = fingerInputDic.GetEnumerator();
        enumerator.MoveNext();
        FingerInput current_input = enumerator.Current.Value;
        return current_input;
    }

    public Vector3 GetDragPoint()
    {
        if (fingerInputDic.Count == 0)
        {
            return Define.EXCEPT_POSITION;
        }
        FingerInput fingerInput = GetCurrentInput();
        return fingerInput.currentTouchORMousePosition;
    }

    public HeroScript SelectBuilding = null;
    public HeroScript DragBuilding = null;

    void TouchBeginState(FingerInput input)
    {
        if (input.currentRayHitLayer == Define.LAYERMASK_BUILDING)
        {
/*            CameraManager.Instance.IsEnablePan = false;*/

            this.SelectBuilding = input.currentRayHitTransform.GetComponent<HeroScript>();
            if (this.DragBuilding == null)
            {
                this.DragBuilding = this.SelectBuilding;
                this.DragBuilding.IsSelect = true;

            }
            else
            {
                this.DragBuilding.IsSelect = false;
                this.DragBuilding = this.SelectBuilding;
                this.DragBuilding.IsSelect = true;
            }
        }
    }
    void TouchPressingState(FingerInput input)
    {

    }
    void TouchEndState(FingerInput input)
    {
        if (this.DragBuilding != null)
        {
            this.DragBuilding.IsSelect = false;
        }
        if (this.SelectBuilding != null)
        {
            this.SelectBuilding = null;
        }
/*        if (CameraManager.Instance.IsEnablePan == false)
        {
            CameraManager.Instance.IsEnablePan = true;

        }*/
    }

    public void UpdateGameInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EntityManager.Instance.SpawnEntity((int)EntityList.WatchTower);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EntityManager.Instance.SpawnEntity((int)EntityList.Creed);
        }
        if (fingerInputDic.Count == 0)
        {
            return;
        }
        foreach (FingerInput input in fingerInputDic.Values)
        {
            if (input.currentTouchPhase == TouchPhase.Began)
            {
                TouchBeginState(input);
            }
            else if (input.currentTouchPhase == TouchPhase.Moved)
            {
                TouchPressingState(input);
            }
            else if (input.currentTouchPhase == TouchPhase.Ended ||
               input.currentTouchPhase == TouchPhase.Canceled)
            {
                TouchEndState(input);
            }
        }
    }


}

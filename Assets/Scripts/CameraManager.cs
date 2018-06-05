using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Ins { get; private set; }

    Vector3 pivotPos;
    [SerializeField] GameObject prefabPivot;
    GameObject pivot;

    public bool Dragging { get; private set; }
    public bool Rotating { get; private set; }

    Vector2 prevMousePos, currentMousePos, originalMousePos;
    [SerializeField] float fire3TouchTime;
    float fire3TouchTimer = -1;
    float fire3TouchOffset = 30;
    [SerializeField] float fire2TouchTime;
    float fire2TouchTimer = -1;
    float fire2TouchOffset = 30;
    [SerializeField] float dragSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] float minDistanceFromPivot;

    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        pivotPos = Map.Ins.centerPos;
        Camera.main.transform.position = pivotPos + new Vector3(0, Map.Ins.rowCount, Map.Ins.rowCount);
        Camera.main.transform.LookAt(pivotPos);

        pivot = Instantiate(prefabPivot);
        pivot.transform.position = pivotPos;
        zoomSpeed *= Map.Ins.rowCount + Map.Ins.colCount;
        pivot.SetActive(false);
    }

    void Update()
    {
        if (UiManager.Ins.OnUi || UiManager.Ins.OnUiDrag) return;
        if (QueryAxisFire3()) return;
        if (QueryAxisFire2()) return;
        if (QueryAxisScrollWheel()) return;
        if (QueryKeyDown(KeyCode.D)) return;
    }

    bool QueryAxisFire3()
    {
        if (VirtualAxisManager.GetAxisDown("Fire3")) {
            prevMousePos = currentMousePos = originalMousePos = GetMousePos();
            fire3TouchTimer = 0;
            Dragging = false;
            return true;
        }
        else if (VirtualAxisManager.GetAxis("Fire3")) {
            currentMousePos = GetMousePos();

            if (Dragging) {
                var prevCamPos = Camera.main.transform.position;
                var temp = currentMousePos - prevMousePos;
                var dirVector = Camera.main.transform.right * temp[0] + Camera.main.transform.up * temp[1];
                Camera.main.transform.position -= dirVector * (dragSpeed * Time.deltaTime);
                pivotPos += Camera.main.transform.position - prevCamPos;
                pivot.transform.position = pivotPos;
            }

            if (fire3TouchTimer >= 0) {
                fire3TouchTimer += Time.deltaTime;
                if (fire3TouchTimer > fire3TouchTime
                    || Mathf.Abs(currentMousePos[0] - originalMousePos[0]) > fire3TouchOffset
                    || Mathf.Abs(currentMousePos[1] - originalMousePos[1]) > fire3TouchOffset) {
                    fire3TouchTimer = -1;
                    Dragging = true;
                }
            }

            prevMousePos = currentMousePos;
            return true;
        }
        else if (VirtualAxisManager.GetAxisUp("Fire3")) {
            fire3TouchTimer = -1;
            return true;
        }
        return false;
    }

    bool QueryAxisFire2()
    {
        if (VirtualAxisManager.GetAxisDown("Fire2")) {
            prevMousePos = currentMousePos = originalMousePos = GetMousePos();
            fire2TouchTimer = 0;
            Rotating = false;
            return true;
        }
        else if (VirtualAxisManager.GetAxis("Fire2")) {
            currentMousePos = GetMousePos();

            if (Rotating) {
                var dirVector = currentMousePos - prevMousePos;
                var xRot = -dirVector[1] * rotateSpeed * Time.deltaTime;
                var newEulerAngle0 = Camera.main.transform.localRotation.eulerAngles[0] + xRot;
                Camera.main.transform.RotateAround(pivotPos, Vector3.up, dirVector[0] * rotateSpeed * Time.deltaTime);
                if (newEulerAngle0 <= 90 || newEulerAngle0 >= 270)
                    Camera.main.transform.RotateAround(pivotPos, Camera.main.transform.right, xRot);
            }

            if (fire2TouchTimer >= 0) {
                fire2TouchTimer += Time.deltaTime;
                if (fire2TouchTimer > fire2TouchTime
                    || Mathf.Abs(currentMousePos[0] - originalMousePos[0]) > fire2TouchOffset
                    || Mathf.Abs(currentMousePos[1] - originalMousePos[1]) > fire2TouchOffset) {
                    fire2TouchTimer = -1;
                    Rotating = true;
                }
            }

            prevMousePos = currentMousePos;
            return true;
        }
        else if (VirtualAxisManager.GetAxisUp("Fire2")) {
            fire2TouchTimer = -1;
            return true;
        }
        return false;
    }

    bool QueryAxisScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            var newPos = Camera.main.transform.position;
            newPos += Camera.main.transform.forward * (zoomSpeed * Time.deltaTime);
            if (HorizontalDistance(newPos, pivotPos) < minDistanceFromPivot) return true;
            Camera.main.transform.position = newPos;
            return true;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            Camera.main.transform.position -= Camera.main.transform.forward * (zoomSpeed * Time.deltaTime);
            return true;
        }
        return false;
    }

    bool QueryKeyDown(KeyCode _keyCode)
    {
        switch (_keyCode) {
            case KeyCode.D:
                if (Input.GetKeyDown(_keyCode))
                    pivot.SetActive(!pivot.activeSelf);
                return true;
        }
        return false;
    }

    Vector2 GetMousePos()
    {
        return new Vector2(Input.mousePosition[0], Input.mousePosition[1]);
    }

    float HorizontalDistance(Vector3 _v0, Vector3 _v1)
    {
        var diff0 = _v0[0] - _v1[0];
        var diff1 = _v0[2] - _v1[2];
        return Mathf.Sqrt(diff0 * diff0 + diff1 * diff1);
    }
}

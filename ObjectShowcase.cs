using UnityEngine;

public enum RollDirection
{
    X,
    Y,
    Z
}

public class ObjectShowcase : MonoBehaviour
{
    [Space]
    [Tooltip("Is the object ready for showcase?")] public bool showcaseReady = true;
    [Space]
    [Space]
    [SerializeField] private float rotScale;
    [SerializeField] private float smoothingRotationDelta;
    [SerializeField] private RollDirection mouseXRollDirection;
    [SerializeField] private bool mouseXInverted;
    [SerializeField] private RollDirection mouseYRollDirection;
    [SerializeField] private bool mouseYInverted;
    [SerializeField] private float pitchMin, pitchMax;
    [SerializeField] private float rollMin, rollMax;
    [SerializeField, Range(0, 5)] private float sustainTime;

    private Vector2 initMousePos;
    private Vector2 currentMousePos;
    private Vector2 difference;
    private Quaternion targetRotation;
    private Quaternion initRotation;
    private Vector3 initVectorRotation;
    private Vector3 newRot;
    private float pitch, roll;
    private float angleSmoothingVelocity;
    private float sustainedTime;

    private void Awake()
    {
        initRotation = transform.rotation;
        initVectorRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!showcaseReady) return;

        float delta = Quaternion.Angle(transform.rotation, targetRotation);

        if (Input.GetMouseButtonDown(0))
        {
            initMousePos = Input.mousePosition;
            sustainedTime = sustainTime;
        }
        else if (Input.GetMouseButton(0))
        {
            currentMousePos = Input.mousePosition;
            difference = currentMousePos - initMousePos;
            pitch = Mathf.Clamp(difference.x * rotScale, pitchMin, pitchMax);
            roll = Mathf.Clamp(difference.y * rotScale, rollMin, rollMax);
            newRot = GetRotationDirection(mouseXRollDirection, pitch) * GetModifier(mouseXInverted) +
                     GetRotationDirection(mouseYRollDirection, roll) * GetModifier(mouseYInverted);
            newRot += initVectorRotation;
            targetRotation = Quaternion.Euler(newRot);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            sustainedTime = 0;
        }
        else if(sustainedTime < sustainTime)
        {
            sustainedTime += Time.deltaTime;
            if(sustainedTime > sustainTime) 
            {
                targetRotation = initRotation;
            }
        }


        if (delta > 0f)
        {
            float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angleSmoothingVelocity, smoothingRotationDelta);
            t = 1.0f - (t / delta);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }


    }

    private Vector3 GetRotationDirection(RollDirection rDirection, float value) => rDirection switch
    {
        RollDirection.X => Vector3.right * value,
        RollDirection.Y => Vector3.up * value,
        RollDirection.Z => Vector3.forward * value,
        _ => Vector3.zero,
    };

    private float GetModifier(bool value) => value ? -1 : 1;
}

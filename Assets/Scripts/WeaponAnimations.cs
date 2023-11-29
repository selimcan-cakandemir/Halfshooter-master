using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{

    public MouseLook mouseLookScript;
    public PlayerMovement playerMovementScript;

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    Vector3 bobPosition;

    Vector3 targetVector;
    Vector3 targetRotation;



    void Awake() {

        mouseLookScript = Camera.main.GetComponent<MouseLook>();
    }

    public void Recoil(bool isShot) { // Calling this from the Gun script

        Vector3 recoilTargetVector = new Vector3(0, 0, -50f);
        Vector3 recoilTargetRotation = new Vector3(200f, 0, 0);
        targetVector = isShot ? recoilTargetVector : Vector3.zero;
        targetRotation = isShot ? recoilTargetRotation : Vector3.zero;
    }

    void Sway() {

        Vector3 invertLook = mouseLookScript.lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation() {

        Vector2 invertLook = mouseLookScript.lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);


    }

    void BobOffset() {

        speedCurve += Time.deltaTime * (playerMovementScript.CheckIfGrounded() ? playerMovementScript.move.magnitude : 1f) + 0.01f;

        bobPosition.x = 
            (curveCos * bobLimit.x * (playerMovementScript.CheckIfGrounded() ? 1 : 0))
            - (playerMovementScript.move.x * travelLimit.x);

        bobPosition.y =
            (curveSin * bobLimit.y)
            - (playerMovementScript.move.y * travelLimit.y);

        bobPosition.z =
            -(playerMovementScript.move.y * travelLimit.z);

    }

    void BobRotation() {

        bobEulerRotation.x = (playerMovementScript.move != Vector3.zero ? multiplier.x * (Mathf.Sin(2*speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRotation.y = (playerMovementScript.move != Vector3.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (playerMovementScript.move != Vector3.zero ? multiplier.z * curveCos * playerMovementScript.move.x : 0);

    }

    float smooth = 1f;
    float smoothrot = 12f;

    void CompositePositionRotation() {

        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition + targetVector, Time.deltaTime * smooth);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation) * Quaternion.Euler(targetRotation), Time.deltaTime * smoothrot);

    }

    void Update() {
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();
        CompositePositionRotation();
    }



}

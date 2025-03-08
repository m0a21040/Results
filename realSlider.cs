using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class realSlider : MonoBehaviour
{
    [Header("Forces")]
    public float normalForce = 5000f;

    [Header("Slider Properties")]
    public float sliderSpinRate = 1200f;
    public Vector3 breakForceX = new Vector3(0.1f, -0.05f, 1.9f);
    public Vector3 breakForceY = new Vector3(0.1f, -0.05f, 1.9f);
    public float changeStartDistance = 2.0f;
    public float dragFactor = 0.98f;

    [Header("Materials")]
    public Material transparentMaterial;
    public Material normalMaterial;

    [Header("Timing")]
    public float delayTime = 0.5f;
    private bool hasThrown = false;
    private bool canThrow = true;
    private Vector3 startPosition;
    private bool shouldApplyBreak = false;

    private Rigidbody rb;
    private Renderer ballRenderer;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [Header("Strike Zone Positions (Adjustable)")]
    public Vector3 inHigh;
    public Vector3 middleHigh;
    public Vector3 outHigh;
    public Vector3 inMiddle;
    public Vector3 middle;
    public Vector3 outMiddle;
    public Vector3 inLow;
    public Vector3 middleLow;
    public Vector3 outLow;

    private Vector3[,] strikeZonePositions;
    private int currentRow = 1;
    private int currentCol = 1;
    private GameObject cursorObject;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();

        if (ballRenderer == null)
        {
            Debug.LogError("Renderer component is missing on this GameObject!");
        }

        rb.isKinematic = true;
        rb.useGravity = true;

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        strikeZonePositions = new Vector3[3, 3]
        {
            { inHigh, middleHigh, outHigh },
            { inMiddle, middle, outMiddle },
            { inLow, middleLow, outLow }
        };

        cursorObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cursorObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cursorObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
        cursorObject.GetComponent<Collider>().enabled = false;

        currentRow = 1;
        currentCol = 1;
        UpdateCursorPosition();

        SetMaterial(true);
    }

    private void Update()
    {
        MoveCursor();

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (!hasThrown)
            {
                StartCoroutine(ThrowSliderAfterDelay(currentRow, currentCol));
            }
        }
        if (hasThrown && (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two)))
        {
            ResetBall();
        }
    }

    private void MoveCursor()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick);

        if (thumbstick.y > 0.5f && currentRow > 0) currentRow--;
        else if (thumbstick.y < -0.5f && currentRow < 2) currentRow++;
        if (thumbstick.x < -0.5f && currentCol > 0) currentCol--;
        else if (thumbstick.x > 0.5f && currentCol < 2) currentCol++;

        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        cursorObject.transform.position = strikeZonePositions[currentRow, currentCol];
    }

    private IEnumerator ThrowSliderAfterDelay(int row, int col)
    {
        yield return new WaitForSeconds(delayTime);

        rb.isKinematic = false;
        yield return new WaitForFixedUpdate();

        hasThrown = true;
        startPosition = transform.position;

        SetMaterial(false);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(strikeZonePositions[row, col] * normalForce, ForceMode.Impulse);
        ApplySliderSpin();
        shouldApplyBreak = false;
    }

    private void ApplySliderSpin()
    {
        Vector3 spinAxis = transform.right;
        Vector3 spin = spinAxis * (sliderSpinRate * 2 * Mathf.PI / 60);
        rb.angularVelocity = spin;
    }

    private void FixedUpdate()
    {
        if (hasThrown)
        {
            float traveledDistance = Vector3.Distance(startPosition, transform.position);

            if (traveledDistance >= changeStartDistance)
            {
                shouldApplyBreak = true;
            }

            if (shouldApplyBreak)
            {
                Vector3 magnusForce = Vector3.Cross(rb.angularVelocity, rb.velocity.normalized);
                magnusForce = breakForceX + breakForceY;
                rb.AddForce(magnusForce, ForceMode.Acceleration);
            }

            rb.velocity *= dragFactor;

            if (rb.velocity.magnitude < 5f)
            {
                rb.velocity = rb.velocity.normalized * 5f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target") || collision.gameObject.CompareTag("TagA") || collision.gameObject.CompareTag("TagB"))
        {
            StopBall();
        }
    }

    private void StopBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void ResetBall()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        hasThrown = false;
        shouldApplyBreak = false;

        // **カーソル位置を中央にリセット**
        currentRow = 1;
        currentCol = 1;
        UpdateCursorPosition();

        SetMaterial(true);
    }

    private void SetMaterial(bool isTransparent)
    {
        if (ballRenderer != null)
        {
            ballRenderer.material = isTransparent ? transparentMaterial : normalMaterial;
        }
    }
}

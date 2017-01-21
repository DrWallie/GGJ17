using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public Transform cam;
    public LayerMask ignoreMask;
    public float mouseXSens = 10f;
    public float mouseYSens = 12f;
    public bool clampVerticalRotation = true;
    public float clampAngle = 180f;
    [Space(10)]
    public float moveSpeed = 6;
    public float lerpSpeed = 10;
    public float groundedDistance = 1.25f;
    public float wallDetectRange = 2;
    [Space(10)]
    public float jumpHeight = 3;
    public float secToJumpApex = .5f;

    float gravity, jumpVelocity;

    private Vector3 groundNormal;
    private Vector3 charNormal;
    private float distGround;
    private bool jumping = false;
    private bool isGrounded;

    private float camSmoothTime = 5f;

    private CapsuleCollider col;
    private Rigidbody rigid;

    private Quaternion charTargetRot;
    private Quaternion camTargetRot;

    private Transform charTransform;
    private Vector3 charPosOld;

    [Range(16, 1440)]
    public int orbDirectionCount = 360;
    public int orbRayLenght = 10;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>().transform;
        cam.SetParent(transform);
        charNormal = transform.up; // normal starts as character up direction
        charTransform = transform;
        charPosOld = transform.position;
        col = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true;

        gravity = (2 * jumpHeight) / Mathf.Pow(secToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * secToJumpApex;

        distGround = col.height - col.center.y; // distance from transform.position to ground
    }

    private void FixedUpdate()
    {
        rigid.AddForce(-gravity * rigid.mass * charNormal);
    }

    private void LateUpdate()
    {
        LookRotation();
        if (jumping) return; // abort Update while jumping to a wall
        Ray ray;
        RaycastHit hit;

        if (Input.GetButtonDown("Jump"))
        {
            Debug.DrawRay(transform.position, (transform.position - charPosOld) * wallDetectRange, Color.blue, 1f);
            //ray = new Ray(transform.position, (transform.position - charPosOld) * wallDetectRange);
            ray = new Ray(transform.position, transform.position - charPosOld);
            if (Physics.Raycast(ray, out hit, wallDetectRange, ignoreMask))// wall ahead?
            { // yes: jump to wall
                JumpToWall(hit.point, hit.normal);
            }
            else if (isGrounded)
            { // no: if grounded, jump up
                rigid.velocity += jumpVelocity * charNormal;
            }
        }

        //charTransform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, 0);

        //Debug.DrawLine(transform.position, transform.position + (transform.position - charPosOld).normalized, Color.blue);
        //Debug.DrawRay(transform.position, new Vector3(Input.GetAxis("Horizontal"), rigid.velocity.y, Input.GetAxis("Vertical")), Color.blue);

        Debug.DrawRay(transform.position, -transform.up * groundedDistance);
        ray = new Ray(transform.position, -transform.up); //casts ray downwards
        if (Physics.Raycast(ray, out hit, distGround + groundedDistance, ~ignoreMask))//if (Physics.Raycast(ray, out hit, groundedDistance , ignoreMask))
        { // use the ray to update charNormal and isGrounded
            //Debug.DrawRay(charTransform.position, -charNormal);
            isGrounded = hit.distance <= distGround + groundedDistance; //if hit.distance is less than distance to ground - max distance to ground isGrounded = true
            isGrounded = true;
            groundNormal = hit.normal;
        }
        else
        {
            //print(charTransform.position);
            isGrounded = false;
            groundNormal = SphereRay();
        }

        Debug.DrawRay(transform.position, (transform.position - charPosOld).normalized, Color.yellow);
        ray = new Ray(transform.position, (transform.position - charPosOld).normalized);
        if (Physics.Raycast(ray, out hit, wallDetectRange))// wall ahead?   
        { // yes: jump to wall
            groundNormal = hit.normal;
        }
        charPosOld = transform.position;

        charNormal = Vector3.Lerp(charNormal, groundNormal, lerpSpeed * Time.deltaTime);

        Vector3 myForward = Vector3.Cross(charTransform.right, charNormal); // find forward direction with new charNormal

        Quaternion targetRot = Quaternion.LookRotation(myForward, charNormal); // align character to the new charNormal while keeping the forward direction

        charTransform.rotation = Quaternion.Lerp(charTransform.rotation, targetRot, lerpSpeed * Time.deltaTime);

        charTransform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime); //move the character forth/ back with Vertical axis:

        //Debug.DrawLine(charTransform.position, charTransform.position + (charTransform.position - charPosOld).normalized * wallDetectRange, Color.red);
        //Debug.DrawRay(transform.position, (transform.position - charPosOld) * wallDetectRange);

        //charPosOld = transform.position;
    }

    private void JumpToWall(Vector3 point, Vector3 normal)
    {
        jumping = true;
        rigid.isKinematic = true; // disable physics while jumping
        Vector3 orgPos = charTransform.position;
        Quaternion orgRot = charTransform.rotation;
        Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
        Vector3 myForward = Vector3.Cross(charTransform.right, normal);
        Quaternion dstRot = Quaternion.LookRotation(myForward, normal);

        StartCoroutine(jumpTime(orgPos, orgRot, dstPos, dstRot, normal));
    }

    private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal)
    {
        for (float t = 0.0f; t < 1.0f;)
        {
            t += Time.deltaTime;
            charTransform.position = Vector3.Lerp(orgPos, dstPos, t);
            charTransform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            yield return null; // return here next frame
        }
        charNormal = normal; // update charNormal
        rigid.isKinematic = false; // enable physics
        jumping = false;
    }

    public void LookRotation()
    {
        charTargetRot = transform.localRotation;
        camTargetRot = cam.localRotation;

        float yRot = Input.GetAxis("Mouse X") * mouseXSens;
        float xRot = Input.GetAxis("Mouse Y") * mouseYSens;

        charTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        camTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation) { camTargetRot = ClampRotationXAxis(camTargetRot); }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, charTargetRot, camSmoothTime * Time.deltaTime);
        cam.localRotation = Quaternion.Slerp(cam.localRotation, camTargetRot, camSmoothTime * Time.deltaTime);
    }

    Quaternion ClampRotationXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -(clampAngle / 2), (clampAngle / 2));

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    //public float SphereRay()
    public Vector3 SphereRay()
    {
        //var distances = new List<float>();
        float shortestDist = orbRayLenght;
        Vector3 closestHit = Vector3.zero;

        foreach (var direction in GetSphereDirections(orbDirectionCount))
        {
            Debug.DrawRay(transform.position, direction * orbRayLenght, Color.red);

            //Vector3.Distance(transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, orbRayLenght))
            {
                if (hit.distance <= shortestDist)
                {
                    shortestDist = hit.distance;
                    //closestHit = hit.transform.position; DIT WAS JE FOUT MONGOOL
                    closestHit = hit.normal;
                }
            }
        }

        return closestHit;
        //return distances.Any() ? distances.Min() : rayLenght;
    }

    private Vector3[] GetSphereDirections(int numDirections)
    {
        var points = new Vector3[numDirections];
        var inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        var off = 2f / numDirections;

        foreach (var k in Enumerable.Range(0, numDirections))
        {
            var y = k * off - 1 + (off / 2);
            var r = Mathf.Sqrt(1 - y * y);
            var phi = k * inc;
            var x = (float)(Mathf.Cos(phi) * r);
            var z = (float)(Mathf.Sin(phi) * r);
            points[k] = new Vector3(x, y, z);
        }

        return points;
    }
}
using System;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraScript : MonoBehaviour
{
    public float CameraFollowSpeed = 10f;
    [SerializeField] private float updateTolerance = 0.1f;
    public AnimationCurve ShakeStrenght;
    private Vector3 shakeOffset = Vector3.zero;
    public Transform FollowingTarget;

    PixelPerfectCamera ppCAmrea;

    private void Start()
    {
        ppCAmrea = GetComponent<PixelPerfectCamera>();
        FollowingTarget = GameManager.instance.player.transform;
    }
    private void Update()
    {
        CameraFollowTarget(FollowingTarget);
    }

    public IEnumerator ShakeScreenForTime(float time)
    {
        Debug.Log("You forgot my shake!.... Shake that body...");
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float strength = ShakeStrenght.Evaluate(elapsedTime / time);
            shakeOffset = UnityEngine.Random.insideUnitSphere * strength;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    private void CameraFollowTarget(Transform Target)
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(Target.position.x, Target.position.y, -10);

        if (Vector3.Distance(currentPosition, targetPosition) > updateTolerance)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * CameraFollowSpeed);
        }

        transform.position += shakeOffset;
    }
    public IEnumerator Zoom(float duration, int zoomAmount)
    {
        Debug.Log("Zooming");
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            ppCAmrea.refResolutionX = Convert.ToInt32(Mathf.Lerp(ppCAmrea.refResolutionX, zoomAmount, time / duration));
            ppCAmrea.refResolutionY = Convert.ToInt32(Mathf.Lerp(ppCAmrea.refResolutionY, zoomAmount / 2, time / duration));

            yield return null;
        }

        ppCAmrea.refResolutionX = zoomAmount;
        ppCAmrea.refResolutionY = zoomAmount / 2;
    }

    public IEnumerator ChangeFollowSpeedAfterTime(float followspeed, float time)
    {
        yield return new WaitForSeconds(time);
        CameraFollowSpeed = followspeed;
    }
}
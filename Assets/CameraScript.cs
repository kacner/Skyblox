using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraScript : MonoBehaviour
{
    public float CameraFollowSpeed = 10f;
    [SerializeField] private float updateTolerance = 0.1f;
    public AnimationCurve ShakeStrenght;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 movetowardsOffset;
    public Transform NPC;
    public bool ShouldFollow = true;

    PixelPerfectCamera ppCAmrea;

    private void Start()
    {
        ppCAmrea = GetComponent<PixelPerfectCamera>();
    }
    private void Update()
    {
        CameraMovement(movetowardsOffset);
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

    private void CameraMovement(Vector3 FinalOffset)
    {
        if (ShouldFollow)
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(GameManager.instance.player.transform.position.x, GameManager.instance.player.transform.position.y, -10);

            if (Vector3.Distance(currentPosition, targetPosition) > updateTolerance)
            {
                transform.position = Vector3.Lerp(currentPosition, targetPosition + FinalOffset, Time.fixedDeltaTime * CameraFollowSpeed);
            }

            transform.position += shakeOffset;
        }
    }
    public IEnumerator MoveToWardsForTime(Transform target, float transitionDuration)
    {
        Debug.Log("MovingToWardsForTime");
        ShouldFollow = false;

        float timer = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, timer);

            yield return null;
        }

        transform.position = targetPosition;
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
}
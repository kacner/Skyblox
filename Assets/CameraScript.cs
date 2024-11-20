using System.Collections;
using TMPro;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public bool shouldFollow = true;
    public float CameraFollowSpeed = 10f;
    [SerializeField] private float updateTolerance = 0.1f;
    public AnimationCurve ShakeStrenght;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 movetowardsOffset;
    public Transform NPC;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            shouldFollow = false;
            StartCoroutine(MoveToWardsForTime(NPC, 4f));
        }

        CameraMovement(movetowardsOffset);
    }

    public IEnumerator ShakeScreenForTime(float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float strength = ShakeStrenght.Evaluate(elapsedTime / time);
            shakeOffset = Random.insideUnitSphere * strength;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    void CameraMovement(Vector3 FinalOffset)
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(GameManager.instance.player.transform.position.x, GameManager.instance.player.transform.position.y, -10);

        if (Vector3.Distance(currentPosition, targetPosition) > updateTolerance)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition + FinalOffset, Time.fixedDeltaTime * CameraFollowSpeed);
        }

        transform.position += shakeOffset;
    }
    private IEnumerator MoveToWardsForTime(Transform target, float duration)
    {
        float timer = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        float transitionduration = duration / 3;

        while (timer < transitionduration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / transitionduration);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        StartCoroutine(StayForTime(target, duration * 0.66f));
    }
    private IEnumerator StayForTime(Transform target, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);


            yield return null;
        }

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
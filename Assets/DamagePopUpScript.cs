using System.Collections;
using UnityEngine;
using TMPro;

public class DamagePopUpScript : MonoBehaviour
{
    private TextMeshPro textMesh;

    public void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(float damageAmount)
    {
        float newfloat = damageAmount;
        if (newfloat < 10f)
        {
            float FinishedNumer = Mathf.Round(newfloat * 10f) / 10f;
            textMesh.SetText(FinishedNumer.ToString());
        }
        else
        textMesh.SetText(newfloat.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", "'"));

        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

}

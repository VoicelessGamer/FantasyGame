using System.Collections;
using UnityEngine;

public class AttachWeapon : MonoBehaviour {

    public GameObject attachPoint;

    public GameObject weapon;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(attach());
    }

    // Update is called once per frame
    IEnumerator attach() {
        yield return new WaitForSeconds(5);

        Instantiate(weapon, attachPoint.transform.position, attachPoint.transform.rotation, attachPoint.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderController : MonoBehaviour {

    public Animator animator;

    public float maxWalkSpeed = 1f;

    public bool test = true;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();

        StartCoroutine(testFunc());
    }

    // Update is called once per frame
    void Update() {
        if(test) {
            return;
        }
        transform.position -= new Vector3(maxWalkSpeed * Time.deltaTime, 0, 0);

        animator.SetFloat("Speed", 1);
    }

    IEnumerator testFunc() {
        yield return new WaitForSeconds(5);
        test = false;
    }
}

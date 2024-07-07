using TMPro.EditorUtilities;
using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
    [SerializeField] private float knockBackForce;

    private GameObject parent;


    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    public void SetParent(GameObject _parent)
    {
        parent = _parent;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bee"))
        {
            Debug.Log("SMOKED");

            Debug.DrawLine(transform.position, parent.transform.position, Color.yellow);
            Bee currentBee = other.gameObject.GetComponent<Bee>();
            currentBee.stunned = true;
            currentBee.rb.velocity = (currentBee.transform.position - transform.position).normalized * knockBackForce;

        }
    }
}

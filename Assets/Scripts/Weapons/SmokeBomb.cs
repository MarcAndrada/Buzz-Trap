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
            Bee currentBee = other.gameObject.GetComponent<Bee>();
            if (!currentBee || currentBee.beeType == Bee.BeeType.QUEEN)
                return;

            currentBee.stunned = true;
            currentBee.rb.velocity = (currentBee.transform.position - transform.position).normalized * knockBackForce;

        }
    }
}

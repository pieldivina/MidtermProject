using UnityEngine;

public class HomeBehavior : MonoBehaviour
{
    public GameObject Frog;

    private void Start()
    {
        Frog.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!Frog.activeSelf)
            {
                Debug.Log("Frog activated at home.");
                Frog.SetActive(true);

                FroggerBehavior frogger = other.GetComponent<FroggerBehavior>();
                if (frogger != null)
                {
                    frogger.gameObject.SetActive(false);
                    GameBehavior.Instance.HomeOccupied();
                }
            }
        }
    }
}

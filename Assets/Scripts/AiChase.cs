using UnityEngine;

public class AiChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    private Rigidbody2D rb;
    private float distance;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Start finding players after a short delay to ensure they've spawned
        InvokeRepeating("FindNearestPlayer", 0.5f, 0.5f);
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Make sure your player prefab has this tag
        float closestDistance = Mathf.Infinity;
        
        foreach (GameObject p in players)
        {
            distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                player = p;
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector2 direction = player.transform.position - transform.position;
            direction.Normalize(); //ça iniatilse sa longueur à  1 //
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //ça touve l angle entre les deux //
            if (distance < 10)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position,
                    speed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(Vector3.forward * angle);
            }
        }
    }
}

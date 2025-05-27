using UnityEngine;

public class Face : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    public float fallForce = 5f;
    private bool isHit = false;
   

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Move the object downward
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        
        if ( collision.gameObject.CompareTag("Left") && gameObject.CompareTag("Left"))
        {
           
            isHit = true;
            GameManager.Instance.AddScore();
            GameManager.Instance.Coin.Play();
            // Debug.Log("Left");
            GameManager.Instance.poff.Play();
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(0, -fallForce);

        }

        if (collision.gameObject.CompareTag("Right") && gameObject.CompareTag("Right"))
        {
           
            isHit = true;
            GameManager.Instance.AddScore();
            GameManager.Instance.Coin.Play();
            //Debug.Log("right");
            GameManager.Instance.poff.Play();
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(2, -fallForce);
        }

        if (collision.gameObject.CompareTag("Right") && gameObject.CompareTag("Left"))
        {
           

            GameManager.Instance.NewGame.SetActive(true);
            GameManager.Instance.OldGame.SetActive(false);
            GameManager.Instance.GameOverReal.Play();
            GameManager.Instance.GameOVer();
        
        }
        if (collision.gameObject.CompareTag("Left") && gameObject.CompareTag("Right"))
        {
           
            GameManager.Instance.NewGame.SetActive(false);
            GameManager.Instance.OldGame.SetActive(true);
            GameManager.Instance.GameOverCartoon.Play();
            GameManager.Instance.GameOVer();
        
        }
        if (collision.gameObject.CompareTag("Enemy")  && !isHit)
        {
            int randomChoice = Random.Range(0, 2); // Returns 0 or 1

            if (randomChoice == 0)
            {
                GameManager.Instance.NewGame.SetActive(true);
                GameManager.Instance.OldGame.SetActive(false);
                GameManager.Instance.GameOverReal.Play();
            }
            else
            {
                GameManager.Instance.NewGame.SetActive(false);
                GameManager.Instance.OldGame.SetActive(true);
                GameManager.Instance.GameOverCartoon.Play();
            }
            GameManager.Instance.GameOVer();
        }



        }


}


using UnityEngine;
using UnityEngine.EventSystems;

public class BasePlayer : MonoBehaviour
{
    public float rotationSpeed = 180f;
    public float moveSpeed = 5f;
    public GameObject goku;

    private int direction = 1;
     public ParticleSystem ParticleSystem;
     public ParticleSystem collect;
   
    void Start()
    {

    }

    void Update()
    {
        if (!GameManager.Instance.GameState)
            return;

        HandleInput();
        HandleRotation();
        //UPDATE LOGIC
    }

    private void HandleInput()
    {


        // Mouse (editor/standalone)
        if (Input.GetMouseButtonDown(0))
        {
            // if pointer is over UI, bail out
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            GameManager.Instance.Tap.Play();
            ToggleDirection();
        }

        // Touch (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // check this finger ID against UI
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                GameManager.Instance.Tap.Play();
                direction *= -1;
                ToggleDirection();
            }
        }
    }

    private void ToggleDirection()
    {
        direction *= -1;  // flips between 1 and -1
                          // only change Y-axis rotation, keep X and Z at whatever they were
        float yAngle = (direction == 1) ? 0f : 180f;
        goku.transform.localRotation = Quaternion.Euler(
            goku.transform.localEulerAngles.x,
            yAngle,
            goku.transform.localEulerAngles.z
        );
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ball"))
        {
            collect.Play();
            GameManager.Instance.AddScore();
            GameManager.Instance.Coin.Play();
            collision.gameObject.SetActive(false);
            GameManager.Instance.ActivateRandomBall();
        }

        


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ParticleSystem.Play();
            collision.gameObject.SetActive(false);
            GameOver();
        }
    }


    private void HandleRotation()
    {
        // Rotate around Z axis automatically (no input needed)
        float zRot = rotationSpeed * Time.deltaTime * direction;
        // Apply rotation; negative for clockwise vs counter-clockwise
        transform.Rotate(0f, 0f, -zRot, Space.Self);
    }



    public void GameOver()
    {
        GameManager.Instance.GameOVer();
    }

    public void Reset()
    {

    }
}

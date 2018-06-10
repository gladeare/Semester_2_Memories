using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour {

    public float rotSpeed;
    public float moveSpeed;
    public int movementOption; //1 = wasd only; 2 = mouse + world position based movement; 3 = mouse + character based movement
    [Range(0, 100)]
    public float damping;

    Vector3 mousePos;
    Vector3 posOnScreen;
    public Rigidbody2D rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update() {
        //basic movement
        switch (movementOption) {

            case 1:
                if (Input.GetKey(KeyCode.A)) {
                    transform.Rotate(0f, 0f, rotSpeed);
                }
                if (Input.GetKey(KeyCode.D)) {
                    transform.Rotate(0f, 0f, -rotSpeed);
                }
                if (Input.GetKey(KeyCode.W)) {
                    transform.position = transform.position + (transform.right * (moveSpeed / 1000));
                }
                if (Input.GetKey(KeyCode.S)) {
                  transform.position = transform.position - (transform.right * (moveSpeed / 1000));
                }
                break;

            case 2:
                mousePos = Input.mousePosition;
                posOnScreen = Camera.main.WorldToScreenPoint(transform.position);
                float angle1 = Mathf.Atan2(mousePos.y - posOnScreen.y, mousePos.x - posOnScreen.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle1));
                if (Input.GetKey(KeyCode.A)){
                    transform.position = transform.position + (transform.up * (moveSpeed / 1000));
                }
                if (Input.GetKey(KeyCode.D)){
                    transform.position = transform.position - (transform.up * (moveSpeed / 1000));
                }
                if (Input.GetKey(KeyCode.W)){
                    transform.position = transform.position + (transform.right * (moveSpeed / 1000));
                }
                if (Input.GetKey(KeyCode.S)){
                    transform.position = transform.position - (transform.right * (moveSpeed / 1000));
                }
                break;

            case 3:
                mousePos = Input.mousePosition;
                posOnScreen = Camera.main.WorldToScreenPoint(transform.position);
                float angle2 = Mathf.Atan2(mousePos.y - posOnScreen.y, mousePos.x - posOnScreen.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle2));

                float x = 0;
                float y = 0;
                if (Input.GetKey(KeyCode.A)){
                    x -= 1;
                    //transform.position = new Vector3(transform.position.x - moveSpeed, transform.position.y, transform.position.z); //use these instead for non-equalized movement
                }
                if (Input.GetKey(KeyCode.D)){
                    x += 1;
                    //transform.position = new Vector3(transform.position.x + moveSpeed, transform.position.y, transform.position.z);
                }
                if (Input.GetKey(KeyCode.W)){
                    y += 1;
                    //transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed, transform.position.z);
                }
                if (Input.GetKey(KeyCode.S)){
                    y -= 1;
                    //transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed, transform.position.z);
                }
                //transform.position += new Vector3(x, y, 0).normalized * moveSpeed; //teleportation-based movement
                rb.AddForce(new Vector2(x, y).normalized * moveSpeed); //force-based movement
                break;
                
        }

        //damping
        rb.velocity = rb.velocity * (1 - damping / 100);
    }
}

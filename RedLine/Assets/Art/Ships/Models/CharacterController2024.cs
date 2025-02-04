using UnityEngine;

public class CharacterController2024 : MonoBehaviour
{
	Animator anim;
    float speed = 0f;

    public float RotationSpeed = 180.0f;
    public float MovementSpeed = 2.0f;

    [System.Serializable]
    public struct KeyTrigger
    {
        public KeyCode key;
        public string trigger;
    }
    public KeyTrigger[] triggers;

    void Start () {
		anim = GetComponentInChildren<Animator> ();
        

	}
	void Update()
	{
		var x = Input.GetAxis ("Horizontal") * Time.deltaTime * RotationSpeed;
		var z = Input.GetAxis ("Vertical") * Time.deltaTime * MovementSpeed;

		transform.Rotate (0, x, 0);
		transform.Translate (0, 0, z);

        if (Input.GetAxis("Vertical") > 0.2f)
        {
            speed += Time.deltaTime * 3f;
        }
        else
        {
            speed -= Time.deltaTime * 2.5f; 
        }

        speed = Mathf.Clamp(speed, 0f, 1f);
        anim.SetFloat("Blend", speed);

        foreach (KeyTrigger trigger in triggers)
            if (Input.GetKey(trigger.key))
                anim.SetTrigger(trigger.trigger);


    }
}
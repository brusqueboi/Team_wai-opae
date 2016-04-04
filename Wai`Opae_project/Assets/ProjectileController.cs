using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
	public Transform Target;
	public float firingAngle = 45.0f;
	public float gravity = 9.8f;
    public string buttonName;
    private int shot = 1;

	protected Transform Projectile;
	private Vector3 originalPos; // Debug. 

	void Awake()
	{
		Projectile = gameObject.transform;
		originalPos = gameObject.transform.position;
	}

	void Start()
	{

    }

    void Update()
    {
        if(shot == 1)
        {
            if (Input.GetButtonDown(buttonName) && buttonName.Equals("AButton"))
            {
                shot = 0;
                FireProjectile();
                Projectile.position = originalPos;
            }

        }
    }

	void FireProjectile()
	{
		if (Target != null)
		{
			StartCoroutine(SimulateProjectile());
		}
		else
		{
			Debug.Log("FireProjectile() failed: unspecified target");
		}
	}


	IEnumerator SimulateProjectile()
	{
        if (true)
        {
            //Sources:
            //http://en.wikipedia.org/wiki/Trajectory_of_a_projectile
            //http://forum.unity3d.com/threads/throw-an-object-along-a-parabola.158855/#post-1087673

            // Calculate distance to target
            float target_Distance = Vector3.Distance(Projectile.position, Target.position);

            // Calculate the velocity needed to throw the object to the target at specified angle.
            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

            // Extract the X  Y componenent of the velocity
            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

            // Calculate flight time.
            float flightDuration = target_Distance / Vx;

            // Rotate projectile to face the target.
            Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);

            float elapse_time = 0;

            while (elapse_time < flightDuration)
            {
                Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

                elapse_time += Time.deltaTime;

                yield return null;
            }

            // Reset and restart for demo.
            yield return new WaitForSeconds(2.0f);
            Projectile.position = originalPos;
            shot = 1;
        }
	}
}

using Unity.VisualScripting;
using UnityEngine;

public class AutonomousAgent : AiAgent

{

    [SerializeField] Movement movement;

    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;

    [Header("Wander")]

    [SerializeField] float wanderRadius = 1;

    [SerializeField] float wanderDistance = 1;

    [SerializeField] float wanderDisplacement = 1;



    float wanderAngle = 0.0f;



    void Start()

    {
        // random within circle degrees (random range 0.0f-360.0f) 

        wanderAngle = Random.Range(0, 360);

    }



    void Update()
    {
        bool hasTarget = false;
        if (seekPerception != null)
        {
            var gameobjects = seekPerception.GetGameObjects();
            if (gameobjects.Length > 0)
            {
                hasTarget = true;
                Vector3 force = Seek(gameobjects[0]);
                movement.ApplyForce(force);
            }
            foreach (var go in gameobjects)
            {
                Debug.DrawLine(transform.position, go.transform.position, Color.red);
            }
        }
        if (fleePerception != null)
        {
            var gameobjects = fleePerception.GetGameObjects();
            if (gameobjects.Length > 0)
            {
                hasTarget = true;
                Vector3 force = Flee(gameobjects[0]);
                movement.ApplyForce(force);
            }
            foreach (var go in gameobjects)
            {
                Debug.DrawLine(transform.position, go.transform.position, Color.green);
            }
        }

        if (!hasTarget)
        {
            Vector3 force = Wander();
            movement.ApplyForce(force);
        }

        transform.position = Utilities.Wrap(transform.position, new Vector3(-15, -15, -15), new Vector3(15, 15, 15));

        if (movement.Velocity.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(movement.Velocity, Vector3.up);
        }
    }

    Vector3 Seek(GameObject go) { 
        Vector3 direction = go.transform.position - transform.position;
        Vector3 force = GetSteeringForce(direction);
        return force;
    }
    Vector3 Flee(GameObject go) {
        Vector3 direction = transform.position - go.transform.position;
        Vector3 force = GetSteeringForce(direction);
        return force;
    }

    private Vector3 Wander()

    {

        

        wanderAngle += Random.Range(-wanderDisplacement, wanderDisplacement);
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 pointOnCircle = rotation * (Vector3.forward * wanderRadius);
        Vector3 circleCenter = movement.Velocity.normalized * wanderDistance;
        Vector3 force = GetSteeringForce(circleCenter + pointOnCircle);
        Debug.DrawLine(transform.position, transform.position + circleCenter, Color.blue);
        Debug.DrawLine(transform.position, transform.position + circleCenter + pointOnCircle, Color.red);
        return force;

    }

    Vector3 GetSteeringForce(Vector3 direction) {
    Vector3 desired = direction.normalized * movement.maxSpeed;
    Vector3 steering = desired - movement.Velocity;
    Vector3 force = Vector3.ClampMagnitude(steering, movement.maxForce);
    return force;
}

}

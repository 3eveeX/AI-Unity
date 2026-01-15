using UnityEngine;

public class AutonomousAgent : AiAgent

{

    [SerializeField] Movement movement;

    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;



    void Start()

    {



    }



    void Update()
    {
        if (seekPerception != null)
        {
            var gameobjects = seekPerception.GetGameObjects();
            if (gameobjects.Length > 0)
            {
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
                Vector3 force = Flee(gameobjects[0]);
                movement.ApplyForce(force);
            }
            foreach (var go in gameobjects)
            {
                Debug.DrawLine(transform.position, go.transform.position, Color.green);
            }
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

    Vector3 GetSteeringForce(Vector3 direction) {
    Vector3 desired = direction.normalized * movement.maxSpeed;
    Vector3 steering = desired - movement.Velocity;
    Vector3 force = Vector3.ClampMagnitude(steering, movement.maxForce);
    return force;
}

}

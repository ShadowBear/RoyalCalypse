using UnityEngine;
using System.Collections;

namespace PolygonArsenal
{
    public class PolygonProjectileRocket : PolygonProjectileScript
    {
        [Range(3,7)]
        [SerializeField] float explosionRadius = 5f;
        public LayerMask damageLayer;

        protected override void FixedUpdate()
        {
            RaycastHit hit;

            float rad;
            if (transform.GetComponent<SphereCollider>())
                rad = transform.GetComponent<SphereCollider>().radius;
            else
                rad = colliderRadius;

            Vector3 dir = transform.GetComponent<Rigidbody>().velocity;
            if (transform.GetComponent<Rigidbody>().useGravity)
                dir += Physics.gravity * Time.deltaTime;
            dir = dir.normalized;

            float dist = transform.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime;

            if (Physics.SphereCast(transform.position, rad, dir, out hit, dist))
            {
                if (hit.collider.isTrigger) return;
                transform.position = hit.point + (hit.normal * collideOffset);

                GameObject impactP = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;

                if (hit.transform.tag == "Destructible") // Projectile will destroy objects tagged as Destructible
                {
                    Destroy(hit.transform.gameObject);
                }else if(hit.transform.tag == "Enemy" || hit.transform.tag == "Player")
                {
                    Collider[] colliders = Physics.OverlapSphere(hit.transform.position, explosionRadius, damageLayer);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Health health = colliders[i].GetComponent<Health>();
                        if (!health) continue;
                        
                        int damageToApplie = CalculateDamage(colliders[i].transform.position);
                        health.Damage(damageToApplie);                        
                    }
                }

                foreach (GameObject trail in trailParticles)
                {
                    GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                    curTrail.transform.parent = null;
                    Destroy(curTrail, 3f);
                }
                Destroy(projectileParticle, 3f);
                Destroy(impactP, 3.5f);
                Destroy(gameObject);

                ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
                //Component at [0] is that of the parent i.e. this object (if there is any)
                for (int i = 1; i < trails.Length; i++)
                {

                    ParticleSystem trail = trails[i];

                    if (trail.gameObject.name.Contains("Trail"))
                    {
                        trail.transform.SetParent(null);
                        Destroy(trail.gameObject, 2f);
                    }
                }
            }
        }

        private int CalculateDamage(Vector3 targetPosition)
        {
            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
            float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

            // Calculate damage as this proportion of the maximum possible damage.
            float damageAmount = relativeDistance * damage;

            // Make sure that the minimum damage is always 0.
            damageAmount = Mathf.Max(0f, damageAmount);

            return (int)damageAmount;
        }
    }
}
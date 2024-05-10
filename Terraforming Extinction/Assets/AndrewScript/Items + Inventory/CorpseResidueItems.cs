using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CorpseResidueItems : Items
{
    public CorpseResidueSO CorpseResidueStats;
    public int Damage { get; set; }
    public int DamageSpeed { get; set; }
    public int Duration { get; set; }
    public int Delay { get; set; }
    public FertilizerTypes[] FertilizedTargets { get; set; }
    public ItemSO[] ItemTargets { get; set; }
    private Dictionary<GameObject, float> collisionDuration = new();
    private Dictionary<GameObject, float> collisionSpeed = new();

    private void Start()
    {
        Damage = CorpseResidueStats.Damage;
        DamageSpeed = CorpseResidueStats.DamageSpeed;
        Duration = CorpseResidueStats.Duration;
        Delay = CorpseResidueStats.Delay;
        FertilizedTargets = CorpseResidueStats.FertilizedTarget;
        ItemTargets = CorpseResidueStats.ItemTargets;
        GameImage = CorpseResidueStats.GameImage;
        gameObject.GetComponent<SpriteRenderer>().sprite = GameImage;


        ResizeCollider();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Plant")
        {
            var collisionScript = collision.gameObject.GetComponent<UprooterManager>();
            bool inFertilizedTargets = false;

            //check if the uprooter has fertilizer level fit in fertilized target 
            foreach(FertilizerTypes fertilizedTarget in FertilizedTargets)
            {
                if (collisionScript.FertilizerLevel[(int)fertilizedTarget] > 0)
                {
                    inFertilizedTargets = true;
                    break;
                }
            }

            //if uprooter fertilized state is in the target
            if (inFertilizedTargets)
            {
                if (!collisionDuration.ContainsKey(collision.gameObject))
                {
                    collisionDuration.Add(collision.gameObject, 0f);
                    collisionSpeed.Add(collision.gameObject, 0f);
                }
                else
                {
                    collisionDuration[collision.gameObject] += Time.deltaTime;
                    
                    Debug.Log("Collision duration with " + collision.gameObject.name + ": " + collisionDuration[collision.gameObject]);
                    float durationOfCollision = collisionDuration[collision.gameObject];
                    
                    //past the delay
                    if(durationOfCollision > Delay)
                    {
                        //this one resets everytime attack is done
                        collisionSpeed[collision.gameObject] += Time.deltaTime;
                        float loopedDurationOfCollision = collisionSpeed[collision.gameObject];

                        //hit the damagespeed
                        if(loopedDurationOfCollision > DamageSpeed)
                        {
                            //Example of manipulating the metrics
                            collisionScript.Health -= Damage;
                            Debug.Log(collisionScript.Health);
                            //reset
                            collisionSpeed[collision.gameObject] = 0f;
                        }

                    }

                    //once the duration is over, destroy itself
                    if(durationOfCollision >= Duration)
                    {
                        Destroy(gameObject);
                    }

                }
            }
        }
    }
}

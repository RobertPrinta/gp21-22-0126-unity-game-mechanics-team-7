using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedTeleport : MonoBehaviour{
   [SerializeField] TeleportOrb teleportOrb;
   [SerializeField] float teleportCooldown;
   [SerializeField] AnimationCurve teleportOrbCurve;
   [SerializeField] float orbSpeed;
   [SerializeField] float orbDuration;
   [SerializeField] float orbDeathTimer;

   Rigidbody teleportRigibody;

   CommandContainer commandContainer;
   bool canTeleport = true;
   float orbStart;
   Vector3 orbDirection;

   void Awake(){
      commandContainer = GetComponentInChildren<CommandContainer>();
      orbDirection = transform.right;
   }

   void Start(){
      teleportRigibody = teleportOrb.GetComponent<Rigidbody>();
   }


   void FixedUpdate(){
      // if (commandContainer.walkCommand > 0 ){
      //    orbDirection = transform.right;
      // }
      // else if (commandContainer.walkCommand < 0){
      //    orbDirection = -transform.right;
      // }
      if (commandContainer.chargedTeleportDownCommand && canTeleport){
         teleportOrb.gameObject.SetActive(true);
         teleportRigibody.velocity = Vector3.zero;
         orbStart = Time.time;
         StartCoroutine(ChargedTeleportCooldown());
         StartCoroutine(teleportOrbDeathTimer());
      }

      if (commandContainer.chargedTeleportCommand){
         SetTeleportOrbVelocity();
      }

      if (commandContainer.chargedTeleportUpCommand){
         if (teleportOrb.gameObject.activeInHierarchy && !teleportOrb.isInsideEntity){
            transform.position = teleportOrb.transform.position;
           
         } 
         teleportOrb.gameObject.SetActive(false);
         StopCoroutine(teleportOrbDeathTimer());
      }
   }

   IEnumerator ChargedTeleportCooldown(){
      canTeleport = false;
      yield return new WaitForSeconds(teleportCooldown);
      canTeleport = true;
   }

   IEnumerator teleportOrbDeathTimer(){
      yield return new WaitForSeconds(orbDeathTimer);
      teleportOrb.gameObject.SetActive(false);
   }
   
   void SetTeleportOrbVelocity(){
      float t = Mathf.InverseLerp(this.orbStart, this.orbStart + orbDuration, Time.time); // time between 0 and 1
      var orbStrenght = teleportOrbCurve.Evaluate(t);
      teleportRigibody.velocity = orbDirection * (orbStrenght * orbSpeed);
   }
}

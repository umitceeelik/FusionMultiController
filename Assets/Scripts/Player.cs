using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Reflection;

namespace FusionMultiController
{
    public class Player : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototype _cc;

        [Header("Animation")]
        public Animator characterAnimator;

        float walkSpeed = 0;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterControllerPrototype>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.direction.Normalize();
                _cc.Move(5 * data.direction * Runner.DeltaTime);

                Vector2 walkVector = new Vector2(_cc.Velocity.x, _cc.Velocity.z);
                walkVector.Normalize();

                walkSpeed = Mathf.Lerp(walkSpeed, Mathf.Clamp01(walkVector.magnitude), Runner.DeltaTime * 5);

                characterAnimator.SetFloat("MovementSpeed", walkSpeed);

                if (data.isJumping && _cc.IsGrounded)
                {
                    Debug.Log("girdi");
                    _cc.Jump();
                    characterAnimator.SetBool("Jump", true);
                }

                if (!data.isJumping && _cc.IsGrounded && data.isDancing)
                {
                    OnDanceAnimationTriggered(data.danceAnimIndex);
                }
            }
        }

        public void OnDanceAnimationTriggered(int index)
        {
            characterAnimator.SetBool("danceAnim", true);
            characterAnimator.SetInteger("danceAnimIndex", index);
        }
        public virtual void OnDancenimationTriggered(int index)
        {
            characterAnimator.SetBool("danceAnim", true);
            characterAnimator.SetInteger("danceAnimIndex", index);
        }

        // Working the end of jumping animation as an animation event.
        public void FinishJumpAnim()
        {
            characterAnimator.SetBool("Jump", false);
        }

        // Working the end of dancing animation as an animation event.
        public void FinishDanceAnim()
        {
            characterAnimator.SetBool("danceAnim", false);
        }
    }
}


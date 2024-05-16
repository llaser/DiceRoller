
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Llaser.DiceRoller
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DiceRoller : UdonSharpBehaviour
    {
        [SerializeField]
        private GameObject[] dice;

        /// <summary>
        /// このコライダーはStartでTriggerにされ、無効化される
        /// ダイスがこの中にあるかの判定時のみ有効化される
        /// </summary>
        [SerializeField, Tooltip("このコライダー内のダイスを振る")]
        private Collider referenceCollider;

        private const float DistaceThreshold = 0.001f;

        [SerializeField, Tooltip("ダイスの出現先")]
        private Transform rollTarget;
        
        [SerializeField, Tooltip("ダイスが出現する範囲(球状), rollTargetのローカルスケール")]
        private float rollRadius;

        [SerializeField, Tooltip("ダイス出現時の角速度の大きさ")]
        private float rollAngularVelocityMagnitude;

        [SerializeField, Tooltip("ダイス出現時の速度")]
        private Vector3 rollVelocity;

        private void Start()
        {
            if (referenceCollider)
            {
                referenceCollider.isTrigger = true;
                referenceCollider.enabled = false;
            }
            else
            {
                Debug.LogError("[DiceRoller] referenceCollider is invalid");
            }
            if (dice == null || dice.Length == 0)
            {
                Debug.LogError("[DiceRoller] no die is set");
            }
            if (!rollTarget)
            {
                Debug.LogError("[DiceRoller] rollTarget is invalid");
            }
        }

        public override void Interact()
        {
            RollDice();
        }

        /// <summary>
        /// referenceCollider内のダイスをrollTarget近辺にランダムな向きで移動する.
        /// referenceCollider内の判定は、ダイスのコライダー中心で実施.
        /// </summary>
        public void RollDice()
        {
            if (dice == null || !referenceCollider || !rollTarget) { return; }
            // Collider.ClosestPoint()のために一時的に有効化
            referenceCollider.enabled = true;

            foreach (GameObject die in dice)
            {
                if (!die) { continue; }

                Collider dieCollider = die.GetComponent<Collider>();
                Vector3 dieCenter = dieCollider ? dieCollider.bounds.center : die.transform.position;
                Vector3 closestPoint = referenceCollider.ClosestPoint(dieCenter);
                if (Vector3.Distance(dieCenter, closestPoint) >= DistaceThreshold) { continue; }

                Vector3 targetPosition = rollTarget.TransformPoint(Random.insideUnitSphere * rollRadius);
                Quaternion targetRotation = Random.rotationUniform;
                Vector3 newAngularVelocity = Random.onUnitSphere * rollAngularVelocityMagnitude;

                if (!Networking.IsOwner(die)) { Networking.SetOwner(Networking.LocalPlayer, die); }

                Rigidbody rigidbody = die.GetComponent<Rigidbody>();
                if (rigidbody)
                {
                    rigidbody.position = targetPosition;
                    rigidbody.rotation = targetRotation;
                    rigidbody.angularVelocity = newAngularVelocity;
                    rigidbody.velocity = rollVelocity;
                }
                else
                {
                    die.transform.SetPositionAndRotation(targetPosition, targetRotation);
                }

                VRCObjectSync objectSync = die.GetComponent<VRCObjectSync>();
                if (objectSync)
                {
                    objectSync.FlagDiscontinuity();
                }
            }
            referenceCollider.enabled = false;
        }
    }
}

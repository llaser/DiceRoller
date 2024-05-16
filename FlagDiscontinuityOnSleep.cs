
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;


namespace Llaser.DiceRoller
{
    /// <summary>
    /// 受信側の挙動を安定させるため、Sleepした次の同期時にFlagDiscontinuity()を実行.
    /// VRCObjectSyncの挙動に依存しているため、今後正常に機能しなくなる可能性あり.
    /// </summary>
    [RequireComponent(typeof(VRCObjectSync))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class FlagDiscontinuityOnSleep : UdonSharpBehaviour
    {
        private bool _wasSleepingAtLastSync;

        private bool _hasSentFlagDiscontinuity;

        private Rigidbody _rigidbody;

        public override void OnPreSerialization()
        {
            if (!_rigidbody) { _rigidbody = GetComponent<Rigidbody>(); }
            if (!_rigidbody) { return; }
            if (_rigidbody.IsSleeping())
            {
                if (_hasSentFlagDiscontinuity) { return; }
                if (!_wasSleepingAtLastSync)
                {
                    _wasSleepingAtLastSync = true;
                    return;
                }
                VRCObjectSync objectSync = GetComponent<VRCObjectSync>();
                if (!objectSync) { return; }
                objectSync.FlagDiscontinuity();
                _hasSentFlagDiscontinuity = true;
            }
            else
            {
                _wasSleepingAtLastSync = false;
                _hasSentFlagDiscontinuity = false;
            }
        }
    }
}

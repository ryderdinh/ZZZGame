using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace DTT.Networking.ConnectionStatus
{
    /// <summary>
    /// Stores internet status targets which can be pinged.
    /// </summary>
    internal class InternetStatusWorker : MonoBehaviour
    {
        /// <summary>
        /// The serialized targets set by the user.
        /// </summary>
        [SerializeField]
        private List<InternetStatusTarget> _targets = new List<InternetStatusTarget>()
        {
            InternetStatusTarget.Standard
        };

        /// <summary>
        /// The index of the default target to use.
        /// </summary>
        [SerializeField]
        private int _defaultTargetIndex = 0;

        /// <summary>
        /// The index of the default target to use.
        /// </summary>
        public int DefaultTargetIndex => _defaultTargetIndex;

        /// <summary>
        /// The amount of targets stored by the handler.
        /// </summary>
        public int TargetCount => _targets.Count;

        /// <summary>
        /// Returns an internet status target based on name or address. Returns null if the
        /// target could not be found. 
        /// </summary>
        /// <param name="nameOrAddress">The name or address of the target.</param>
        /// <returns>The internet status target. Null if not found.</returns>
        public InternetStatusTarget GetTarget(string nameOrAddress)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                InternetStatusTarget target = _targets[i];
                if (target.Name == nameOrAddress || target.Address == nameOrAddress)
                    return target;
            }

            return null;
        }

        /// <summary>
        /// Returns a target at a given index. 
        /// </summary>
        /// <param name="index">The zero based index of the target in the stored list of targets.</param>
        /// <returns>The target.</returns>
        public InternetStatusTarget GetTarget(int index) => _targets[index];
    }
}

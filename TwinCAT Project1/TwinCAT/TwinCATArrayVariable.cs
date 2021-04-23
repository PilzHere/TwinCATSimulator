using System;
using TwinCAT.Ads;

namespace TwinCAT
{
    class TwinCATArrayVariable<T> : TwinCATVariable<T>
    {
        /// <summary>Number of elements in the array.</summary>
        private int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwinCATVariable<T>"/>
        /// class.
        /// </summary>
        /// <remark>
        /// Intended to be called only from an instance of
        /// <see cref="TwinCATVariableBuilder<T>"/>
        /// </remark>
        /// <param name="adsClient">TwinCAT ADS communication client.</param>
        /// <param name="name">
        /// Full name of the mirrored variable on the TwinCAT server.
        /// </param>
        /// <param name="length">
        /// Length of the variable in case it is an array, otherwise 1.
        /// </param>
        /// <param name="cycleTime">
        /// TwinCAT ADS server scans variables for updates this often.
        /// Expressed in milliseconds.
        /// </param>
        /// <param name="maxDelay">
        /// The maximum period during which the TwinCAT ADS server can
        /// aggregate notification events originating from variable updates.
        /// Expressed in milliseconds.
        /// </param>
        internal TwinCATArrayVariable(TcAdsClient adsClient,
                                      string name,
                                      int length,
                                      int cycleTime,
                                      int maxDelay) :
        base(adsClient, name, cycleTime, maxDelay)
        {
            this.length = length;
            // Create an array of requested length;
            this.value = (T)(object)Array.CreateInstance(typeof(T).GetElementType(), length);
        }

        /// <summary>
        /// The Value property represents the value of the replica. If polling
        /// is enabled the value is fetched from the variable on the TwinCAT
        /// ADS server every time read. When written the variable on the
        /// TwinCAT ADS server is updated.
        /// </summary>
        public override T Value
        {
            get
            {
                if (polling)
                {
                    SetValue(adsClient.ReadAny(handle, typeof(T), new int[] { length }));
                }
                return (T)((ICloneable)this.value).Clone();
            }
            set
            {
                DateTime now = DateTime.Now;
                // Throw an exception if it is too short or long.
                var array = value as Array;
                if (array.Length != length)
                {
                    throw new TwinCATException("The source array consists of " + array.Length + " element(s), but " + name + " requires " + length + " element(s).");
                }
                // When polling is disabled, only update value if it differs
                // from the current one to prevent spurious updates.
                if (polling || !System.Collections.Generic.EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    lock (forcedIOLock)
                    {
                        if (!forcedIO)
                        {
                            // Copy contents of source array to target array.
                            var thisArray = this.value as Array;
                            ((System.Collections.ICollection)value).CopyTo(thisArray, 0);
                            adsClient.WriteAny(handle, value);
                        }
                    }
                    OnUpdateEvent(now);
                }
            }
        }

        /// <summary>
        /// Acquire the handle to the registered notification of changes to
        /// the replicated variable, if not already present.
        /// </summary>
        protected override void AcquireDeviceNotificationHandle()
        {
            notificationHandle = adsClient.AddDeviceNotificationEx(name, AdsTransMode.OnChange, cycleTime, maxDelay, (SetValueDelegate)SetValue, typeof(T), new int[] { length });
        }
    }
}

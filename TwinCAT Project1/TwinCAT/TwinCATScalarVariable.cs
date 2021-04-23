using System;
using TwinCAT.Ads;

namespace TwinCAT
{
    class TwinCATScalarVariable<T> : TwinCATVariable<T>
    {
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
        /// <param name="cycleTime">
        /// TwinCAT ADS server scans variables for updates this often.
        /// Expressed in milliseconds.
        /// </param>
        /// <param name="maxDelay">
        /// The maximum period during which the TwinCAT ADS server can
        /// aggregate notification events originating from variable updates.
        /// Expressed in milliseconds.
        /// </param>
        internal TwinCATScalarVariable(TcAdsClient adsClient,
                                       string name,
                                       int cycleTime,
                                       int maxDelay) :
            base(adsClient, name, cycleTime, maxDelay)
        {
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
                    SetValue(adsClient.ReadAny(handle, typeof(T)));
                }
                return this.value;
            }
            set
            {
                DateTime now = DateTime.Now;
                // When polling is disabled, only update value if it differs
                // from the current one to prevent spurious updates.
                if (polling || !System.Collections.Generic.EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    lock (forcedIOLock)
                    {
                        if (!forcedIO)
                        {
                            this.value = value;
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
            notificationHandle = adsClient.AddDeviceNotificationEx(name, AdsTransMode.OnChange, cycleTime, maxDelay, (SetValueDelegate)SetValue, typeof(T));
        }
    }
}

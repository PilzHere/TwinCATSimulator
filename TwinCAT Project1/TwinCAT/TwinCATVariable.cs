using System;
using TwinCAT.Ads;

namespace TwinCAT
{
    /// <summary>
    /// Delegate used when notified by the TwinCAT ADS client to update the
    /// value of an instance of <see cref="TwinCATVariable<T>"/>.
    /// </summary>
    /// <param name="value">The new value of the variable.</param>
    delegate void SetValueDelegate(object value);

    /// <summary>
    /// The representation of a variable replicating a variable on the TwinCAT
    /// server. The replicated variable can either be updated when changed on
    /// the server, or every time read.
    /// </summary>
    public abstract class TwinCATVariable<T>
    {
        /// <summary>TwinCAT ADS communication client.</summary>
        protected TcAdsClient adsClient;
        /// <summary>
        /// Full name of the variable to replicate on the TwinCAT server.
        /// </summary>
        protected string name;
        /// <summary>
        /// Handle to the replicated variable on the TwinCAT server.
        /// </summary>
        protected int handle;
        /// <summary>
        /// Handle to the registered notification of changes to the
        /// replicated variable.
        /// </summary>
        protected int? notificationHandle = null;

        /// <summary>
        /// Whether the TwinCAT ADS server is polled for changes to the
        /// variable or if changes are pushed to the client.
        /// </summary>
        protected bool polling = false;
        /// <summary>Value of the replicated variable.</summary>
        protected T value;
        /// <summary>
        /// Event handler(s) invoked when a change of the replicated variable
        /// is detected.
        /// </summary>
        private event EventHandler<TwinCATVariableUpdatedEventArgs> updateEvent;
        /// <summary>Lock used for setting the forcedIO flag.</summary>
        protected readonly object forcedIOLock = new object();
        /// <summary>Flag keeping track of status of the forced io.</summary>
        protected bool forcedIO = false;

        /// <summary>
        /// TwinCAT ADS server scans variables for updates this often.
        /// Expressed in milliseconds.
        /// </summary>
        protected int cycleTime;
        /// <summary>
        /// The maximum period during which the TwinCAT ADS server can
        /// aggregate notification events originating from variable updates.
        /// Expressed in milliseconds.
        /// </summary>
        protected int maxDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwinCATVariable<T>"/>
        /// class.</summary>
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
        protected TwinCATVariable(TcAdsClient adsClient,
                                  string name,
                                  int cycleTime,
                                  int maxDelay)
        {
            this.adsClient = adsClient;
            this.name = name;
            this.cycleTime = cycleTime;
            this.maxDelay = maxDelay;
            handle = adsClient.CreateVariableHandle(name);
        }

        /// <summary>
        /// Destructs and instance of the <see cref="TwinCATVariable<T>"/>
        /// class.</summary>
        /// <remark>
        /// The purpose of the destructor is to release TwinCAT ADS client
        /// handles acquired.
        /// </remark>
        ~TwinCATVariable()
        {
            ReleaseDeviceNotificationHandle();
            adsClient?.DeleteVariableHandle(handle);
        }

        /// <summary>
        /// The Polling property represents whether the TwinCAT ADS server is
        /// updating the value of the replica whenever the replicated variable
        /// is updated or if the replicated variable is polled every time the
        /// replica is read.
        /// </summary>
        public bool Polling
        {
            get { return polling; }
            set
            {
                polling = value;
                // Only subscribe to events signaling changes to the
                // replicated variable when not in polling mode.
                if (polling)
                {
                    ReleaseDeviceNotificationHandle();
                }
                else
                {
                    AcquireDeviceNotificationHandle();
                }
            }
        }

        /// <summary>
        /// The Value property represents the value of the replica. If polling
        /// is enabled the value is fetched from the variable on the TwinCAT
        /// ADS server every time read. When written the variable on the
        /// TwinCAT ADS server is updated.
        /// </summary>
        public abstract T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the value of the variable and notifies all observers, if any.
        /// </summary>
        /// <param name="value">Value to set.</param>
        public void SetValue(object value)
        {
            DateTime now = DateTime.Now;
            // Only update value if it differs from the current one to prevent
            // spurious updateEvents.
            if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(this.value, (T)value))
            {
                this.value = (T)value;
                OnUpdateEvent(now);
            }
        }

        /// <summary>
        /// Force the TwinCat variable to the state of value
        /// </summary>
        /// <param name="value">Value to set</param>
        public void Force(T value)
        {
            lock (forcedIOLock)
            {
                forcedIO = false;
                Value = value;
                forcedIO = true;
            }
        }

        /// <summary>
        /// Release the forced IO
        /// </summary>
        public void Release()
        {
            lock (forcedIOLock)
            {
                forcedIO = false;
            }
        }

        /// <summary>
        /// The UpdateEvent property is a list of delegates invoked when the
        /// value of the replicated variable is updated.
        /// </summary>
        public event EventHandler<TwinCATVariableUpdatedEventArgs> UpdateEvent
        {
            add
            {
                if (updateEvent == null)
                {
                    AcquireDeviceNotificationHandle();
                }
                updateEvent += value;
                OnUpdateEvent(DateTime.Now);
            }
            remove
            {
                updateEvent -= value;
                if (updateEvent == null)
                {
                    ReleaseDeviceNotificationHandle();
                }
            }
        }

        /// <summary>
        /// Acquire the handle to the registered notification of changes to
        /// the replicated variable, if not already present.
        /// </summary>
        protected abstract void AcquireDeviceNotificationHandle();

        /// <summary>
        /// Release the handle to the registered notification of changes to
        /// the replicated variable, if present.
        /// </summary>
        private void ReleaseDeviceNotificationHandle()
        {
            if (notificationHandle != null)
            {
                adsClient.DeleteDeviceNotification(notificationHandle.Value);
            }
        }
        
        /// <summary>
        /// Notify all observers that the value of the replica has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdateEvent(DateTime now)
        {
            updateEvent?.Invoke(this, new TwinCATVariableUpdatedEventArgs(now));
        }
    }
}

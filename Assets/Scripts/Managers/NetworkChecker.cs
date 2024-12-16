using DTT.Networking.ConnectionStatus;
using DTT.Singletons;
using Services;
using UniRx;

public class NetworkChecker : SingletonBehaviour<NetworkChecker>
{
    public bool isOnline = true;

    public void OnStart()
    {
        InternetStatusManager.DefaultTarget.StatusUpdate += newStatus =>
        {
            switch (newStatus)
            {
                case InternetStatus.ONLINE:
                    HandleOnOnline();
                    break;
                case InternetStatus.OFFLINE:
                case InternetStatus.UNKNOWN:
                    HandleOnOffline();
                    break;
                case InternetStatus.RECONNECTING:
                    HandleOnReconnect();
                    break;
            }
        };
    }

    private void HandleOnOffline()
    {
        isOnline = false;
        MessageBroker.Default.Publish(new NetworkStatusChange { Status = isOnline });
    }

    private void HandleOnOnline()
    {
        isOnline = true;
        MessageBroker.Default.Publish(new NetworkStatusChange { Status = isOnline });
    }

    private void HandleOnReconnect()
    {
    }
}
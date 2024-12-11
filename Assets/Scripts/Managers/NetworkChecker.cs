using DTT.Networking.ConnectionStatus;
using DTT.Singletons;

public class NetworkChecker : SingletonBehaviour<NetworkChecker>
{
    public bool isOnline = true;

    private void Start()
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
    }

    private void HandleOnOnline()
    {
        isOnline = true;
    }

    private void HandleOnReconnect()
    {
    }
}
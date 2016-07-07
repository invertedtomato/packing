using System.ComponentModel;

namespace InvertedTomato.Feather {
    public enum DisconnectionType {
        [Description("No message received within the keep-alive window.")]
        KeepAliveTimeout,

        [Description("The connection was unexpectedly interrupted.")]
        ConnectionInterupted,

        [Description("The local side closed the connection.")]
        LocalDisconnection,

        [Description("The remote side closed the connection.")]
        RemoteDisconnection
    }
}

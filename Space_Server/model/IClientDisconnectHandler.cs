namespace Space_Server.model {
    public interface IClientDisconnectHandler {
        void Disconnect(NetworkPlayer player);
    }
}
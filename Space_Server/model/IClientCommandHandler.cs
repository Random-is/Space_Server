namespace Space_Server.model {
    public interface IClientCommandHandler {
        void Handle(NetworkPlayer player, string[] command);
    }
}
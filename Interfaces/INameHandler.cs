namespace kartlegging_websocket_prosjekt_backend.Interfaces;

public interface INameHandler
{
    string GetNewName();
    string GenerateInitialUserName();
    string RerollName(string displayName);
    int MessageCount(string userName);

    void NameActive(string userName);
}
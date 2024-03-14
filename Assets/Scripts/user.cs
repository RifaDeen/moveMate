[System.Serializable]
public class User
{
    public string UserId;
    public string Username;
    public string Email;
    

    public User(){}  //default for json serialization

    public User(string userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    
    }
}

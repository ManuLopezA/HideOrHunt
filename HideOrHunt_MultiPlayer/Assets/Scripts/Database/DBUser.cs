public class UserLogin
{
    public string username { get; set; }
    public string password { get; set; }
}

public class UserLoginResponse
{
    public string msg { get; set; }
    public string status { get; set; }
    public bool isValid { get; set; }
}

public class UserScore
{
    public string username { get; set; }
    public int score { get; set; }
}

public class ValidatePassword
{
    public string password { get; set; }
}

public class ValidatePasswordResponse
{
    public bool isValid;
}

public class UsersSubmit
{
    public string password { get; set; }
    public UserScore[] data { get; set; }
}

public class UsersSubmitResponse
{
    public bool success;
}

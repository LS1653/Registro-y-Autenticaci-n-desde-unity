using System;

[Serializable]
public class User
{
    public string _id;
    public string username;
    public bool estado;
    public UserDat data;
}

[Serializable]
public class UserDat
{
    public int score;
}

[Serializable]
public class UserListResponse
{
    public User[] usuarios;
}

[System.Serializable]
public class UserUpdateData
{
    public string username;
    public UserDat data;
}

[System.Serializable]
public class UserUpdateResponse
{
    public User usuario;
}
using System.Net;
using System.Text;
using Newtonsoft.Json;

string ip = "127.0.0.1";
int port = 1234;

List<Usuario> MisUsuarios = new List<Usuario>();
Dictionary<string, string> Tokens = new Dictionary<string, string>();

HttpListener listener = new HttpListener();

listener.Prefixes.Add($"http://{ip}:{port}/");

listener.Start();

Console.WriteLine($"Servidor escuchando en http://{ip}:{port}/");

while (true)
{
    var context = await listener.GetContextAsync();

    await HandleRequest(context);
}

async Task HandleRequest(HttpListenerContext context)
{
    var request = context.Request;
    var response = context.Response;

    Console.WriteLine(
        $"Recibido: {request.HttpMethod} {request.RawUrl}"
    );

    // Endpoint de Registro
    if (request.HttpMethod == "POST" &&
        request.RawUrl == "/api/usuarios")
    {
        var reader =
            new StreamReader(
                request.InputStream,
                request.ContentEncoding
            );

        string requestBody =
            await reader.ReadToEndAsync();

        AuthData authData =
            JsonConvert.DeserializeObject<AuthData>(
                requestBody
            );

        if (authData == null)
        {
            var error = new ErrorMessage
            {
                msg = "Debe enviar el usuario",
                field = "username"
            };

            string contentResponse =
                JsonConvert.SerializeObject(error);

            await SendResponse(
                response,
                400,
                contentResponse
            );

            return;
        }

        if (string.IsNullOrEmpty(authData.username))
        {
            var error = new ErrorMessage
            {
                msg = "Debe enviar el usuario",
                field = "username"
            };

            string contentResponse =
                JsonConvert.SerializeObject(error);

            await SendResponse(
                response,
                400,
                contentResponse
            );

            return;
        }

        if (string.IsNullOrEmpty(authData.password))
        {
            var error = new ErrorMessage
            {
                msg = "Debe enviar la contraseña",
                field = "password"
            };

            string contentResponse =
                JsonConvert.SerializeObject(error);

            await SendResponse(
                response,
                400,
                contentResponse
            );

            return;
        }

        if (MisUsuarios.Any(
                u => u.username == authData.username))
        {
            var error = new ErrorMessage
            {
                msg = "Ya existe usuario con ese username",
                field = "username"
            };

            string contentResponse =
                JsonConvert.SerializeObject(error);

            await SendResponse(
                response,
                400,
                contentResponse
            );

            return;
        }

        Usuario nuevoUsuario =
            new Usuario
            {
                _id = Guid.NewGuid().ToString(),
                username = authData.username,
                password = authData.password,
                estado = true,
                data = new UserData()
            };

        MisUsuarios.Add(nuevoUsuario);

        string jsonResponse =
            JsonConvert.SerializeObject(
                new RegistroResponse(nuevoUsuario)
            );

        await SendResponse(
            response,
            200,
            jsonResponse
        );

        return;
    }

    // Endpoint de Login
    if (request.HttpMethod == "POST" &&
        request.RawUrl == "/api/auth/login")
    {
        var reader =
            new StreamReader(
                request.InputStream,
                request.ContentEncoding
            );
    
        string requestBody =
            await reader.ReadToEndAsync();
    
        AuthData authData =
            JsonConvert.DeserializeObject<AuthData>(
                requestBody
            );
    
        if (authData == null)
        {
            var error = new ErrorMessage
            {
                msg = "Debe enviar el usuario",
                field = "username"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                400,
                contentResponse
            );
    
            return;
        }
    
        if (string.IsNullOrEmpty(authData.username))
        {
            var error = new ErrorMessage
            {
                msg = "Debe enviar el usuario",
                field = "username"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                400,
                contentResponse
            );
    
            return;
        }
    
        if (string.IsNullOrEmpty(authData.password))
        {
            var error = new ErrorMessage
            {
                msg = "Debe enviar la contraseña",
                field = "password"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                400,
                contentResponse
            );
    
            return;
        }
    
        Usuario usuario =
            MisUsuarios.FirstOrDefault(
                u => u.username == authData.username
            );
    
        if (usuario == null)
        {
            var error = new ErrorMessage
            {
                msg = "Usuario o contraseña incorrectos"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                401,
                contentResponse
            );
    
            return;
        }
    
        if (usuario.password != authData.password)
        {
            var error = new ErrorMessage
            {
                msg = "Usuario o contraseña incorrectos"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                401,
                contentResponse
            );
    
            return;
        }
    
        string token =
            Guid.NewGuid().ToString();
    
        Tokens[token] = usuario.username;
    
        string jsonResponse =
            JsonConvert.SerializeObject(
                new LoginResponse(usuario, token)
            );
    
        await SendResponse(
            response,
            200,
            jsonResponse
        );
    
        return;
    }

    // Endpoint para listar usuarios
    if (request.HttpMethod == "GET" &&
        request.RawUrl == "/api/usuarios")
    {
        string token = request.Headers["x-token"];
    
        Console.WriteLine(
            $"Token recibido: {token}"
        );
    
        if (string.IsNullOrEmpty(token))
        {
            var error = new ErrorMessage
            {
                msg = "No se proporcionó un token de autenticación"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                401,
                contentResponse
            );
    
            return;
        }
    
        if (!Tokens.ContainsKey(token))
        {
            var error = new ErrorMessage
            {
                msg = "Token no válido"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                401,
                contentResponse
            );
    
            return;
        }
    
        List<UsuarioDto> usuarios =
            MisUsuarios
            .Select(usuario => new UsuarioDto
            {
                _id = usuario._id,
                username = usuario.username,
                estado = usuario.estado,
                data = usuario.data
            })
            .ToList();
    
        var usersResponse = new UserListResponse
        {
            usuarios = usuarios
        };
    
        string jsonResponse =
            JsonConvert.SerializeObject(
                usersResponse
            );
    
        await SendResponse(
            response,
            200,
            jsonResponse
        );
    
        return;
    }

    // Endpoint para obtener el perfil de un usuario
    if (request.HttpMethod == "GET" &&
    request.RawUrl != null &&
    request.RawUrl.StartsWith("/api/usuarios/"))
    {
        string token = request.Headers["x-token"];
    
        Console.WriteLine(
            $"Token recibido: {token}"
        );
    
        if (string.IsNullOrEmpty(token))
        {
            var error = new ErrorMessage
            {
                msg = "No se proporcionó un token de autenticación"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                401,
                contentResponse
            );
    
            return;
        }
    
        if (!Tokens.ContainsKey(token))
        {
            var error = new ErrorMessage
            {
                msg = "Token no válido"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                401,
                contentResponse
            );
    
            return;
        }
    
        string username =
            request.RawUrl.Substring(
                "/api/usuarios/".Length
            );
    
        Usuario usuario =
            MisUsuarios.FirstOrDefault(
                u => u.username == username
            );
    
        if (usuario == null)
        {
            var error = new ErrorMessage
            {
                msg = "Usuario no encontrado"
            };
    
            string contentResponse =
                JsonConvert.SerializeObject(error);
    
            await SendResponse(
                response,
                404,
                contentResponse
            );
    
            return;
        }
    
        string jsonResponse =
            JsonConvert.SerializeObject(
                new LoginResponse(
                    usuario,
                    token
                )
            );
    
        await SendResponse(
            response,
            200,
            jsonResponse
        );
    
        return;
    }
}

async Task SendResponse(
    HttpListenerResponse response,
    int statusCode,
    string content)
{
    int contentLength =
        Encoding.UTF8.GetByteCount(content);

    response.ContentLength64 = contentLength;
    response.StatusCode = statusCode;
    response.ContentType = "application/json";

    var output = response.OutputStream;

    var buffer =
        Encoding.UTF8.GetBytes(content);

    await output.WriteAsync(
        buffer,
        0,
        buffer.Length
    );

    output.Close();
}

public class Usuario
{
    public string _id;
    public string username { get; set; }
    public string password { get; set; }
    public bool estado;
    public object data;
}

public class UserData
{
    public int score;
    public int level;
}

public class ErrorMessage
{
    public string msg;
    public string field;
}

public class AuthData
{
    public string username;
    public string password;
}

public class RegistroResponse
{
    public UsuarioDto usuario;
    public string token;

    public RegistroResponse(Usuario usuario)
    {
        this.usuario = new UsuarioDto
        {
            username = usuario.username,
            _id = usuario._id,
            data = usuario.data,
            estado = usuario.estado
        };

        this.token = "your_generated_token_here";
    }
}

public class LoginResponse
{
    public UsuarioDto usuario;
    public string token;

    public LoginResponse(
        Usuario usuario,
        string token)
    {
        this.usuario = new UsuarioDto
        {
            username = usuario.username,
            _id = usuario._id,
            data = usuario.data,
            estado = usuario.estado
        };

        this.token = token;
    }
}

public class UsuarioDto
{
    public string _id;
    public string username { get; set; }
    public bool estado;
    public object data;
}

public class UserListResponse
{
    public List<UsuarioDto> usuarios;
}

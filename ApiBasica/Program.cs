using System.Net;
using System.Text;
using Newtonsoft.Json;

string ip = "127.0.0.1";
int port = 1234;

List<Usuario> MisUsuarios = new List<Usuario>();

HttpListener listener = new HttpListener();

listener.Prefixes.Add($"http://{ip}:{port}/");

listener.Start();

Console.WriteLine($"Servidor escuchando en http://{ip}:{port}/");

while (true)
{
    var context = await listener.GetContextAsync();

    HandleRequest(context);
}

async void HandleRequest(HttpListenerContext context)
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

public class UsuarioDto
{
    public string _id;
    public string username { get; set; }
    public bool estado;
    public object data;
}

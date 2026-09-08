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
}

async void SendResponse(
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

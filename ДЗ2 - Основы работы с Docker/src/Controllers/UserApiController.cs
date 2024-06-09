using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;

namespace src.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
public class UserApiController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UserApiController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Create user
    /// </summary>
    /// <remarks>This can only be done by the logged in user.</remarks>
    /// <param name="body">Created user object</param>
    /// <response code="0">successful operation</response>
    [HttpPost]
    [Route("/api/v1/user")]
    [ValidateModelState]
    [SwaggerOperation("CreateUser")]
    public virtual IActionResult CreateUser([FromBody]User body)
    { 
        using var dataSource = Connect();
        using var cmd = dataSource.CreateCommand($@"
insert into public.""user""(
username, firstname, lastname, email, phone)
values (:username, :firstname, :lastname, :email, :phone)
returning id;
");
        cmd.Parameters.AddWithValue("username", body.Username);
        cmd.Parameters.AddWithValue("firstname", body.FirstName);
        cmd.Parameters.AddWithValue("lastname", body.LastName);
        cmd.Parameters.AddWithValue("email", body.Email);
        cmd.Parameters.AddWithValue("phone", body.Phone);
        var id = (int)cmd.ExecuteScalar();

        return Created(this.Url.Action(nameof(FindUserById), new { userId = id }), new { id });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>deletes a single user based on the ID supplied</remarks>
    /// <param name="userId">ID of user</param>
    /// <response code="204">user deleted</response>
    /// <response code="0">unexpected error</response>
    [HttpDelete]
    [Route("/api/v1/user/{userId}")]
    [ValidateModelState]
    [SwaggerOperation("DeleteUser")]
    [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "unexpected error")]
    public virtual IActionResult DeleteUser([FromRoute][Required]long? userId)
    { 
        using var dataSource = Connect();
        using var cmd = dataSource.CreateCommand($@"
delete from public.""user""
where id = :userId;
");
        cmd.Parameters.AddWithValue("userId", userId);
        cmd.ExecuteNonQuery();

        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Returns a user based on a single ID, if the user does not have access to the user</remarks>
    /// <param name="userId">ID of user</param>
    /// <response code="200">user response</response>
    /// <response code="0">unexpected error</response>
    [HttpGet]
    [Route("/api/v1/user/{userId}")]
    [ValidateModelState]
    [SwaggerOperation("FindUserById")]
    [SwaggerResponse(statusCode: 200, type: typeof(User), description: "user response")]
    [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "unexpected error")]
    public virtual IActionResult FindUserById([FromRoute][Required]long? userId)
    {
        return new ObjectResult(Select(userId.Value).Single());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Update user with User ID supplied</remarks>
    /// <param name="userId">ID of user</param>
    /// <param name="body"></param>
    /// <response code="200">user updated</response>
    /// <response code="0">unexpected error</response>
    [HttpPut]
    [Route("/api/v1/user/{userId}")]
    [ValidateModelState]
    [SwaggerOperation("UpdateUser")]
    [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "unexpected error")]
    public virtual IActionResult UpdateUser([FromRoute][Required]long? userId, [FromBody]User body)
    { 
        using var dataSource = Connect();
        using var cmd = dataSource.CreateCommand($@"
update public.""user""
set username = :username, firstname = :firstname, lastname = :lastname, email = :email, phone = :phone
where id = :userId;
");
        cmd.Parameters.AddWithValue("username", body.Username);
        cmd.Parameters.AddWithValue("firstname", body.FirstName);
        cmd.Parameters.AddWithValue("lastname", body.LastName);
        cmd.Parameters.AddWithValue("email", body.Email);
        cmd.Parameters.AddWithValue("phone", body.Phone);
        cmd.Parameters.AddWithValue("userId", userId);
        cmd.ExecuteNonQuery();

        return Ok();
    }

    private NpgsqlDataSource Connect() {
        var host = Environment.GetEnvironmentVariable("ASPNETCORE_DBHOST") ?? "arch.homework";// "otus-ms-db-service";
        var port = Environment.GetEnvironmentVariable("ASPNETCORE_DBPORT") ?? "30000";// "5432";
        var username = Environment.GetEnvironmentVariable("ASPNETCORE_DBUSERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("PGPASSWORD");
        var database = Environment.GetEnvironmentVariable("ASPNETCORE_DBNAME") ?? "otusms"; // "test";
        var connectionString = $"Host={host}:{port};Username={username};Password={password};Database={database};";
        var dataSource = NpgsqlDataSource.Create(connectionString);

        return dataSource;
    }

    private IReadOnlyCollection<User> Select(long userId) {
        using var dataSource = Connect();
        var result = new List<User>();
        using (var cmd = dataSource.CreateCommand($@"
select id, username, firstname, lastname, email, phone
from public.user
where id = {userId}"))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(new User {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Email = reader.GetString(4),
                    Phone = reader.GetString(5),
                });
            }
        }
        return result;
    }
}

/// <summary>
/// 
/// </summary>
[DataContract]
public partial class Error : IEquatable<Error>
{ 
    /// <summary>
    /// Gets or Sets Code
    /// </summary>
    [Required]

    [DataMember(Name="code")]
    public int? Code { get; set; }

    /// <summary>
    /// Gets or Sets Message
    /// </summary>
    [Required]

    [DataMember(Name="message")]
    public string Message { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Error {\n");
        sb.Append("  Code: ").Append(Code).Append("\n");
        sb.Append("  Message: ").Append(Message).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    /// <summary>
    /// Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Error)obj);
    }

    /// <summary>
    /// Returns true if Error instances are equal
    /// </summary>
    /// <param name="other">Instance of Error to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(Error other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return 
            (
                Code == other.Code ||
                Code != null &&
                Code.Equals(other.Code)
            ) && 
            (
                Message == other.Message ||
                Message != null &&
                Message.Equals(other.Message)
            );
    }

    /// <summary>
    /// Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hashCode = 41;
            // Suitable nullity checks etc, of course :)
                if (Code != null)
                hashCode = hashCode * 59 + Code.GetHashCode();
                if (Message != null)
                hashCode = hashCode * 59 + Message.GetHashCode();
            return hashCode;
        }
    }

    #region Operators
    #pragma warning disable 1591

    public static bool operator ==(Error left, Error right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Error left, Error right)
    {
        return !Equals(left, right);
    }

    #pragma warning restore 1591
    #endregion Operators
}

/// <summary>
/// 
/// </summary>
[DataContract]
public partial class User : IEquatable<User>
{ 
    /// <summary>
    /// Gets or Sets Id
    /// </summary>

    [DataMember(Name="id")]
    public long? Id { get; set; }

    /// <summary>
    /// Gets or Sets Username
    /// </summary>

    [MaxLength(256)]
    [DataMember(Name="username")]
    public string Username { get; set; }

    /// <summary>
    /// Gets or Sets FirstName
    /// </summary>

    [DataMember(Name="firstName")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or Sets LastName
    /// </summary>

    [DataMember(Name="lastName")]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or Sets Email
    /// </summary>

    [DataMember(Name="email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or Sets Phone
    /// </summary>

    [DataMember(Name="phone")]
    public string Phone { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class User {\n");
        sb.Append("  Id: ").Append(Id).Append("\n");
        sb.Append("  Username: ").Append(Username).Append("\n");
        sb.Append("  FirstName: ").Append(FirstName).Append("\n");
        sb.Append("  LastName: ").Append(LastName).Append("\n");
        sb.Append("  Email: ").Append(Email).Append("\n");
        sb.Append("  Phone: ").Append(Phone).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    /// <summary>
    /// Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((User)obj);
    }

    /// <summary>
    /// Returns true if User instances are equal
    /// </summary>
    /// <param name="other">Instance of User to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(User other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return 
            (
                Id == other.Id ||
                Id != null &&
                Id.Equals(other.Id)
            ) && 
            (
                Username == other.Username ||
                Username != null &&
                Username.Equals(other.Username)
            ) && 
            (
                FirstName == other.FirstName ||
                FirstName != null &&
                FirstName.Equals(other.FirstName)
            ) && 
            (
                LastName == other.LastName ||
                LastName != null &&
                LastName.Equals(other.LastName)
            ) && 
            (
                Email == other.Email ||
                Email != null &&
                Email.Equals(other.Email)
            ) && 
            (
                Phone == other.Phone ||
                Phone != null &&
                Phone.Equals(other.Phone)
            );
    }

    /// <summary>
    /// Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hashCode = 41;
            // Suitable nullity checks etc, of course :)
                if (Id != null)
                hashCode = hashCode * 59 + Id.GetHashCode();
                if (Username != null)
                hashCode = hashCode * 59 + Username.GetHashCode();
                if (FirstName != null)
                hashCode = hashCode * 59 + FirstName.GetHashCode();
                if (LastName != null)
                hashCode = hashCode * 59 + LastName.GetHashCode();
                if (Email != null)
                hashCode = hashCode * 59 + Email.GetHashCode();
                if (Phone != null)
                hashCode = hashCode * 59 + Phone.GetHashCode();
            return hashCode;
        }
    }

    #region Operators
    #pragma warning disable 1591

    public static bool operator ==(User left, User right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(User left, User right)
    {
        return !Equals(left, right);
    }

    #pragma warning restore 1591
    #endregion Operators
}

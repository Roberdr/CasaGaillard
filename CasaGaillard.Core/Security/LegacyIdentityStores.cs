using System.Data;
using System.Data.Common;
using CasaGaillard.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Security;

/// <summary>Identity stores for the ASP.NET Identity 2 schema used by MVC.</summary>
public sealed class LegacyIdentityUserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>,
    IUserEmailStore<IdentityUser>, IUserLockoutStore<IdentityUser>, IUserSecurityStampStore<IdentityUser>,
    IUserRoleStore<IdentityUser>
{
    private readonly AppDbContext _db;
    public LegacyIdentityUserStore(AppDbContext db) => _db = db;

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(user.Id)) user.Id = Guid.NewGuid().ToString();
        user.NormalizedUserName = Normalize(user.UserName);
        user.NormalizedEmail = Normalize(user.Email);
        await ExecuteAsync("INSERT INTO dbo.AspNetUsers (Id,UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount) VALUES (@id,@username,@email,@emailConfirmed,@password,@stamp,@phone,@phoneConfirmed,@twoFactor,@lockoutEnd,@lockoutEnabled,@failed)", cancellationToken,
            ("@id", user.Id), ("@username", user.UserName), ("@email", user.Email), ("@emailConfirmed", user.EmailConfirmed),
            ("@password", user.PasswordHash), ("@stamp", user.SecurityStamp), ("@phone", user.PhoneNumber),
            ("@phoneConfirmed", user.PhoneNumberConfirmed), ("@twoFactor", user.TwoFactorEnabled), ("@lockoutEnd", ToLegacyDate(user.LockoutEnd)),
            ("@lockoutEnabled", user.LockoutEnabled), ("@failed", user.AccessFailedCount));
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.NormalizedUserName = Normalize(user.UserName);
        user.NormalizedEmail = Normalize(user.Email);
        var changed = await ExecuteAsync("UPDATE dbo.AspNetUsers SET UserName=@username,Email=@email,EmailConfirmed=@emailConfirmed,PasswordHash=@password,SecurityStamp=@stamp,PhoneNumber=@phone,PhoneNumberConfirmed=@phoneConfirmed,TwoFactorEnabled=@twoFactor,LockoutEndDateUtc=@lockoutEnd,LockoutEnabled=@lockoutEnabled,AccessFailedCount=@failed WHERE Id=@id", cancellationToken,
            ("@id", user.Id), ("@username", user.UserName), ("@email", user.Email), ("@emailConfirmed", user.EmailConfirmed),
            ("@password", user.PasswordHash), ("@stamp", user.SecurityStamp), ("@phone", user.PhoneNumber),
            ("@phoneConfirmed", user.PhoneNumberConfirmed), ("@twoFactor", user.TwoFactorEnabled), ("@lockoutEnd", ToLegacyDate(user.LockoutEnd)),
            ("@lockoutEnabled", user.LockoutEnabled), ("@failed", user.AccessFailedCount));
        return changed == 0 ? IdentityResult.Failed(new IdentityError { Description = "La cuenta ya no existe." }) : IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var changed = await ExecuteAsync("DELETE FROM dbo.AspNetUsers WHERE Id=@id", cancellationToken, ("@id", user.Id));
        return changed == 0 ? IdentityResult.Failed(new IdentityError { Description = "La cuenta ya no existe." }) : IdentityResult.Success;
    }

    public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
        FindAsync("SELECT TOP (1) * FROM dbo.AspNetUsers WHERE Id=@value", "@value", userId, cancellationToken);

    public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
        FindAsync("SELECT TOP (1) * FROM dbo.AspNetUsers WHERE UPPER(UserName)=@value", "@value", normalizedUserName, cancellationToken);

    public Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        FindAsync("SELECT TOP (1) * FROM dbo.AspNetUsers WHERE UPPER(Email)=@value", "@value", normalizedEmail, cancellationToken);

    public string GetUserId(IdentityUser user) => user.Id;
    public string? GetUserName(IdentityUser user) => user.UserName;
    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken) { user.UserName = userName; user.NormalizedUserName = Normalize(userName); return Task.CompletedTask; }
    public string? GetNormalizedUserName(IdentityUser user) => user.NormalizedUserName;
    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);
    public Task SetNormalizedUserNameAsync(IdentityUser user, string? name, CancellationToken cancellationToken) { user.NormalizedUserName = name; return Task.CompletedTask; }
    public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken) { user.PasswordHash = passwordHash; return Task.CompletedTask; }
    public string? GetPasswordHash(IdentityUser user) => user.PasswordHash;
    public bool HasPassword(IdentityUser user) => user.PasswordHash is not null;
    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);
    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash is not null);
    public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken) { user.Email = email; user.NormalizedEmail = Normalize(email); return Task.CompletedTask; }
    public string? GetEmail(IdentityUser user) => user.Email;
    public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);
    public Task SetNormalizedEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken) { user.NormalizedEmail = email; return Task.CompletedTask; }
    public string? GetNormalizedEmail(IdentityUser user) => user.NormalizedEmail;
    public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);
    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);
    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken) { user.EmailConfirmed = confirmed; return Task.CompletedTask; }
    public Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken cancellationToken) { user.SecurityStamp = stamp; return Task.CompletedTask; }
    public Task<string?> GetSecurityStampAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.SecurityStamp);
    public Task<DateTimeOffset?> GetLockoutEndDateAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnd);
    public Task SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) { user.LockoutEnd = lockoutEnd; return Task.CompletedTask; }
    public Task<int> IncrementAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken) { user.AccessFailedCount++; return Task.FromResult(user.AccessFailedCount); }
    public Task ResetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken) { user.AccessFailedCount = 0; return Task.CompletedTask; }
    public Task<int> GetAccessFailedCountAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.AccessFailedCount);
    public Task<bool> GetLockoutEnabledAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnabled);
    public Task SetLockoutEnabledAsync(IdentityUser user, bool enabled, CancellationToken cancellationToken) { user.LockoutEnabled = enabled; return Task.CompletedTask; }

    public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var roles = new List<string>();
        await ReadAsync("SELECT r.Name FROM dbo.AspNetRoles r JOIN dbo.AspNetUserRoles ur ON ur.RoleId=r.Id WHERE ur.UserId=@id", cancellationToken,
            command => Add(command, "@id", user.Id), reader => roles.Add(reader.GetString(0)));
        return roles;
    }

    public async Task<bool> IsInRoleAsync(IdentityUser user, string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var count = await ScalarAsync("SELECT COUNT(1) FROM dbo.AspNetUserRoles ur JOIN dbo.AspNetRoles r ON r.Id=ur.RoleId WHERE ur.UserId=@id AND UPPER(r.Name)=@role", cancellationToken,
            ("@id", user.Id), ("@role", normalizedRoleName));
        return Convert.ToInt32(count) > 0;
    }

    public async Task AddToRoleAsync(IdentityUser user, string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await ExecuteAsync("INSERT INTO dbo.AspNetUserRoles (UserId,RoleId) SELECT @userId,r.Id FROM dbo.AspNetRoles r WHERE UPPER(r.Name)=@role AND NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles ur WHERE ur.UserId=@userId AND ur.RoleId=r.Id)", cancellationToken,
            ("@userId", user.Id), ("@role", normalizedRoleName));
    }

    public async Task RemoveFromRoleAsync(IdentityUser user, string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await ExecuteAsync("DELETE ur FROM dbo.AspNetUserRoles ur JOIN dbo.AspNetRoles r ON r.Id=ur.RoleId WHERE ur.UserId=@userId AND UPPER(r.Name)=@role", cancellationToken,
            ("@userId", user.Id), ("@role", normalizedRoleName));
    }

    public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var users = new List<IdentityUser>();
        await ReadAsync("SELECT u.* FROM dbo.AspNetUsers u JOIN dbo.AspNetUserRoles ur ON ur.UserId=u.Id JOIN dbo.AspNetRoles r ON r.Id=ur.RoleId WHERE UPPER(r.Name)=@role", cancellationToken,
            command => Add(command, "@role", normalizedRoleName), reader => users.Add(Map(reader)));
        return users;
    }

    public void Dispose() { }

    private async Task<IdentityUser?> FindAsync(string sql, string parameterName, string value, CancellationToken token)
    {
        IdentityUser? user = null;
        await ReadAsync(sql, token, command => Add(command, parameterName, value), reader => user = Map(reader));
        return user;
    }

    private async Task<int> ExecuteAsync(string sql, CancellationToken token, params (string Name, object? Value)[] values)
    {
        return await WithConnectionAsync(async connection =>
        {
            await using var command = connection.CreateCommand(); command.CommandText = sql;
            foreach (var value in values) Add(command, value.Name, value.Value);
            return await command.ExecuteNonQueryAsync(token);
        }, token);
    }

    private async Task<object?> ScalarAsync(string sql, CancellationToken token, params (string Name, object? Value)[] values)
    {
        return await WithConnectionAsync(async connection =>
        {
            await using var command = connection.CreateCommand(); command.CommandText = sql;
            foreach (var value in values) Add(command, value.Name, value.Value);
            return await command.ExecuteScalarAsync(token);
        }, token);
    }

    private async Task ReadAsync(string sql, CancellationToken token, Action<DbCommand> configure, Action<DbDataReader> consume)
    {
        await WithConnectionAsync(async connection =>
        {
            await using var command = connection.CreateCommand(); command.CommandText = sql; configure(command);
            await using var reader = await command.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token)) consume(reader);
            return 0;
        }, token);
    }

    private async Task<T> WithConnectionAsync<T>(Func<DbConnection, Task<T>> action, CancellationToken token)
    {
        var connection = _db.Database.GetDbConnection();
        var close = connection.State != ConnectionState.Open;
        if (close) await _db.Database.OpenConnectionAsync(token);
        try { return await action(connection); }
        finally { if (close) await _db.Database.CloseConnectionAsync(); }
    }

    private static void Add(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter(); parameter.ParameterName = name; parameter.Value = value ?? DBNull.Value;
        if (value is string) { parameter.DbType = DbType.String; parameter.Size = 4000; }
        command.Parameters.Add(parameter);
    }

    private static IdentityUser Map(DbDataReader reader)
    {
        var userName = Value<string>(reader, "UserName") ?? string.Empty;
        var email = Value<string>(reader, "Email");
        return new IdentityUser
        {
            Id = Value<string>(reader, "Id") ?? string.Empty, UserName = userName, NormalizedUserName = Normalize(userName),
            Email = email, NormalizedEmail = Normalize(email), EmailConfirmed = Value<bool>(reader, "EmailConfirmed"),
            PasswordHash = Value<string>(reader, "PasswordHash"), SecurityStamp = Value<string>(reader, "SecurityStamp"),
            PhoneNumber = Value<string>(reader, "PhoneNumber"), PhoneNumberConfirmed = Value<bool>(reader, "PhoneNumberConfirmed"),
            TwoFactorEnabled = Value<bool>(reader, "TwoFactorEnabled"), LockoutEnd = Value<DateTime?>(reader, "LockoutEndDateUtc") is DateTime lockout ? new DateTimeOffset(DateTime.SpecifyKind(lockout, DateTimeKind.Utc)) : null,
            LockoutEnabled = Value<bool>(reader, "LockoutEnabled"), AccessFailedCount = Value<int>(reader, "AccessFailedCount"), ConcurrencyStamp = Guid.NewGuid().ToString()
        };
    }

    private static T? Value<T>(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        if (reader.IsDBNull(ordinal)) return default;
        return (T)Convert.ChangeType(reader.GetValue(ordinal), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }

    private static DateTime? ToLegacyDate(DateTimeOffset? value) => value?.UtcDateTime;
    private static string? Normalize(string? value) => value?.Trim().ToUpperInvariant();
}

public sealed class LegacyIdentityRoleStore : IRoleStore<IdentityRole>
{
    private readonly AppDbContext _db;
    public LegacyIdentityRoleStore(AppDbContext db) => _db = db;
    public string GetRoleId(IdentityRole role) => role.Id;
    public string? GetRoleName(IdentityRole role) => role.Name;
    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.Id);
    public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.Name);
    public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken) { role.Name = roleName; role.NormalizedName = roleName?.Trim().ToUpperInvariant(); return Task.CompletedTask; }
    public string? GetNormalizedRoleName(IdentityRole role) => role.NormalizedName;
    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.NormalizedName);
    public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken) { role.NormalizedName = normalizedName; return Task.CompletedTask; }
    public Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken) => FindAsync("SELECT TOP (1) Id,Name FROM dbo.AspNetRoles WHERE Id=@value", roleId, cancellationToken);
    public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) => FindAsync("SELECT TOP (1) Id,Name FROM dbo.AspNetRoles WHERE UPPER(Name)=@value", normalizedRoleName, cancellationToken);
    public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); if (string.IsNullOrWhiteSpace(role.Id)) role.Id = Guid.NewGuid().ToString();
        await ExecuteAsync("INSERT INTO dbo.AspNetRoles (Id,Name) VALUES (@id,@name)", cancellationToken, ("@id",role.Id),("@name",role.Name)); return IdentityResult.Success;
    }
    public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); var count=await ExecuteAsync("UPDATE dbo.AspNetRoles SET Name=@name WHERE Id=@id", cancellationToken,("@id",role.Id),("@name",role.Name));
        return count==0 ? IdentityResult.Failed(new IdentityError { Description="El rol ya no existe." }) : IdentityResult.Success;
    }
    public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); var count=await ExecuteAsync("DELETE FROM dbo.AspNetRoles WHERE Id=@id", cancellationToken,("@id",role.Id));
        return count==0 ? IdentityResult.Failed(new IdentityError { Description="El rol ya no existe." }) : IdentityResult.Success;
    }
    public void Dispose() { }
    private async Task<IdentityRole?> FindAsync(string sql, string value, CancellationToken token)
    {
        IdentityRole? role=null; await WithConnectionAsync(async connection => { await using var command=connection.CreateCommand(); command.CommandText=sql; Add(command,"@value",value); await using var reader=await command.ExecuteReaderAsync(token); if(await reader.ReadAsync(token)) role=new IdentityRole(reader.GetString(1)){Id=reader.GetString(0),NormalizedName=reader.GetString(1).ToUpperInvariant()}; return 0; },token); return role;
    }
    private async Task<int> ExecuteAsync(string sql,CancellationToken token,params (string Name,object? Value)[] values) => await WithConnectionAsync(async connection=>{await using var command=connection.CreateCommand(); command.CommandText=sql; foreach(var value in values) Add(command,value.Name,value.Value); return await command.ExecuteNonQueryAsync(token);},token);
    private async Task<T> WithConnectionAsync<T>(Func<DbConnection,Task<T>> action,CancellationToken token){var connection=_db.Database.GetDbConnection();var close=connection.State!=ConnectionState.Open;if(close)await _db.Database.OpenConnectionAsync(token);try{return await action(connection);}finally{if(close)await _db.Database.CloseConnectionAsync();}}
    private static void Add(DbCommand command,string name,object? value){var parameter=command.CreateParameter();parameter.ParameterName=name;parameter.Value=value??DBNull.Value;if(value is string){parameter.DbType=DbType.String;parameter.Size=4000;}command.Parameters.Add(parameter);}
}

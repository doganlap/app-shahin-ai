using System;
using System.DirectoryServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Grc.Integration.Application.Connectors;

/// <summary>
/// Active Directory connector
/// </summary>
public class ActiveDirectoryConnector
{
    private readonly ILogger<ActiveDirectoryConnector> _logger;
    private readonly string _ldapPath;
    private readonly string _username;
    private readonly string _password;

    public ActiveDirectoryConnector(
        string ldapPath,
        string username,
        string password,
        ILogger<ActiveDirectoryConnector> logger)
    {
        _ldapPath = ldapPath;
        _username = username;
        _password = password;
        _logger = logger;
    }

    /// <summary>
    /// Sync users from Active Directory
    /// </summary>
    public async Task<List<AdUser>> SyncUsersAsync()
    {
        var users = new List<AdUser>();
        
        try
        {
            using var entry = new DirectoryEntry(_ldapPath, _username, _password);
            using var searcher = new DirectorySearcher(entry)
            {
                Filter = "(&(objectClass=user)(objectCategory=person))",
                PropertiesToLoad = { "samAccountName", "displayName", "mail", "department", "title" }
            };

            var results = searcher.FindAll();
            
            foreach (SearchResult result in results)
            {
                var user = new AdUser
                {
                    Username = result.Properties["samAccountName"][0]?.ToString(),
                    DisplayName = result.Properties["displayName"][0]?.ToString(),
                    Email = result.Properties["mail"][0]?.ToString(),
                    Department = result.Properties["department"][0]?.ToString(),
                    Title = result.Properties["title"][0]?.ToString()
                };
                
                users.Add(user);
            }
            
            _logger.LogInformation("Synced {Count} users from Active Directory", users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing users from Active Directory");
            throw;
        }
        
        return await Task.FromResult(users);
    }
}

/// <summary>
/// Active Directory user
/// </summary>
public class AdUser
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public string Title { get; set; }
}


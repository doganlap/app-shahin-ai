using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Grc.Data;

/* This is used if database provider does't define
 * IGrcDbSchemaMigrator implementation.
 */
public class NullGrcDbSchemaMigrator : IGrcDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

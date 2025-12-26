using System.Threading.Tasks;

namespace Grc.Data;

public interface IGrcDbSchemaMigrator
{
    Task MigrateAsync();
}

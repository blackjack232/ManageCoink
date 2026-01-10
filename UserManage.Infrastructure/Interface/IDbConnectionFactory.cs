using System.Data;

namespace UserManage.Infrastructure.Interface
{

    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
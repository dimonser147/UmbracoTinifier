using System.Linq;
using Tinifier.Core.Infrastructure;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.Section
{
    public class TSectionRepo : ITSectionRepo
    {
        private readonly DatabaseSchemaHelper _dbHelper;
        private readonly DatabaseContext _dbContext;
        private readonly string _adminAlias = "admin";

        public TSectionRepo()
        {
            var logger = LoggerResolver.Current.Logger;
            _dbContext = ApplicationContext.Current.DatabaseContext;
            _dbHelper = new DatabaseSchemaHelper(_dbContext.Database, logger, _dbContext.SqlSyntax);
        }

        public void AssignTinifierToAdministrators()
        {
            // Umbraco 7.7+
            if (_dbHelper.TableExist("umbracoUserGroup2App"))
            {
                int? groupId = _dbContext.Database
                    .ExecuteScalar<int?>("select id from umbracoUserGroup where userGroupAlias = @0", _adminAlias);
                if (groupId.HasValue && groupId != 0)
                {
                    int rows = _dbContext.Database
                        .ExecuteScalar<int>("select count(*) from umbracoUserGroup2App where userGroupId = @0 and app = @1",
                            groupId.Value, PackageConstants.SectionAlias);
                    if (rows == 0)
                    {
                        _dbContext.Database.Execute("insert umbracoUserGroup2App values (@0, @1)",
                            groupId.Value, PackageConstants.SectionAlias);
                    }

                }
            }
            // Umbraco 7.2 - 7.6
            else if (_dbHelper.TableExist("UmbracoUser2app"))
            {
                string sql = @"select u.id from umbracoUser as u
                                inner join umbracoUserType as t on u.userType = t.id
                                    where t.userTypeAlias = @0";
                var usersIds = _dbContext.Database.Query<int>(sql, _adminAlias).ToList();
                foreach (int userId in usersIds)
                {
                    int rows = _dbContext.Database
                        .ExecuteScalar<int>("select count(*) from UmbracoUser2app where [user] = @0 and app = @1",
                            userId, PackageConstants.SectionAlias);
                    if (rows == 0)
                    {
                        _dbContext.Database.Execute("insert UmbracoUser2app values (@0, @1)",
                            userId, PackageConstants.SectionAlias);
                    }
                }
            }
        }
    }
}

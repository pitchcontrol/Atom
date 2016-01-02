using System.Collections.Generic;
using System.Data;
using System.Linq;
using Atom.Models;
using ByndyuSoft.Infrastructure.Dapper;

namespace Atom.Services
{
    public class Dal
    {
        public IEnumerable<MenuTree> GetMenuTree()
        {
            string sql = "select ut_MenuGroupView.pkid Id, null Url,ut_MenuGroupView.nam Name,ut_MenuGroupView.idparent parentGroupId, 1 isGroup, null PageId from ut_MenuGroupView union select ut_MenuPageView.idmenupage Id, ut_MenuPage.page  Url,ut_MenuPageView.nam Name, ut_MenuPageView.idgroup parentGroupId, 0 isGroup, ut_MenuPageView.pkid PageId  from ut_MenuPageView left join ut_MenuPage ON ut_MenuPage.pkid = ut_MenuPageView.idmenupage";
            IEnumerable<MenuTree> trees;
            using (IDbConnection dbConnection = ConnectionService.GetConnection())
            {
                dbConnection.Open();
                trees = dbConnection.Query<MenuTree>(new QueryObject(sql));
            }

            var groups = trees.Where(i => i.IsGroup).ToDictionary(i => i.Id);
            foreach (MenuTree node in trees)
            {
                if (groups.ContainsKey(node.ParentGroupId) && groups[node.ParentGroupId].IsGroup)
                    groups[node.ParentGroupId].Children.Add(node);
            }
            return trees;
        }

        public IEnumerable<MenuTree> GetMenuTree(int page)
        {
            string sql =
                "select ut_MenuPageView.idmenupage Id, ut_MenuPage.page  Url,ut_MenuPageView.nam Name,ut_MenuPageView.idgroup parentGroupId, 0 isGroup, ut_MenuPageView.pkid PageId  from ut_MenuPageView left join ut_MenuPage ON ut_MenuPage.pkid = ut_MenuPageView.idmenupage where idmenupage = @page";
            IEnumerable<MenuTree> trees;
            using (IDbConnection dbConnection = ConnectionService.GetConnection())
            {
                dbConnection.Open();
                trees = dbConnection.Query<MenuTree>(new QueryObject(sql, new { @page = page }));
            }
            return trees;
        }
        public IEnumerable<MenuTree> GetMenuTree(string name)
        {
            string sql =
                "select ut_MenuPageView.idmenupage Id, ut_MenuPage.page  Url,ut_MenuPageView.nam Name,ut_MenuPageView.idgroup parentGroupId, 0 isGroup, ut_MenuPageView.pkid PageId  from ut_MenuPageView left join ut_MenuPage ON ut_MenuPage.pkid = ut_MenuPageView.idmenupage where nam like '%" + name + "%'";
            IEnumerable<MenuTree> trees;
            using (IDbConnection dbConnection = ConnectionService.GetConnection())
            {
                dbConnection.Open();
                trees = dbConnection.Query<MenuTree>(new QueryObject(sql));
            }
            return trees;
        }

        public IEnumerable<Role> GetRoleForGroup(int group)
        {
            string sql =
                "select [ut_RoleGroup].idrole Id,ut_Roles.nam Name,[ut_RoleGroup].visability Visibility  from [ut_RoleGroup] left join ut_Roles ON ut_Roles.pkid = [ut_RoleGroup].idrole where [ut_RoleGroup].idgroup = @group";
            IEnumerable<Role> roles;
            using (IDbConnection dbConnection = ConnectionService.GetConnection())
            {
                dbConnection.Open();
                roles = dbConnection.Query<Role>(new QueryObject(sql, new { @group = group }));
            }
            return roles;
        }
        public IEnumerable<Role> GetRoleForPage(int page)
        {
            string sql =
                "select [ut_RolePage].idrole Id,ut_Roles.nam Name,[ut_RolePage].visability Visibility  from [ut_RolePage] left join ut_Roles ON ut_Roles.pkid = [ut_RolePage].idrole where ut_RolePage.idpage = @page";
            IEnumerable<Role> roles;
            using (IDbConnection dbConnection = ConnectionService.GetConnection())
            {
                dbConnection.Open();
                roles = dbConnection.Query<Role>(new QueryObject(sql, new { @page = page }));
            }
            return roles;
        }

        public IEnumerable<Role> GetGlobalRoles()
        {
            string sql =
                "select ut_Roles.pkid Id,ut_Roles.nam Name from ut_Roles";
            IEnumerable<Role> roles;
            using (IDbConnection dbConnection = ConnectionService.GetConnection())
            {
                dbConnection.Open();
                roles = dbConnection.Query<Role>(new QueryObject(sql));
            }
            return roles;
        } 
    }
}

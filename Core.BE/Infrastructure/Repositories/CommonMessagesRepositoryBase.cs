using Emeint.Core.BE.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Emeint.Core.BE.Infrastructure.Repositories
{
    public abstract class CommonMessagesRepositoryBase
    {
        private DbContext _dbContext;

        public CommonMessagesRepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected void ExecuteSql(string targetEntity, EntityActionType changeType, KeyValuePair<string, object> keyAttr, List<KeyValuePair<string, object>> properties)
        {
            string query;
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (changeType == EntityActionType.Created)
            {
                string attributes = string.Join(",", properties.Select(s => string.Format("{0}", s.Key)));
                string paramtersNames = string.Join(",", properties.Select(s => string.Format("@{0}", s.Key)));

                properties.ForEach(param =>
                {
                    parameters.Add(new SqlParameter($"@{param.Key}", param.Value == null ? DBNull.Value : param.Value));
                });

                query = $"insert into {targetEntity} ({attributes}) values ({paramtersNames})";
            }
            else if (changeType == EntityActionType.Updated)
            {
                string attributesWithParams = string.Join(",", properties.Select(s => string.Format(" {0}=@{0}", s.Key)));
                properties.ForEach(param =>
                {
                    parameters.Add(new SqlParameter($"@{param.Key}", param.Value == null ? DBNull.Value : param.Value));
                });
                query = $"update {targetEntity} set {attributesWithParams} where {keyAttr.Key}=@{keyAttr.Key}";
            }
            else
            {
                parameters.Add(new SqlParameter($"@{keyAttr.Key}", keyAttr.Value));
                query = $"delete from {targetEntity} where {keyAttr.Key}=@{keyAttr.Key}";
            }

            //https://stackoverflow.com/questions/59662673/are-ef-core-3-1-executesqlraw-executesqlrawasync-drop-in-replacements-for-exec
            _dbContext.Database.ExecuteSqlRaw(query, parameters);
        }
    }
}

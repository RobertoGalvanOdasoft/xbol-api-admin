using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Odasoft.XBOL.Data.Repositories
{
    /// <summary>
    /// Generic repository implementation for any entity type without constraints.
    /// Supports composite keys and provides full CRUD operations.
    /// </summary>
    public class GenericRepository<TEntity>
        where TEntity : class
    {
        protected DbContext DbContext { get; set; }
        protected readonly DbSet<TEntity> DbSet;
        protected bool Disposed;

        public GenericRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
            Disposed = false;
        }

        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? pageSize = null,
            int? currentPage = null,
            params string[] includedProperties)
        {
            return GetQuery(filter, orderBy, pageSize, currentPage, includedProperties)
                .AsNoTracking();
        }

        public void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task Update(TEntity entity)
        {
            var entry = DbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keys = GetPrimaryKeys(entity);
                var currentEntry = await GetByCompositeKeyAsync(keys);

                if (currentEntry != null)
                {
                    var attachedEntry = DbContext.Entry(currentEntry);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    DbSet.Attach(entity);
                    DbContext.Entry(entity).State = EntityState.Modified;
                }
            }
            else if (entry.State == EntityState.Unchanged)
            {
                DbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateSync(TEntity entity, bool commitChanges = true)
        {
            var entry = DbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keys = GetPrimaryKeys(entity);
                var currentEntry = GetByCompositeKeySync(keys);

                if (currentEntry != null)
                {
                    var attachedEntry = DbContext.Entry(currentEntry);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    DbSet.Attach(entity);
                    DbContext.Entry(entity).State = EntityState.Modified;
                }
            }
            else if (entry.State == EntityState.Unchanged)
            {
                DbContext.Entry(entity).State = EntityState.Modified;
            }

            if (commitChanges && entry.State == EntityState.Modified)
            {
                DbContext.SaveChanges();
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var entry = DbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keys = GetPrimaryKeys(entity);
                var currentEntry = await GetByCompositeKeyAsync(keys);

                if (currentEntry != null)
                {
                    var attachedEntry = DbContext.Entry(currentEntry);
                    attachedEntry.CurrentValues.SetValues(entity);
                    entry = attachedEntry;
                }
                else
                {
                    DbSet.Attach(entity);
                    DbContext.Entry(entity).State = EntityState.Modified;
                }
            }

            if (entry.State == EntityState.Unchanged)
            {
                DbContext.Entry(entity).State = EntityState.Modified;
                return;
            }

            if (entry.State == EntityState.Modified)
            {
                await DbContext.SaveChangesAsync();
            }
        }

        public void HardDelete(TEntity entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            DbSet.Remove(entity);
        }

        public async Task HardDeleteAsync(TEntity entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves an entity by its composite key.
        /// </summary>
        public virtual async Task<TEntity?> GetByCompositeKeyAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
            {
                throw new ArgumentException("At least one key value must be provided.", nameof(keyValues));
            }

            return await DbSet.FindAsync(keyValues);
        }

        /// <summary>
        /// Retrieves an entity by its composite key (synchronously).
        /// </summary>
        public virtual TEntity? GetByCompositeKeySync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
            {
                throw new ArgumentException("At least one key value must be provided.", nameof(keyValues));
            }

            return DbSet.Find(keyValues);
        }

        /// <summary>
        /// Retrieves an entity by a single long ID.
        /// </summary>
        public async Task<TEntity?> GetByIdAsync(long id)
        {
            return await DbSet.FindAsync(id);
        }

        /// <summary>
        /// Retrieves an entity by a single string ID.
        /// </summary>
        public async Task<TEntity?> GetByIdAsync(string id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual IQueryable<TEntity> GetQuery(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null,
            int? take = null,
            params string[] includedProperties)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter).AsNoTracking();
            }

            foreach (var includedProperty in includedProperties)
            {
                query = query.Include(includedProperty).AsNoTracking();
            }

            if (orderBy != null)
            {
                query = orderBy(query).AsNoTracking();
            }

            if (skip.HasValue && take.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.AsNoTracking();
        }

        public async Task<IEnumerable<N>> ExecuteStoredProcedureValues<N>(
            string query,
            Dictionary<string, object> parameters,
            string? connectionString = null)
        {
            using IDbConnection connection = GetConnection(connectionString);
            connection.Open();

            var items = await connection.QueryAsync<N>(
                $"{query}",
                GetDynamicParameters(parameters),
                commandType: CommandType.StoredProcedure,
                commandTimeout: 0
            );

            connection.Close();
            return items;
        }

        public IEnumerable<N> ExecuteStoredProcedureValues<N>(
            string query,
            CommandType commandType,
            string? connectionString = null)
        {
            return ExecuteStoredProcedureValuesSync<N>(
                query,
                commandType,
                new Dictionary<string, object>(),
                connectionString
            );
        }

        public IEnumerable<N> ExecuteStoredProcedureValuesSync<N>(
            string query,
            CommandType commandType,
            Dictionary<string, object> parameters,
            string? connectionString = null)
        {
            using IDbConnection connection = GetConnection(connectionString);
            connection.Open();

            var items = connection.Query<N>(
                $"{query}",
                GetDynamicParameters(parameters),
                commandType: commandType,
                commandTimeout: 0
            );

            connection.Close();
            return items;
        }

        public void ExecuteQuerySync(
            string query,
            string? connectionString = null,
            int? commandTimeout = null)
        {
            using IDbConnection connection = GetConnection(connectionString);
            connection.Open();
            var items = connection.Execute($"{query}", commandTimeout: commandTimeout);
            connection.Close();
        }

        public async Task<IEnumerable<N>> ExecuteStoredProcedureValues<N>(
            string query,
            object parameters,
            string? connectionString = null)
        {
            return await ExecuteStoredProcedureValues<N>(
                query,
                GetDictionaryParameters(parameters),
                connectionString
            );
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await DbContext.Database.BeginTransactionAsync();
        }

        protected Dictionary<string, object> GetDictionaryParameters(object parameters)
        {
            var sqlParameters = new Dictionary<string, object>();

            foreach (PropertyInfo prop in parameters.GetType().GetProperties())
            {
                var value = prop.GetValue(parameters, null);

                if (value is not null)
                {
                    sqlParameters.Add(prop.Name, value);
                }
            }

            return sqlParameters;
        }

        protected DynamicParameters GetDynamicParameters(Dictionary<string, object> parameters)
        {
            var sqlParameters = new DynamicParameters();

            foreach (var pair in parameters)
            {
                if (pair.Value is DataTable dataTable)
                {
                    sqlParameters.Add(pair.Key, dataTable.AsTableValuedParameter());
                }
                else
                {
                    sqlParameters.Add(pair.Key, pair.Value);
                }
            }

            return sqlParameters;
        }

        public IDbConnection GetConnection(string? connectionString = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = DbContext.Database.GetDbConnection();
                connectionString = connection.ConnectionString;
            }

            return new NpgsqlConnection(connectionString);
        }

        protected void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                DbContext.Dispose();
            }

            Disposed = true;
        }

        /// <summary>
        /// Extracts primary key values from an entity instance.
        /// Works with both single and composite keys.
        /// </summary>
        private object[] GetPrimaryKeys(TEntity entity)
        {
            var keyNames = GetKeyNames();
            Type type = typeof(TEntity);
            var keys = new object[keyNames.Length];

            for (int i = 0; i < keyNames.Length; i++)
            {
                var propertyInfo = type.GetProperty(keyNames[i]);

                if (propertyInfo == null)
                {
                    throw new InvalidOperationException($"Property '{keyNames[i]}' was not found on entity '{type.Name}'.");
                }

                var keyValue = propertyInfo.GetValue(entity, null);

                if (keyValue == null)
                {
                    throw new InvalidOperationException($"The primary key property '{keyNames[i]}' on entity '{type.Name}' cannot be null.");
                }

                keys[i] = keyValue;
            }

            return keys;
        }

        /// <summary>
        /// Retrieves primary key names from the DbContext model.
        /// Returns an array supporting both single and composite keys.
        /// </summary>
        private string[] GetKeyNames()
        {
            var entityType = DbContext.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType?.FindPrimaryKey();

            if (primaryKey == null)
            {
                throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' is not registered in the DbContext or does not have a primary key defined.");
            }

            return primaryKey.Properties.Select(x => x.Name).ToArray();
        }
    }
}

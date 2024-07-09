using BookLibrary.Data;
using BookLibrary.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BookLibrary.Repositories.Abstractions
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BaseRepository<TEntity>> _logger;

        public BaseRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<TEntity>> logger)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private async Task<TEntity> InsertOrUpdateEntityAsync(TEntity entity, bool isUpdate)
        {
            _logger.LogInformation($"BaseRepository - {(isUpdate ? "Update" : "Insert")}EntityAsync - Entering - User Id - {GetUserId()}");

            if (entity.GetType().BaseType == typeof(AuditableEntity) || entity.GetType().BaseType == typeof(AuditableEntityWithDelete))
            {
                var now = DateTime.Now;
                var userId = GetUserId();
                if (isUpdate)
                {
                    entity.GetType().GetProperty("LastModificationTime")?.SetValue(entity, now, null);
                    entity.GetType().GetProperty("LastModifierUserId")?.SetValue(entity, userId, null);
                }
                else
                {
                    entity.GetType().GetProperty("CreationTime")?.SetValue(entity, now, null);
                    entity.GetType().GetProperty("CreatorUserId")?.SetValue(entity, userId, null);
                }
            }

            if (isUpdate)
            {
                _unitOfWork._context.Entry(entity).State = EntityState.Modified;
                _unitOfWork._context.Set<TEntity>().Update(entity);
            }
            else
            {
                await _unitOfWork._context.Set<TEntity>().AddAsync(entity);
            }

            await _unitOfWork.CommitAsync();
            _logger.LogInformation($"BaseRepository - {(isUpdate ? "Update" : "Insert")}EntityAsync - Completed - User Id - {GetUserId()}");
            return entity;
        }

        public async Task<TEntity> InsertOrUpdateAsync(int? id, TEntity entity)
        {
            _logger.LogInformation("BaseRepository - InsertOrUpdateAsync (int) - Entering - User Id - " + GetUserId());

            var existingEntity = id != null ? await _unitOfWork._context.Set<TEntity>().FindAsync(id) : null;
            if (existingEntity != null)
            {
                _unitOfWork._context.Entry(existingEntity).State = EntityState.Detached;
            }

            return await InsertOrUpdateEntityAsync(entity, existingEntity != null);
        }        


        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - InsertAsync - Entering - User Id - " + GetUserId());
            return await InsertOrUpdateEntityAsync(entity, false);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _logger.LogInformation("BaseRepository - UpdateAsync - Entering - User Id - " + GetUserId());
            await InsertOrUpdateEntityAsync(entity, true);
        }

        public async Task SoftDeleteAsync(TEntity entity)
        {
            _logger.LogInformation($"BaseRepository - SoftDeleteAsync - Entering - User Id - {GetUserId()}");

            if (entity is AuditableEntity auditableEntity)
            {
                auditableEntity.LastModificationTime = DateTime.Now;
                auditableEntity.LastModifierUserId = GetUserId();
                _unitOfWork._context.Entry(auditableEntity).Property(e => e.LastModificationTime).IsModified = true;
                _unitOfWork._context.Entry(auditableEntity).Property(e => e.LastModifierUserId).IsModified = true;
            }

            var isActiveProperty = entity.GetType().GetProperty("IsActive");
            if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
            {
                isActiveProperty.SetValue(entity, false, null);
                _unitOfWork._context.Entry(entity).Property("IsActive").IsModified = true;
                _logger.LogInformation("BaseRepository - SoftDeleteAsync - IsActive property set to false.");
            }
            else
            {
                _logger.LogWarning("BaseRepository - SoftDeleteAsync - IsActive property not found or not a boolean.");
                return;
            }

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("BaseRepository - SoftDeleteAsync - Entity updated and committed.");
        }


        public IQueryable<TEntity> GetAll()
        {
            _logger.LogInformation("BaseRepository - GetAll - Entering");             
            var query = _unitOfWork._context.Set<TEntity>().AsQueryable(); 
            
            return query;
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            _logger.LogInformation("BaseRepository - GetAllAsync - Entering");
            return await _unitOfWork._context.Set<TEntity>().ToListAsync();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            _logger.LogInformation("BaseRepository - Get - Entering");

            var query = GetAll().Where(expression);

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (orderBy != null)
            {
                query = orderBy(query).AsQueryable();
            }

            return query;
        }

        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>> searchExpression, params Expression<Func<TEntity, object>>[] includes)
        {
            _logger.LogInformation("BaseRepository - GetAll (Paged) - Entering");
            return GetAll(pageNumber, pageSize, orderBy, orderDirection, searchExpression, null, includes);
        }

        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection)
        {
            _logger.LogInformation("BaseRepository - GetAll (Paged, No Filters) - Entering");
            return GetAll(pageNumber, pageSize, orderBy, orderDirection, null, null, null);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            _logger.LogInformation($"BaseRepository - GetByIdAsync (int) - Entering - Id: {id}");
            var entity = await _unitOfWork._context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No {typeof(TEntity).Name} object found with id: {id}");
            }

            return entity;
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression) => GetAll().Where(expression);

        public virtual IQueryable<TEntity> Include<TProperty>(IQueryable<TEntity> query, Expression<Func<TEntity, TProperty>> path)
        {
            _logger.LogInformation("BaseRepository - Include - Entering");
            return query.Include(path);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"BaseRepository - DeleteAsync (int) - Entering - Id: {id}");
            var entity = await _unitOfWork._context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No {typeof(TEntity).Name} object found with id: {id}");
            }

            _unitOfWork._context.Set<TEntity>().Remove(entity);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation($"BaseRepository - DeleteAsync (int) - Completed - Id: {id}");
        }

        public PagedList<TEntity> GetAll(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, Expression<Func<TEntity, bool>>? expression, Expression<Func<TEntity, bool>>? searchExpression, params Expression<Func<TEntity, object>>[]? includes)
        {
            _logger.LogInformation("BaseRepository - GetAll (Paged with Filters) - Entering");

            var query = expression != null ? GetAll().Where(expression) : GetAll();

            if (searchExpression != null)
            {
                query = query.Where(searchExpression);
            }

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return PagedList<TEntity>.ToPagedList(query, pageNumber ?? 1, pageSize ?? 10, orderBy, orderDirection);
        }

        public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression)
        {
            _logger.LogInformation("BaseRepository - ExistAsync - Entering");
            return await _unitOfWork._context.Set<TEntity>().AsNoTracking().AnyAsync(expression);
        }

        public async Task<TEntity?> GetOneNoTracking(Expression<Func<TEntity, bool>> expression)
        {
            _logger.LogInformation("BaseRepository - GetOneNoTracking - Entering");
            return await _unitOfWork._context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<TEntity?> GetOneTracking(Expression<Func<TEntity, bool>> expression)
        {
            _logger.LogInformation("BaseRepository - GetOneTracking - Entering");
            return await _unitOfWork._context.Set<TEntity>().FirstOrDefaultAsync(expression);
        }

        public async Task<IReadOnlyList<TEntity>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation($"BaseRepository - GetPagedReponseAsync - Entering - PageNumber: {pageNumber}, PageSize: {pageSize}");
            return await _unitOfWork._context
                .Set<TEntity>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        private long GetUserId()
        {
            long.TryParse(_httpContextAccessor.HttpContext?.User.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value, out long userId);
            return userId;
        }

    }
}

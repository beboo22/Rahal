using Application.Abstraction.Specification;
using ApplicationBusiness.Fetures.TripService.Query;
using Domain.Entity;
using Domain.Entity.PostEntity;
using Domain.Entity.TripEntity;
using System.Linq.Expressions;

namespace Application.Abstraction.spacification
{

    public class HiringPostSearchSpecification : Specification<HiringPost>
    {
        public HiringPostSearchSpecification(DateTime? date, string? title, bool OrderDesBytimeCreated, int? pageIndex, int? pageSize = 5)
        {
            Expression<Func<HiringPost, bool>> _criteria = post => true;

            if (!string.IsNullOrWhiteSpace(title))
                _criteria = _criteria.AndAlso(post => post.Title.Contains(title));

            if (date.HasValue)
                _criteria = _criteria.AndAlso(post => post.CreatedAt.Date == date.Value.Date);



            crateria = _criteria;
            includes.Add(post => post.Comments);
            // Pagination
            if (pageIndex.HasValue && pageIndex > 0)
            {
                int skip = (pageIndex.Value - 1) * (pageSize.HasValue ? pageSize.Value : 1);
                ApplyPagination(skip, (pageSize.HasValue ? pageSize.Value : 1));
            }
            if (OrderDesBytimeCreated)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);
        }
    }
    public class PaymentSpecification : Specification<PaymentRequest>
    {
        public PaymentSpecification(string? providerRef)
        {
            Expression<Func<PaymentRequest, bool>> _criteria = post => true;

            if (!string.IsNullOrWhiteSpace(providerRef))
                _criteria = _criteria.AndAlso(post => post.ProviderRef == providerRef);

            crateria = _criteria;
        }
    }







    public class ExperiencePostSearchSpecification : Specification<ExperiencePost>
    {
        public ExperiencePostSearchSpecification(
            DateTime? date,
            int? id,
            string? title,
            string? country,
            string? city,
            bool OrderDesBytimeCreated,
            int? pageIndex,
            int pageSize = 5)
        {
            Expression<Func<ExperiencePost, bool>> _criteria = post => true;

            if (id.HasValue)
            {
                crateria = x => x.Id == id.Value;

                // --------------------
                // Includes (IMPORTANT)
                // --------------------

                includes.Add(post => post.Comments);
                includes.Add(x => x.CreatedBy);
                includes.Add(x => x.Likes);

                return;
            }


            if (!string.IsNullOrWhiteSpace(title))
                _criteria = _criteria.AndAlso(post => post.Title.Contains(title));

            if (date.HasValue)
                _criteria = _criteria.AndAlso(post => post.CreatedAt.Date == date.Value.Date);

            if (!string.IsNullOrWhiteSpace(country))
                _criteria = _criteria.AndAlso(post => post.Country.Contains(country));

            if (!string.IsNullOrWhiteSpace(city))
                _criteria = _criteria.AndAlso(post => post.City.Contains(city));


            crateria = _criteria;

            includes.Add(post => post.Comments);
            includes.Add(x => x.CreatedBy);
            // Pagination
            if (pageIndex.HasValue && pageIndex > 0)
            {
                int skip = (pageIndex.Value - 1) * pageSize;
                ApplyPagination(skip, pageSize);
            }

            if (OrderDesBytimeCreated)
                AddOrderByDecs(x => x.CreatedAt);
            else
                AddOrderBy(x => x.CreatedAt);
        }

    }



    public class Specification<T> : ISpecification<T> where T : BaseEntity
    {
        //.Where(p=>p.ID == id)
        public Expression<Func<T, bool>>? crateria { get; set; }
        public List<Expression<Func<T, object>>> includes { get; set; } = new List<Expression<Func<T, object>>>();
        public List<Func<IQueryable<T>, IQueryable<T>>> IncludeChains { get; set; }
        = new();
        public Expression<Func<T, object>> Orderby { get; set; }
        public Expression<Func<T, object>> OrderbyDecs { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagination { get; set; }
        protected void AndAlso(Expression<Func<T, bool>> expression)
        {
            if (crateria == null)
            {
                crateria = expression;
                return;
            }

            var parameter = crateria.Parameters[0];

            var visitor = new ReplaceExpressionVisitor(
                expression.Parameters[0],
                parameter);

            var body = Expression.AndAlso(
                crateria.Body,
                visitor.Visit(expression.Body)!);

            crateria = Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        public Specification()
        {
            //crateria = null;
        }

        public Specification(Expression<Func<T, bool>> _crateria)
        {
            crateria = _crateria;
        }

        public void AddOrderBy(Expression<Func<T, object>> _)
        {
            Orderby = _;
        }
        protected void AddIncludeChain(
        Func<IQueryable<T>, IQueryable<T>> includeExpression)
        {
            IncludeChains.Add(includeExpression);
        }
        public void AddOrderByDecs(Expression<Func<T, object>> _)
        {
            OrderbyDecs = _;
        }

        public void ApplyPagination(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagination = true;
        }

    }

    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            if (node == _oldValue)
                return _newValue;

            return base.Visit(node);
        }
    }

}

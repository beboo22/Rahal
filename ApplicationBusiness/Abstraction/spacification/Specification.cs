using Application.Abstraction.Specification;
using ApplicationBusiness.Fetures.TripService.Query;
using Domain.Entity;
using Domain.Entity.PostEntity;
using Domain.Entity.TripEntity;
using System.Linq.Expressions;

namespace Application.Abstraction.spacification
{
    
    public class HiringPostSearchSpecification:Specification<HiringPost>
    {
        public HiringPostSearchSpecification(DateTime? date, string? title, int? pageIndex, int pageSize = 5)
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
                int skip = (pageIndex.Value - 1) * pageSize;
                ApplyPagination(skip, pageSize);
            }
            AddOrderByDecs(post => post.CreatedAt);
        }
    }
    public class PaymentSpecification:Specification<PaymentRequest>
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
            string? title,
            string? country,
            string? city,
            string? tipsAndRecommendations,
            decimal? budget, 
            int? pageIndex, 
            int pageSize = 5)
        {
            Expression<Func<ExperiencePost, bool>> _criteria = post => true;

            if (!string.IsNullOrWhiteSpace(title))
                _criteria = _criteria.AndAlso(post => post.Title.Contains(title));

            if (date.HasValue)
                _criteria = _criteria.AndAlso(post => post.CreatedAt.Date == date.Value.Date);

            if (!string.IsNullOrWhiteSpace(country))
                _criteria = _criteria.AndAlso(post => post.Country.Contains(country));

            if (!string.IsNullOrWhiteSpace(city))
                _criteria = _criteria.AndAlso(post => post.City.Contains(city));

            if (!string.IsNullOrWhiteSpace(tipsAndRecommendations))
                _criteria = _criteria.AndAlso(post => post.TipsAndRecommendations.Contains(tipsAndRecommendations));

            if (budget.HasValue)
                _criteria = _criteria.AndAlso(post => post.Budget <= budget.Value);

            crateria = _criteria;

            includes.Add(post => post.Comments);
            // Pagination
            if (pageIndex.HasValue && pageIndex > 0)
            {
                int skip = (pageIndex.Value - 1) * pageSize;
                ApplyPagination(skip, pageSize);
            }
            AddOrderByDecs(post => post.CreatedAt);
        }
    }



    public class Specification<T> : ISpecification<T> where T : BaseEntity
    {
        //.Where(p=>p.ID == id)
        public Expression<Func<T, bool>>? crateria { get; set; }
        public List<Expression<Func<T, object>>> includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> Orderby { get; set; }
        public Expression<Func<T, object>> OrderbyDecs { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagination { get; set; }
        protected void AndAlso(Expression<Func<T, bool>> expression)
        {
            var param = crateria.Parameters[0];
            var body = Expression.AndAlso(
                Expression.Invoke(crateria, param),
                Expression.Invoke(expression, param));
            crateria = Expression.Lambda<Func<T, bool>>(body, param);
        }
        public Specification()
        {
            //crateria = null;
        }

        public Specification(Expression<Func<T, bool>> _crateria)
        {
            crateria = _crateria;
        }

        public void AddOrderBy (Expression<Func<T, object>> _)
        {
            Orderby = _;
        }
        
        public void AddOrderByDecs (Expression<Func<T, object>> _)
        {
            OrderbyDecs = _;
        }

        public void ApplyPagination(int skip,int take)
        {
            Skip = skip;
            Take = take;
            IsPagination = true;
        }

    }

}

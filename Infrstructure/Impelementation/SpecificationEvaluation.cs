using Application.Abstraction.Specification;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Impelementation
{
    public static class SpecificationEvaluation<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            // 1. الشروط أولاً (Criteria)
            if (spec.crateria is not null)
                query = query.Where(spec.crateria);

            // 2. الربط ثانياً (Includes) - انقلها هنا قبل الترتيب والصفحات
            query = spec.includes.Aggregate(query,
                (current, include) => current.Include(include));

            query = spec.IncludeChains.Aggregate(
                query,
                (current, includeChain) =>
                    includeChain(current));
            // 3. الترتيب ثالثاً
            if (spec.Orderby is not null)
                query = query.OrderBy(spec.Orderby);
            else if (spec.OrderbyDecs is not null)
                query = query.OrderByDescending(spec.OrderbyDecs);




            // 4. الصفحات أخيراً
            if (spec.IsPagination)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }




    }
}

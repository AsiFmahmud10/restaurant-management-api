using ProductManagement.Common.Annotation;
using ProductManagement.Db;

namespace ProductManagement.Middilewire;
using System;

public class TransactionMiddleware(ApplicationDbContext dbContext) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint =  context.GetEndpoint();

        var metaData = endpoint?.Metadata.GetMetadata<TransactionAttribute>();

        if (metaData != null)
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync();
                await next(context);
                await dbContext.SaveChangesAsync();
                await dbContext.Database.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await dbContext.Database.RollbackTransactionAsync();
                throw;
            }
        }else
        {
            await next(context);
        }
            
    }
}
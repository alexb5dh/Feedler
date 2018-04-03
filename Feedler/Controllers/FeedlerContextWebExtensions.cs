using System;
using System.Threading.Tasks;
using Feedler.Extensions.MVC.ExceptionHandling;
using Microsoft.EntityFrameworkCore;

namespace Feedler.Controllers
{
    /// <summary>
    /// Extensions for EF to use on Web API layer.
    /// </summary>
    public static class FeedlerContextWebExtensions
    {
        /// <summary>
        /// Gets entity by <paramref name="id"/> from <paramref name="db"/> or throws <see cref="NotFoundException"/> if entity is not found.
        /// </summary>
        public static async Task<T> FindOrThrowAsync<T>(this DbSet<T> db, string id) where T: class =>
            await db.FindAsync(id) ?? throw new NotFoundException(typeof(T), id);

        /// <inheritdoc cref="FindOrThrowAsync{T}(DbSet{T},string)"/>
        public static async Task<T> FindOrThrowAsync<T>(this DbSet<T> db, Guid id) where T: class =>
            await db.FindAsync(id) ?? throw new NotFoundException(typeof(T), id);
    }
}
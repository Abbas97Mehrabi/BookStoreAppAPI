﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Extensions
{
    public static class BookRepositoryExtension
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books,
            uint minPrice, uint maxPrice)
        {
            return books.Where(book => book.Price >= minPrice && book.Price <= maxPrice);
        }
        public static IQueryable<Book> Search(this IQueryable<Book> books,
            string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return books;
            }
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return books.Where(b => b.Title
                .ToLower()
                .Contains(lowerCaseTerm));
        }
    }
}

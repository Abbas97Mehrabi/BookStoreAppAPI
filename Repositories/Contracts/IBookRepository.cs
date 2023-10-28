using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IBookRepository : IRepositoryBase<Book>
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(bool treckChanges);
        Task<Book> GetOneBookByIdAsync(int id ,bool treckChanges);
        void CreateOneBook (Book book);
        void UpdateOneBook(Book book);
        void DeleteOneBook(Book book);
    }
}

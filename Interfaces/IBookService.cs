namespace BookManagementAPI.Interfaces
{
    using BookManagementAPI.Models;
    using MongoDB.Bson;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBookService
    {
        Task<List<string>> GetBooksByPage(int page, int pageSize);
        Task<Books?> Get(ObjectId id);
        Task<(bool success, string? message)> Create(Books book);
        Task<List<string>> CreateMany(List<Books> books);
        Task Update(ObjectId id, Books book);
        Task SoftDelete(ObjectId id);
        Task SoftDeleteMany(List<ObjectId> ids);
    }
}
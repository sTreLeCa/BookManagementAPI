namespace BookManagementAPI.Interfaces
{
    using BookManagementAPI.Models;
    using MongoDB.Bson;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBookRepository
    {
        Task<List<string>> GetBooksByPage(int page, int pageSize);
        Task<Books?> GetById(ObjectId id);
        Task<Books?> GetByTitle(string title);
        Task Create(Books book);
        Task CreateMany(List<Books> books);
        Task Update(ObjectId id, Books book);
        Task SoftDelete(ObjectId id);
        Task SoftDeleteMany(List<ObjectId> ids);
        Task<List<string>> GetAllTitles();
        Task IncrementViewCount(ObjectId id);
    }
}
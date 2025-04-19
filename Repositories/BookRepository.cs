namespace BookManagementAPI.Repositories
{
    using BookManagementAPI.Interfaces;
    using BookManagementAPI.Models;
    using MongoDB.Driver;
    using MongoDB.Bson;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;

    public class BookRepository : IBookRepository
    {
        private readonly IMongoCollection<Books> _books;

        public BookRepository(IMongoDatabase database)
        {
            _books = database.GetCollection<Books>("books");
        }

        public async Task<List<string>> GetBooksByPage(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await _books
                .Find(x => !x.IsDeleted)
                .SortByDescending(x => x.ViewsCount)
                .Skip(skip)
                .Limit(pageSize)
                .Project(x => x.Title)
                .ToListAsync();
        }

        public async Task<Books?> GetById(ObjectId id)
        {
            return await _books
                .Find(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Books?> GetByTitle(string title)
        {
            return await _books
                .Find(x => x.Title == title)
                .FirstOrDefaultAsync();
        }

        public async Task Create(Books book) => 
            await _books.InsertOneAsync(book);

        public async Task CreateMany(List<Books> books) => 
            await _books.InsertManyAsync(books);

        public async Task Update(ObjectId id, Books book) => 
            await _books.ReplaceOneAsync(x => x.Id == id, book);

        public async Task SoftDelete(ObjectId id)
        {
            await _books.UpdateOneAsync(
                x => x.Id == id, 
                Builders<Books>.Update
                    .Set(x => x.IsDeleted, true)
                    .Set(x => x.DeletedDate, DateTime.UtcNow));
        }

        public async Task SoftDeleteMany(List<ObjectId> ids)
        {
            var filter = Builders<Books>.Filter.In(book => book.Id, ids);
            var update = Builders<Books>.Update
                .Set(book => book.IsDeleted, true)
                .Set(book => book.DeletedDate, DateTime.UtcNow);

            await _books.UpdateManyAsync(filter, update);
        }

        public async Task<List<string>> GetAllTitles() => 
            await _books.Find(_ => true).Project(x => x.Title).ToListAsync();

        // Separated responsibility for incrementing view count
        public async Task IncrementViewCount(ObjectId id)
        {
            await _books.UpdateOneAsync(
                Builders<Books>.Filter.Eq(x => x.Id, id),
                Builders<Books>.Update.Inc(x => x.ViewsCount, 1));
        }
    }
}
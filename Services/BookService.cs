namespace BookManagementAPI.Services
{
    using BookManagementAPI.Interfaces;
    using BookManagementAPI.Models;
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<string>> GetBooksByPage(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            
            var bookTitles = await _bookRepository.GetBooksByPage(page, pageSize);
            
            // Increment view count for returned books
            foreach (string title in bookTitles)
            {
                var book = await _bookRepository.GetByTitle(title);
                if (book != null)
                {
                    await _bookRepository.IncrementViewCount(book.Id);
                }
            }
            
            return bookTitles;
        }

        public async Task<Books?> Get(ObjectId id)
        {
            var book = await _bookRepository.GetById(id);

            if (book != null)
            {
                // Increment view count
                await _bookRepository.IncrementViewCount(id);
                
                // Re-fetch the book to get updated view count
                book = await _bookRepository.GetById(id);
                
                // Calculate popularity score
                book?.CalculatePopularityScore();
            }

            return book;
        }

        public async Task<(bool success, string? message)> Create(Books book)
        {
            var existingBook = await _bookRepository.GetByTitle(book.Title);
            
            if (existingBook != null && !existingBook.IsDeleted)
            {
                return (false, $"Book with this title {book.Title} already exists");
            }
            else if (existingBook != null && existingBook.IsDeleted)
            {
                book.Id = existingBook.Id;
                await _bookRepository.Update(existingBook.Id, book);
                return (true, null);
            }
            else 
            {
                await _bookRepository.Create(book);
                return (true, null);
            }
        }

        public async Task<List<string>> CreateMany(List<Books> books)
        {
            var warnings = new List<string>();

            foreach (var book in books)
            {
                var result = await Create(book);
                if (!result.success)
                {
                    warnings.Add(result.message ?? "Unknown error occurred");
                }
            }

            return warnings;
        }

        public async Task Update(ObjectId id, Books book) => 
            await _bookRepository.Update(id, book);

        public async Task SoftDelete(ObjectId id) => 
            await _bookRepository.SoftDelete(id);

        public async Task SoftDeleteMany(List<ObjectId> ids) => 
            await _bookRepository.SoftDeleteMany(ids);
    }
}
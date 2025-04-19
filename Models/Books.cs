namespace BookManagementAPI.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    public class Books
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("author name")]
        public string AuthorName { get; set; } = string.Empty;

        [BsonElement("publication year")]
        public int PublicationYear { get; set; }

        [BsonElement("views count")]
        private int _viewsCount = 0;
        
        public int ViewsCount 
        { 
            get => _viewsCount; 
            // Only allow internal setting of view count
            internal set => _viewsCount = value; 
        }

        [BsonElement("isDeleted")]
        public bool IsDeleted { get; internal set; } = false;

        [BsonElement("DeletedDate")]
        public DateTime? DeletedDate { get; internal set; } = null;

        [BsonIgnore]
        public double PopularityScore { get; private set; }

        // Move business logic to the model (encapsulation)
        public void CalculatePopularityScore()
        {
            PopularityScore = ViewsCount * 0.5 + (DateTime.UtcNow.Year - PublicationYear) * 2;
        }

        // Factory method to create a book (better encapsulation)
        public static Books Create(string title, string authorName, int publicationYear)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
                
            if (string.IsNullOrWhiteSpace(authorName))
                throw new ArgumentException("Author name cannot be empty", nameof(authorName));
                
            if (publicationYear <= 0)
                throw new ArgumentException("Publication year must be positive", nameof(publicationYear));
                
            return new Books
            {
                Title = title,
                AuthorName = authorName,
                PublicationYear = publicationYear
            };
        }
    }
}
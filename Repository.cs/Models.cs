using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class UserModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string SecuryStamp { get; set; }
    }

    public class NoteModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }

    public class PasswordResult
    {
        public string Hash { get; set; }
        public string SecurityStamp { get; set; }
    }
}

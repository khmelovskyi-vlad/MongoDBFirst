using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDBFirst
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var db = new MangoCRUD("AddressBook");
            //var person = new PersonModel()
            //{
            //    FirstName = "SomeFirstName",
            //    LastName = "SomeLastName",
            //    PrimaryAddress = new AddressModel()
            //    {
            //        StreetAddress = "some street",
            //        City = "some city",
            //        State = "PA",
            //        ZipCode = "234234"
            //    }
            //};

            //await db.InsertRecord("Users", person);

            //var records = await db.LoadRecords<PersonModel>("Users");
            //foreach (var rec in records)
            //{
            //    Console.WriteLine($"{ rec.Id }: {rec.FirstName} {rec.LastName}");
            //    if (rec.PrimaryAddress != null)
            //    {
            //        Console.WriteLine(rec.PrimaryAddress.City);
            //    }
            //    Console.WriteLine();
            //}

            //var oneRec = await db.LoadRecordById<PersonModel>("Users", new Guid("dfec7b56-2f92-408e-95a7-9527541e94f2"));
            //oneRec.DateOfBirth = new DateTime(1923, 10, 20, 0, 0, 0, DateTimeKind.Utc);
            //await db.UpsertRecord("Users", oneRec.Id, oneRec);
            //await db.DeleteRecord<PersonModel>("Users", oneRec.Id);

            var records = await db.LoadRecords<NameModel>("Users");
            foreach (var rec in records)
            {
                Console.WriteLine($"{rec.FirstName} {rec.LastName}");
                Console.WriteLine();
            }

            Console.WriteLine("Hello World!");
        }
    }

    public class NameModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressModel PrimaryAddress { get; set; }
        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }
    }

    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class MangoCRUD
    {
        private IMongoDatabase db;
        public MangoCRUD(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);

        }

        public async Task InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            await collection.InsertOneAsync(record);
        }
        public async Task<List<T>> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return await (await collection.FindAsync(new BsonDocument())).ToListAsync();
        }
        public async Task<T> LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await (await collection.FindAsync(filter)).FirstAsync();
        }
        public async Task UpsertRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);
            var result = await collection.ReplaceOneAsync(new BsonDocument("_id", BsonValue.Create(id)), record, new ReplaceOptions { IsUpsert = true });
        }
        public async Task DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            await collection.DeleteOneAsync(filter);
        }
    }
}

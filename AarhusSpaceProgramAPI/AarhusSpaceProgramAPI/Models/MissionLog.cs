using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AarhusSpaceProgramAPI.Models;

public class MissionLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id  { get; set; }
    public string MissionId  { get; set; }
    public string Message { get; set; }
    public DateTime? Timestamp { get; set; }
}
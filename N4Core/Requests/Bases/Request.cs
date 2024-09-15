using AutoMapper;
using N4Core.Records.Bases;
using N4Core.Requests.Enums;

namespace N4Core.Requests.Bases
{
    public class Request : Record
    {
        public RequestOperations Operation { get; }
        public Profile[]? MapperProfiles { get; private set; }

        public Request(RequestOperations operation)
        {
            Operation = operation;
        }

        public void SetMapperProfiles(params Profile[] mapperProfiles) => MapperProfiles = mapperProfiles?.ToArray();
    }
}

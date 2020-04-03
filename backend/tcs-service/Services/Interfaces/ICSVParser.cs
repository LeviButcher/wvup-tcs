using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace tcs_service.Services {
    ///<summary>Parses a CSV File into a List of a specifc type</summary>
    public interface ICSVParser<T> {
        ///<summary>Parses a CSV File into a List of a specifc type</summary>
        IEnumerable<T> Parse (IFormFile file);
    }
}
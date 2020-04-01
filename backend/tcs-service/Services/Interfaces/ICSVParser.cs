using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace tcs_service.Services
{
    public interface ICSVParser<T>
    {
        IEnumerable<T> Parse(IFormFile file);
    }
}
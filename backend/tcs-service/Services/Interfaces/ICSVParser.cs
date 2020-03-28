using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

public interface ICSVParser<T>
{
    IEnumerable<T> Parse(IFormFile file);
}
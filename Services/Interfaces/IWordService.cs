using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for loading word lists by category
/// </summary>
public interface IWordService
{

    Task<List<string>> GetWordsByCategoryAsync(string category);


    Task<List<string>> GetAllCategoriesAsync();


    Task<string> GetRandomWordAsync(string category);
}

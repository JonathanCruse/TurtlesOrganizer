using TurtlesOrganizer.Application.DTOs;

namespace TurtlesOrganizer.Application.Services;

public interface IPersonService
{
    Task<PersonDto> CreatePersonAsync(CreatePersonDto dto);
    Task<PersonDto?> GetPersonByIdAsync(Guid id);
    Task<IEnumerable<PersonDto>> GetAllPersonsAsync();
    Task<IEnumerable<PersonDto>> GetTrainersAsync();
}

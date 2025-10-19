using TurtlesOrganizer.Application.DTOs;
using TurtlesOrganizer.Domain.Entities;
using TurtlesOrganizer.Domain.Repositories;
using TurtlesOrganizer.Domain.ValueObjects;

namespace TurtlesOrganizer.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;

    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<PersonDto> CreatePersonAsync(CreatePersonDto dto)
    {
        var email = Email.Create(dto.Email);
        var person = new Person(dto.FullName, email, dto.MembershipId, dto.IsTrainer);
        await _personRepository.AddAsync(person);
        return MapToDto(person);
    }

    public async Task<PersonDto?> GetPersonByIdAsync(Guid id)
    {
        var person = await _personRepository.GetByIdAsync(id);
        return person != null ? MapToDto(person) : null;
    }

    public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
    {
        var persons = await _personRepository.GetAllAsync();
        return persons.Select(MapToDto);
    }

    public async Task<IEnumerable<PersonDto>> GetTrainersAsync()
    {
        var persons = await _personRepository.GetAllAsync();
        return persons.Where(p => p.IsTrainer).Select(MapToDto);
    }

    private PersonDto MapToDto(Person person) => new PersonDto(
        person.Id,
        person.FullName,
        person.Email.Value,
        person.MembershipId,
        person.IsMember,
        person.IsTrainer
    );
}

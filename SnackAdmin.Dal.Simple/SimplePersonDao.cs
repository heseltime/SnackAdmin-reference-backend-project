using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Simple;

public class SimplePersonDao : IPersonDao
{
    private static IList<Person> personList = new List<Person>
    {
        new Person(1, "John", "Doe", DateTime.Now.AddYears(-10)),
        new Person(2, "Jane", "Doe", DateTime.Now.AddYears(-20)),
        new Person(3, "Max", "Mustermann", DateTime.Now.AddYears(-30))
    };

    public Task<bool> DeleteAsync(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> FindAllAsync()
    {
        return Task.FromResult<IEnumerable<Person>>(personList);
    }

    public Task<Person?> FindByIdAsync(int id)
    {
        
        return Task.FromResult(personList.SingleOrDefault(p => p.Id == id));
    }

    public Task<bool> InsertAsync(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(Person person)
    {
        var existingPerson = await FindByIdAsync(person.Id);
        if (existingPerson is null)
        {
            return false;
        }

        personList.Remove(existingPerson);
        personList.Add(person);
        return true;
    }

    public Task<bool> UpdateAsync(IEntity entity)
    {
        throw new NotImplementedException();
    }
}

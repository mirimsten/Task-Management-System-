namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class EngineerImplementation : IEngineer
{
    public int Create(Engineer _engineer)
    {
        if (DataSource.Engineers.FirstOrDefault(e => e?.Id == _engineer.Id) != null)
        {
            throw new DalAlreadyExistsException($"The new engineer cannot be created, an engineer with ID: {_engineer.Id} already exists in the system.");
        }
        DataSource.Engineers.Add(_engineer);
        return _engineer.Id;
    }

    public void Reset()
    {
        DataSource.Engineers.Clear();
        DataSource.Engineers.Add(new Engineer(325907210, "admin", "admin@gmail.com", EngineerExperience.Expert, 1500));
;
    }

    public void Delete(int id)
    {
        if (DataSource.Engineers.FirstOrDefault(e => e?.Id == id) == null)
        {
            throw new DalDoesNotExistException($"Can't delete, engineer with ID: {id} does not exist!!");
        }
        DataSource.Engineers.RemoveAll(e => e!.Id == id);
    }

    public Engineer? Read(int id)
    {
        return DataSource.Engineers.FirstOrDefault(e => e?.Id == id);
    }

    public Engineer? Read(Func<Engineer, bool> filter)
    {
        return DataSource.Engineers.FirstOrDefault(filter);
    }

    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        if (filter == null)
            return DataSource.Engineers.Select(e => e);
        else
            return DataSource.Engineers.Where(filter);
    }

    public void Update(Engineer _engineer)
    {
        if (DataSource.Engineers.FirstOrDefault(e => e!.Id == _engineer.Id) == null)
        {
            throw new DalDoesNotExistException($"Can't update, engineer with ID: {_engineer?.Id} does not exist!!");
        }
        DataSource.Engineers.RemoveAll(e => e?.Id == _engineer.Id);
        DataSource.Engineers.Add(_engineer);
    }
}

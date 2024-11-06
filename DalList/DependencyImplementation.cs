namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class DependencyImplementation : IDependency
{
    public int Create(Dependency _dependency)
    {
        int newId = DataSource.Config.NextDependencyId;
        Dependency dependency = _dependency with { Id = newId };
        DataSource.Dependencies.Add(dependency);
        return newId;
    }

    public void Reset()
    {
        DataSource.Dependencies.Clear();
    }

    public void Delete(int id)
    {
        if (DataSource.Dependencies.FirstOrDefault(d => d?.Id == id) == null)
        {
            throw new DalDoesNotExistException($"Can't delete, Dependency with ID: {id} does not exist!!");
        }
        DataSource.Dependencies.RemoveAll(d => d?.Id == id);

    }

    public Dependency? Read(int id)
    {
        return DataSource.Dependencies.FirstOrDefault(d => d?.Id == id);
    }

    public Dependency? Read(Func<Dependency, bool> filter)
    {
        return DataSource.Dependencies.FirstOrDefault(filter!);
    }

    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
    {
        if (filter == null)
            return DataSource.Dependencies;
        else
            return DataSource.Dependencies.Where(filter!);
    }

    public void Update(Dependency _dependency)
    {
        if (DataSource.Dependencies.FirstOrDefault(d => d?.Id == _dependency.Id) == null)
        {
            throw new DalDoesNotExistException($"Can't update, Dependency with ID: {_dependency?.Id} does not exist!!");
        }
        DataSource.Dependencies.RemoveAll(d => d?.Id == _dependency.Id);
        DataSource.Dependencies.Add(_dependency);
    }

}
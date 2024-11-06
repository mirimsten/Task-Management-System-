namespace Dal;
using DalApi;
using DO;



internal class EngineerImplementation : IEngineer
{
    const string engineerRoot = "engineers"; //XML Serializer
    public int Create(Engineer engineer)
    {
        var engineersList = XMLTools.LoadListFromXMLSerializer<Engineer>(engineerRoot);
        if (engineersList.Exists(e => e?.Id == engineer?.Id))
            throw new DalAlreadyExistsException($"The new engineer cannot be created, an engineer with ID: {engineer.Id} already exists in the system.");
        engineersList.Add(engineer);
        XMLTools.SaveListToXMLSerializer(engineersList, engineerRoot);
        return engineer.Id;
    }

    public void Reset()
    {
        var engineersList = XMLTools.LoadListFromXMLSerializer<Engineer>(engineerRoot);
        engineersList.Clear();
        engineersList.Add(new Engineer(325907210, "admin", "admin@gmail.com", EngineerExperience.Expert, 1500));
        XMLTools.SaveListToXMLSerializer(engineersList, engineerRoot);
    }

    public void Delete(int id)
    {
        var engineersList = XMLTools.LoadListFromXMLSerializer<Engineer>(engineerRoot);
        if (engineersList.RemoveAll(e => e?.Id == id) == 0)
            throw new DalDoesNotExistException($"Can't delete, engineer with ID: {id} does not exist!!");
        XMLTools.SaveListToXMLSerializer(engineersList, engineerRoot);
    }

    public Engineer? Read(int id) =>
        XMLTools.LoadListFromXMLSerializer<Engineer>(engineerRoot).FirstOrDefault(e => e?.Id == id) ?? null;

    public Engineer? Read(Func<Engineer, bool> filter) =>
        XMLTools.LoadListFromXMLSerializer<Engineer>(engineerRoot).FirstOrDefault(filter) ?? null;

    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        var engineersList = XMLTools.LoadListFromXMLSerializer<Engineer>(engineerRoot);
        return filter == null ? engineersList.Select(e => e) : engineersList.Where(filter);
    }

    public void Update(Engineer e)
    {
        Delete(e.Id);
        Create(e);
    }

}

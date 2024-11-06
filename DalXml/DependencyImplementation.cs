namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class DependencyImplementation : IDependency
{
    const string dependencyRoot = "dependencies"; // XElement
    public int Create(Dependency dependency)
    {
        XElement dependencyElement = XMLTools.LoadListFromXMLElement(dependencyRoot);
        int runId = Config.NextDependencyId;
        XElement id = new("Id", runId);
        XElement dependentTask = new("DependentTask", dependency.DependentTask);
        XElement dependsOnTask = new("DependsOnTask", dependency.DependsOnTask);
        dependencyElement.Add(new XElement("Dependency", id, dependentTask, dependsOnTask));
        XMLTools.SaveListToXMLElement(dependencyElement, dependencyRoot);
        return runId;
    }

    public void Reset()
    {
        XElement dependencyElement = XMLTools.LoadListFromXMLElement(dependencyRoot);
        dependencyElement.Elements().Remove();
        XMLTools.SaveListToXMLElement(dependencyElement, dependencyRoot);
        XMLTools.Set("data-config", "NextDependencyId", 1);

    }

    public void Delete(int id)
    {
        XElement dependencyElement = XMLTools.LoadListFromXMLElement(dependencyRoot);
        (dependencyElement.Elements().FirstOrDefault(d => (int?)d.Element("Id") == id)
            ?? throw new DalDoesNotExistException($"Can't delete, Dependency with ID: {id} does not exist!!")).Remove();
        XMLTools.SaveListToXMLElement(dependencyElement, dependencyRoot);
    }

    public Dependency? Read(int id) => XMLTools.ToDependency(
         XMLTools.LoadListFromXMLElement(dependencyRoot)!.Elements().FirstOrDefault(d =>

            (int?)d.Element("Id") == id
         )!
);

    public Dependency? Read(Func<Dependency, bool> filter) => XMLTools.LoadListFromXMLElement(dependencyRoot).Elements()
        .Select(e => XMLTools.ToDependency(e)).FirstOrDefault(filter);

    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null) =>
        XMLTools.LoadListFromXMLElement(dependencyRoot).Elements().Select(e => XMLTools.ToDependency(e)).Where(filter is null? x=>true: filter);
        
    public void Update(Dependency dependency)
    {
        Delete(dependency.Id);
        Create(dependency);
    }

}

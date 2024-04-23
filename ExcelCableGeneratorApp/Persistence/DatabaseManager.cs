using DocumentFormat.OpenXml.Drawing;
using ExcelCableGeneratorApp.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ExcelCableGeneratorApp.Persistence;

internal class DatabaseManager
{
    public DatabaseManager()
    {
    }

    public bool Create(Job job)
    {
        using var db = new CableJobContext();
        Debug.WriteLine($"Database path: {db.DbPath}."); // Note: This sample requires the database to be created before running.

        if (Read(job.Name) != null) 
        {
            Debug.WriteLine($"There was already a job with the name: {job.Name}");
            return false;
        }

        Debug.WriteLine($"Inserting a new job with name: {job.Name}");
        db.Add(job);
        db.SaveChanges();
        
        return true;
    }

    public Job? Read(string jobName)
    {
        using var db = new CableJobContext();
        Debug.WriteLine($"Database path: {db.DbPath}."); // Note: This sample requires the database to be created before running.

        Console.WriteLine("Querying for a job");
        //var job = db.Jobs.Where(j => j.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        var job = db.Jobs.FirstOrDefault(j => EF.Functions.Collate(j.Name, "NOCASE") == jobName);

        return job;
    }

    public List<string> AllJobNames()
    {
        using var db = new CableJobContext();
        Debug.WriteLine($"Database path: {db.DbPath}."); // Note: This sample requires the database to be created before running.

        Console.WriteLine("Getting list of job names");

        return [.. db.Jobs.Select(j => j.Name)];
    }
}
